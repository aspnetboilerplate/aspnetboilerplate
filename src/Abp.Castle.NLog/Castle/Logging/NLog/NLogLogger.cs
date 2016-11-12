using NLog;

namespace Abp.Castle.Logging.NLog
{
    //[Serializable]
    using System;
    using NLog;

    /// <summary>
    ///   Implementation of <see cref="global::Castle.Core.Logging.ILogger" /> for NLog.
    /// </summary>
    public class NLogLogger : global::Castle.Core.Logging.ILogger
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NLogLogger" /> class.
        /// </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="factory"> The factory. </param>
        public NLogLogger(Logger logger, NLogLoggerFactory factory)
        {
            Logger = logger;
            Factory = factory;
        }

        internal NLogLogger()
        {
        }

        /// <summary>
        ///   Determines if messages of priority "debug" will be logged.
        /// </summary>
        /// <value> True if "debug" messages will be logged. </value>
        public bool IsDebugEnabled
        {
            get { return Logger.IsDebugEnabled; }
        }

        /// <summary>
        ///   Determines if messages of priority "error" will be logged.
        /// </summary>
        /// <value> <c>true</c> if "error" messages will be logged, <c>false</c> otherwise </value>
        public bool IsErrorEnabled
        {
            get { return Logger.IsErrorEnabled; }
        }

        /// <summary>
        ///   Determines if messages of priority "fatal" will be logged.
        /// </summary>
        /// <value> <c>true</c> if "fatal" messages will be logged, <c>false</c> otherwise </value>
        public bool IsFatalEnabled
        {
            get { return Logger.IsFatalEnabled; }
        }

        /// <summary>
        ///   Determines if messages of priority "info" will be logged.
        /// </summary>
        /// <value> <c>true</c> if "info" messages will be logged, <c>false</c> otherwise </value>
        public bool IsInfoEnabled
        {
            get { return Logger.IsInfoEnabled; }
        }

        /// <summary>
        ///   Determines if messages of priority "warn" will be logged.
        /// </summary>
        /// <value> <c>true</c> if "warn" messages will be logged, <c>false</c> otherwise </value>
        public bool IsWarnEnabled
        {
            get { return Logger.IsWarnEnabled; }
        }

        /// <summary>
        ///   Gets or sets the factory.
        /// </summary>
        /// <value> The factory. </value>
        protected internal NLogLoggerFactory Factory { get; set; }

        /// <summary>
        ///   Gets or sets the logger.
        /// </summary>
        /// <value> The logger. </value>
        protected internal Logger Logger { get; set; }

        /// <summary>
        ///   Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            return Logger.ToString();
        }

        /// <summary>
        ///   Creates a child logger with the specied <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns> </returns>
        public virtual global::Castle.Core.Logging.ILogger CreateChildLogger(String name)
        {
            return Factory.Create(Logger.Name + "." + name);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="message"> The message to log </param>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="messageFactory"> Factory constructing lazily the message to log if the level is enabled </param>
        public void Debug(Func<string> messageFactory)
        {
            if (IsDebugEnabled == false)
            {
                return;
            }
            Log(LogLevel.Debug, messageFactory());
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="message"> The message to log </param>
        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void DebugFormat(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Debug, exception, format, args);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Debug, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs a debug message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Debug, exception, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="message"> The message to log </param>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="messageFactory"> Factory constructing lazily the message to log if the level is enabled </param>
        public void Error(Func<string> messageFactory)
        {
            if (IsErrorEnabled == false)
            {
                return;
            }
            Log(LogLevel.Error, messageFactory());
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="message"> The message to log </param>
        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void ErrorFormat(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Error, exception, format, args);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Error, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs an error message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Error, exception, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="message"> The message to log </param>
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="messageFactory"> Factory constructing lazily the message to log if the level is enabled </param>
        public void Fatal(Func<string> messageFactory)
        {
            if (IsFatalEnabled == false)
            {
                return;
            }
            Log(LogLevel.Fatal, messageFactory());
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="message"> The message to log </param>
        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void FatalFormat(string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Fatal, exception, format, args);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Fatal, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs a fatal message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Fatal, exception, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="message"> The message to log </param>
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        ///   Logs a info message.
        /// </summary>
        /// <param name="messageFactory"> Factory constructing lazily the message to log if the level is enabled </param>
        public void Info(Func<string> messageFactory)
        {
            if (IsInfoEnabled == false)
            {
                return;
            }
            Log(LogLevel.Info, messageFactory());
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="message"> The message to log </param>
        public void Info(string message, Exception exception)
        {
            Log(LogLevel.Info, message, exception);
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void InfoFormat(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Info, exception, format, args);
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Info, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs an info message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Info, exception, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="message"> The message to log </param>
        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="messageFactory"> Factory constructing lazily the message to log if the level is enabled </param>
        public void Warn(Func<string> messageFactory)
        {
            if (IsWarnEnabled == false)
            {
                return;
            }
            Log(LogLevel.Warn, messageFactory());
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="message"> The message to log </param>
        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void WarnFormat(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Warn, exception, format, args);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Warn, formatProvider, format, args);
        }

        /// <summary>
        ///   Logs a warn message.
        /// </summary>
        /// <param name="exception"> The exception to log </param>
        /// <param name="formatProvider"> The format provider to use </param>
        /// <param name="format"> Format string for the message to log </param>
        /// <param name="args"> Format arguments for the message to log </param>
        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Warn, exception, formatProvider, format, args);
        }

        private void Log(LogLevel logLevel, string message)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, message));
        }

        private void Log(LogLevel logLevel, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, string message, Exception exception)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, message)
            {
                Exception = exception
            });
        }

        private void Log(LogLevel logLevel, Exception exception, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Exception = exception,
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, IFormatProvider formatProvider, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                FormatProvider = formatProvider,
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, Exception exceptoin, IFormatProvider formatProvider, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Exception = exceptoin,
                FormatProvider = formatProvider,
                Parameters = args
            });
        }
    }
}
