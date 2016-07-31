
using log4net;
using log4net.Core;
using log4net.Util;
using System;
using System.Globalization;

namespace Abp
{
    [Serializable]
    public class Log4NetLogger : MarshalByRefObject, Castle.Core.Logging.ILogger
    {
        private static readonly Type DeclaringType = typeof(Log4NetLogger);

        public Log4NetLogger(ILogger logger, Log4NetFactory factory)
        {
            this.Logger = logger;
            this.Factory = factory;
        }

        internal Log4NetLogger()
        {
        }

        internal Log4NetLogger(ILog log, Log4NetFactory factory)
            : this(log.Logger, factory)
        {
        }

        public bool IsDebugEnabled
        {
            get { return this.Logger.IsEnabledFor(Level.Debug); }
        }

        public bool IsErrorEnabled
        {
            get { return this.Logger.IsEnabledFor(Level.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return this.Logger.IsEnabledFor(Level.Fatal); }
        }

        public bool IsInfoEnabled
        {
            get { return this.Logger.IsEnabledFor(Level.Info); }
        }

        public bool IsWarnEnabled
        {
            get { return this.Logger.IsEnabledFor(Level.Warn); }
        }

        protected internal Log4NetFactory Factory { get; set; }

        protected internal ILogger Logger { get; set; }

        public override string ToString()
        {
            return this.Logger.ToString();
        }

        public virtual Castle.Core.Logging.ILogger CreateChildLogger(string name)
        {
            return this.Factory.Create(this.Logger.Name + "." + name);
        }

        public void Debug(string message)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, message, null);
            }
        }

        public void Debug(Func<string> messageFactory)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, messageFactory.Invoke(), null);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, message, exception);
            }
        }

        public void DebugFormat(string format, params Object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        public void DebugFormat(Exception exception, string format, params Object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, new SystemStringFormat(formatProvider, format, args), null);
            }
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Debug, new SystemStringFormat(formatProvider, format, args), exception);
            }
        }

        public void Error(string message)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, message, null);
            }
        }

        public void Error(Func<string> messageFactory)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, messageFactory.Invoke(), null);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, message, exception);
            }
        }

        public void ErrorFormat(string format, params Object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        public void ErrorFormat(Exception exception, string format, params Object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, new SystemStringFormat(formatProvider, format, args), null);
            }
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Error, new SystemStringFormat(formatProvider, format, args), exception);
            }
        }

        public void Fatal(string message)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, message, null);
            }
        }

        public void Fatal(Func<string> messageFactory)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, messageFactory.Invoke(), null);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params Object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        public void FatalFormat(Exception exception, string format, params Object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, new SystemStringFormat(formatProvider, format, args), null);
            }
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Fatal, new SystemStringFormat(formatProvider, format, args), exception);
            }
        }

        public void Info(string message)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, message, null);
            }
        }

        public void Info(Func<string> messageFactory)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, messageFactory.Invoke(), null);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, message, exception);
            }
        }

        public void InfoFormat(string format, params Object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        public void InfoFormat(Exception exception, string format, params Object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, new SystemStringFormat(formatProvider, format, args), null);
            }
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Info, new SystemStringFormat(formatProvider, format, args), exception);
            }
        }

        public void Warn(string message)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, message, null);
            }
        }

        public void Warn(Func<string> messageFactory)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, messageFactory.Invoke(), null);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, message, exception);
            }
        }

        public void WarnFormat(string format, params Object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        public void WarnFormat(Exception exception, string format, params Object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, new SystemStringFormat(formatProvider, format, args), null);
            }
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, Level.Warn, new SystemStringFormat(formatProvider, format, args), exception);
            }
        }
    }
}