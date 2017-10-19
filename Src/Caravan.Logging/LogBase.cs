using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Logging
{
    public abstract class LogBase : ILog
    {
        protected string LogName { get; private set; }

        public LogBase()
        {
            LogName = this.GetType().FullName;
        }

        public LogBase(string logName)
        {
            LogName = logName;
        }

        protected abstract void Log(string level, string title, string message, Dictionary<string, string> tags);
        
        #region Implementation of ILogger
        
        /// <summary>
        /// Write a log in debug level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        public void Debug(string title, string message)
        {
            Log("DEBUG", title, message, null);
        }

        /// <summary>
        /// Write an exception log in debug level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        public void Debug(string title, Exception exception)
        {
            Log("DEBUG", title, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in debug level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Debug(string title, string message, Dictionary<string, string> tags)
        {
            Log("DEBUG", title, message, tags);
        }

        /// <summary>
        /// Write an exception log in debug level with some extra attributes
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        public void Debug(string title, Exception exception, Dictionary<string, string> tags)
        {
            Log("DEBUG", title, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in debug level.
        /// </summary>
        /// <param name="message">log message.</param>
        public void Debug(string message)
        {
            Log("DEBUG", null, message, null);
        }

        /// <summary>
        /// Write an exception log in debug level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        public void Debug(Exception exception)
        {
            Log("DEBUG", null, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in debug level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Debug(string message, Dictionary<string, string> tags)
        {
            Log("DEBUG", null, message, tags);
        }

        /// <summary>
        /// Write an exception log in debug level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        public void Debug(Exception exception, Dictionary<string, string> tags)
        {
            Log("DEBUG", null, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in info level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        public void Info(string title, string message)
        {
            Log("INFO", title, message, null);
        }

        /// <summary>
        /// Write an exception log in info level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        public void Info(string title, Exception exception)
        {
            Log("INFO", title, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in info level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Info(string title, string message, Dictionary<string, string> tags)
        {
            Log("INFO", title, message, tags);
        }

        /// <summary>
        /// Write an exception log in info level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        public void Info(string title, Exception exception, Dictionary<string, string> tags)
        {
            Log("INFO", title, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in info level.
        /// </summary>
        /// <param name="message">log message.</param>
        public void Info(string message)
        {
            Log("INFO", null, message, null);
        }

        /// <summary>
        /// Write an exception log in info level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        public void Info(Exception exception)
        {
            Log("INFO", null, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in info level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Info(string message, Dictionary<string, string> tags)
        {
            Log("INFO", null, message, tags);
        }

        /// <summary>
        /// Write an exception log in info level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        public void Info(Exception exception, Dictionary<string, string> tags)
        {
            Log("INFO", null, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in warn level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        public void Warn(string title, string message)
        {
            Log("WARN", title, message, null);
        }

        /// <summary>
        /// Write an exception log in warn level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        public void Warn(string title, Exception exception)
        {
            Log("WARN", title, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in warn level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Warn(string title, string message, Dictionary<string, string> tags)
        {
            Log("WARN", title, message, tags);
        }

        /// <summary>
        /// Write an exception log in warn log with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        public void Warn(string title, Exception exception, Dictionary<string, string> tags)
        {
            Log("WARN", title, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in warn level.
        /// </summary>
        /// <param name="message">log message.</param>
        public void Warn(string message)
        {
            Log("WARN", null, message, null);
        }

        /// <summary>
        /// Write an exception log in warn level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        public void Warn(Exception exception)
        {
            Log("WARN", null, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in warn level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">  kv pairs</param>
        public void Warn(string message, Dictionary<string, string> tags)
        {
            Log("WARN", null, message, tags);
        }

        /// <summary>
        /// Write an exception log in warn log with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        public void Warn(Exception exception, Dictionary<string, string> tags)
        {
            Log("WARN", null, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in error level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        public void Error(string title, string message)
        {
            Log("ERROR", title, message, null);
        }

        /// <summary>
        /// Write an exception log in error level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        public void Error(string title, Exception exception)
        {
            Log("ERROR", title, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in error level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Error(string title, string message, Dictionary<string, string> tags)
        {
            Log("ERROR", title, message, tags);
        }

        /// <summary>
        /// Write an exception log in error level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        public void Error(string title, Exception exception, Dictionary<string, string> tags)
        {
            Log("ERROR", title, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in error level.
        /// </summary>
        /// <param name="message">log message.</param>
        public void Error(string message)
        {
            Log("ERROR", null, message, null);
        }

        /// <summary>
        /// Write an exception log in error level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        public void Error(Exception exception)
        {
            Log("ERROR", null, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in error level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Error(string message, Dictionary<string, string> tags)
        {
            Log("ERROR", null, message, tags);
        }

        /// <summary>
        /// Write an exception log in error level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        public void Error(Exception exception, Dictionary<string, string> tags)
        {
            Log("ERROR", null, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in fatal level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        public void Fatal(string title, string message)
        {
            Log("FATAL", title, message, null);
        }

        /// <summary>
        /// Write an exception log in fatal level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        public void Fatal(string title, Exception exception)
        {
            Log("FATAL", title, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Fatal(string title, string message, Dictionary<string, string> tags)
        {
            Log("FATAL", title, message, tags);
        }

        /// <summary>
        /// Write an exception log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        public void Fatal(string title, Exception exception, Dictionary<string, string> tags)
        {
            Log("FATAL", title, exception.ToString(), tags);
        }

        /// <summary>
        /// Write a log in fatal level.
        /// </summary>
        /// <param name="message">log message.</param>
        public void Fatal(string message)
        {
            Log("FATAL", null, message, null);
        }

        /// <summary>
        /// Write an exception log in fatal level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        public void Fatal(Exception exception)
        {
            Log("FATAL", null, exception.ToString(), null);
        }

        /// <summary>
        /// Write a log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        public void Fatal(string message, Dictionary<string, string> tags)
        {
            Log("FATAL", null, message, tags);
        }

        /// <summary>
        /// Write an exception log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        public void Fatal(Exception exception, Dictionary<string, string> tags)
        {
            Log("FATAL", null, exception.ToString(), tags);
        }

        #endregion
    }
}
