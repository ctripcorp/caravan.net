using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    class DefaultServerSourceManager : IServerSourceManager
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultServerSourceManager));

        private ILoadBalancerContext _loadBalancerContext;

        private IProperty _dataFolder;
        private IProperty _globalDataFolder;

        private DataContractJsonSerializer _serializer;

        public event EventHandler<ServerSourceRestoreEvent> OnServerSourceRestore;

        public DefaultServerSourceManager(ILoadBalancerContext loadBalancerContext)
        {
            _loadBalancerContext = loadBalancerContext;
            _serializer = new DataContractJsonSerializer(typeof(List<LoadBalancerRoute>));

            string prefix = _loadBalancerContext.LoadBalancerKey;
            _dataFolder = _loadBalancerContext.ConfigurationManager.GetProperty(prefix + "." + ConfigurationKeys.DataFolder);

            string globalPrefix = _loadBalancerContext.ManagerId;
            _globalDataFolder = _loadBalancerContext.ConfigurationManager.GetProperty(globalPrefix + "." + ConfigurationKeys.DataFolder);
        }

        private string GetDataFolder()
        {
            string dataFolder = _dataFolder.Value;
            if (string.IsNullOrWhiteSpace(dataFolder))
                dataFolder = _globalDataFolder.Value;

            return dataFolder;
        }

        private string GetBackupFileName(string dataFolder)
        {
            if (string.IsNullOrWhiteSpace(dataFolder))
                return null;

            return Path.Combine(dataFolder, _loadBalancerContext.LoadBalancerKey);
        }

        public void Backup(List<LoadBalancerRoute> routes)
        {
            lock (this)
            {
                FileStream fileStream = null;
                string dataFolder = "";
                string fileName = "";
                try
                {
                    dataFolder = GetDataFolder();
                    fileName = GetBackupFileName(dataFolder);
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        _logger.Info("Back up file name is null or empty! Backup canceled.", _loadBalancerContext.AdditionalInfo);
                        return;
                    }

                    if (!Directory.Exists(dataFolder))
                        Directory.CreateDirectory(dataFolder);

                    fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    _serializer.WriteObject(fileStream, routes);
                    fileStream.Flush();
                }
                catch (Exception ex)
                {
                    _logger.Info("Error occurred while backing up server source data. FileName:" + fileName, ex, _loadBalancerContext.AdditionalInfo);
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                }
            }
        }

        public List<LoadBalancerRoute> Restore()
        {
            lock (this)
            {
                FileStream fileStream = null;
                try
                {
                    string dataFolder = GetDataFolder();
                    string fileName = GetBackupFileName(dataFolder);
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        _logger.Info("Back up file name is null or empty! Backup canceled.", _loadBalancerContext.AdditionalInfo);
                        return new List<LoadBalancerRoute>();
                    }
                    if (!File.Exists(fileName))
                    {
                        _logger.Info("Back up file(" + fileName + ") is not exist! Restore canceled.", _loadBalancerContext.AdditionalInfo);
                        return new List<LoadBalancerRoute>();
                    }

                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    var routes = (List<LoadBalancerRoute>)_serializer.ReadObject(fileStream);
                    routes = routes.FilterInvalidEntities(_logger, _loadBalancerContext.AdditionalInfo);
                    ServerSourceRestoreEvent e = new ServerSourceRestoreEvent(routes);
                    RaiseServerSourceRestoreEvent(e);
                    return e.Routes;
                }
                catch (Exception ex)
                {
                    _logger.Info("Error occurred while restoring server source data", ex, _loadBalancerContext.AdditionalInfo);
                    return new List<LoadBalancerRoute>();
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                }
            }
        }

        private void RaiseServerSourceRestoreEvent(ServerSourceRestoreEvent e)
        {
            if (OnServerSourceRestore == null)
                return;

            try
            {
                OnServerSourceRestore(this, e);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while raising ServerSourceRestoreEvent.", ex, _loadBalancerContext.AdditionalInfo);
            }
        }
    }
}
