using System;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;

namespace Abp.Events.Bus.Factories
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory"/> implementation is used to get/release
    /// handlers using Ioc.
    /// </summary>
    public class IocHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// Type of the handler.
        /// </summary>
        public Type HandlerType { get; private set; }

        private readonly IocManager _iocManager;

        /// <summary>
        /// Creates a new instance of <see cref="IocHandlerFactory"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler</param>
        public IocHandlerFactory(Type handlerType)
        {
            _iocManager = IocManager.Instance;
            HandlerType = handlerType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IocHandlerFactory"/> class.
        /// </summary>
        /// <param name="iocManager"></param>
        /// <param name="handlerType">Type of the handler</param>
        public IocHandlerFactory(IocManager iocManager, Type handlerType)
        {
            _iocManager = iocManager;
            HandlerType = handlerType;
        }

        /// <summary>
        /// Resolves handler object from Ioc container.
        /// </summary>
        /// <returns>Resolved handler object</returns>
        public IEventHandler GetHandler()
        {
            return (IEventHandler)_iocManager.Resolve(HandlerType);
        }

        /// <summary>
        /// Releases handler object using Ioc container.
        /// </summary>
        /// <param name="handler">Handler to be released</param>
        public void ReleaseHandler(IEventHandler handler)
        {
            _iocManager.Release(handler);
        }
    }
}