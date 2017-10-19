using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Logging.Null;

namespace Com.Ctrip.Soa.Caravan.Logging
{
    /// <summary>
    /// LogManager 类
    /// </summary>
    public static class LogManager
    {
        private static ILogManager _currentManager;

        public static ILogManager CurrentManager 
        {
            get
            {
                return _currentManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _currentManager = value;
            }
        }

        static LogManager()
        {
            _currentManager = Com.Ctrip.Soa.Caravan.Logging.Null.NullLogManager.Instance;
        }

        /// <summary>
        /// Return a ILog named corresponding to the class passed as parameter, using
        /// the statically bound <see cref="ILogManager"/> instance.
        /// </summary>
        /// <param name="type">the returned ILog will be named after class</param>
        /// <returns>logger</returns>
        public static ILog GetLogger(Type type)
        {
            return _currentManager.GetLogger(type);
        }

        /// <summary>
        /// Return an appropriate <see cref="ILog"/> instance as specified by the
        /// <code>name</code> parameter.
        /// <p/>
        /// Null-valued name arguments are considered invalid.
        /// </summary>
        /// <param name="name">the name of the Logger to return</param>
        /// <returns>a Logger instance</returns>
        public static ILog GetLogger(string name)
        {
            return _currentManager.GetLogger(name);
        }
    }
}