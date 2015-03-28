using System;

namespace Abp
{
    /// <summary>
    /// This class can be used to provide an action when
    /// Dipose method is called.
    /// </summary>
    public class DisposeAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// Creates a new <see cref="DisposeAction"/> object.
        /// </summary>
        /// <param name="action">Action to be executed when this object is disposed.</param>
        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            if (_action != null)
            {
                _action();
            }
        }
    }
}
