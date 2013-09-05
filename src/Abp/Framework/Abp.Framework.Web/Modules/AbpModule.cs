using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Modules
{
    public abstract class AbpModule
    {
        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done before dependency registers.
        /// </summary>
        /// <param name="initializationContext"> </param>
        public virtual void PreInitialize(AbpInitializationContext initializationContext)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Register dependency installers and components.
        /// </summary>
        /// <param name="initializationContext"> </param>
        public virtual void Initialize(AbpInitializationContext initializationContext)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done after dependency registers.
        /// </summary>
        /// <param name="initializationContext"> </param>
        public virtual void PostInitialize(AbpInitializationContext initializationContext)
        {
            
        }
    }
}
