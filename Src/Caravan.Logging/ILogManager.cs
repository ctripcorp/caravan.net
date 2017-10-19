using System;

namespace Com.Ctrip.Soa.Caravan.Logging
{
    /// <summary>
    /// Interface ILogManager
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Return the name of the logger manager specifying the type of logger it manages.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Return a logger named corresponding to the class passed as parameter, using
        /// the statically bound <see cref="ILogManager"/> instance.
        /// </summary>
        /// <param name="type">the returned logger will be named after the type</param>
        /// <returns>logger</returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Return an appropriate <see cref="ILog"/> instance as specified by the
        /// <code>name</code> parameter.
        /// Null-valued name arguments are considered invalid.
        /// </summary>
        /// <param name="name">the name of the Logger to return</param>
        /// <returns>a Logger instance</returns>
        ILog GetLogger(string name);
    }
}