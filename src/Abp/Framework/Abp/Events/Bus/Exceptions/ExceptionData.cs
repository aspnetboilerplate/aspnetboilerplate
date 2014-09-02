using System;

namespace Abp.Events.Bus.Exceptions
{
    /// <summary>
    /// This type of events can be used to notify for an exception.
    /// </summary>
    public class ExceptionData : EventData
    {
        /// <summary>
        /// Exception object.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public ExceptionData(Exception exception)
        {
            Exception = exception;
        }
    }
}
