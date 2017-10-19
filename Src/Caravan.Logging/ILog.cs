using System.Collections.Generic;

namespace Com.Ctrip.Soa.Caravan.Logging
{
    /// <summary>
    /// Interface ILog
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write a log in debug level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        void Debug(string title, string message);

        /// <summary>
        /// Write an exception log in debug level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        void Debug(string title, System.Exception exception);

        /// <summary>
        /// Write a log in debug level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Debug(string title, string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in debug level with some extra attributes
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        void Debug(string title, System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in debug level.
        /// </summary>
        /// <param name="message">log message.</param>
        void Debug(string message);

        /// <summary>
        /// Write an exception log in debug level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        void Debug(System.Exception exception);

        /// <summary>
        /// Write a log in debug level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Debug(string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in debug level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        void Debug(System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in info level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        void Info(string title, string message);

        /// <summary>
        /// Write an exception log in info level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        void Info(string title, System.Exception exception);

        /// <summary>
        /// Write a log in info level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Info(string title, string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in info level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        void Info(string title, System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in info level.
        /// </summary>
        /// <param name="message">log message.</param>
        void Info(string message);

        /// <summary>
        /// Write an exception log in info level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        void Info(System.Exception exception);

        /// <summary>
        /// Write a log in info level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Info(string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in info level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        void Info(System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in warn level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        void Warn(string title, string message);

        /// <summary>
        /// Write an exception log in warn level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        void Warn(string title, System.Exception exception);

        /// <summary>
        /// Write a log in warn level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Warn(string title, string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in warn log with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        void Warn(string title, System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in warn level.
        /// </summary>
        /// <param name="message">log message.</param>
        void Warn(string message);

        /// <summary>
        /// Write an exception log in warn level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        void Warn(System.Exception exception);

        /// <summary>
        /// Write a log in warn level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">  kv pairs</param>
        void Warn(string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in warn log with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        void Warn(System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in error level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        void Error(string title, string message);

        /// <summary>
        /// Write an exception log in error level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        void Error(string title, System.Exception exception);

        /// <summary>
        /// Write a log in error level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Error(string title, string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in error level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        void Error(string title, System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in error level.
        /// </summary>
        /// <param name="message">log message.</param>
        void Error(string message);

        /// <summary>
        /// Write an exception log in error level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        void Error(System.Exception exception);

        /// <summary>
        /// Write a log in error level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Error(string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in error level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        void Error(System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in fatal level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="message">log message.</param>
        void Fatal(string title, string message);

        /// <summary>
        /// Write an exception log in fatal level.
        /// </summary>
        /// <param name="title">log title.</param>
        /// <param name="exception">an Exception instance.</param>
        void Fatal(string title, System.Exception exception);

        /// <summary>
        /// Write a log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Fatal(string title, string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="title">log title</param>
        /// <param name="exception">an Exception instance to log</param>
        /// <param name="tags">kv pairs</param>
        void Fatal(string title, System.Exception exception, Dictionary<string, string> tags);

        /// <summary>
        /// Write a log in fatal level.
        /// </summary>
        /// <param name="message">log message.</param>
        void Fatal(string message);

        /// <summary>
        /// Write an exception log in fatal level.
        /// </summary>
        /// <param name="exception">an Exception instance.</param>
        void Fatal(System.Exception exception);

        /// <summary>
        /// Write a log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="tags">kv pairs</param>
        void Fatal(string message, Dictionary<string, string> tags);

        /// <summary>
        /// Write an exception log in fatal level with some extra attributes.
        /// </summary>
        /// <param name="exception">an Exception instance</param>
        /// <param name="tags">kv pairs</param>
        void Fatal(System.Exception exception, Dictionary<string, string> tags);
    }
}