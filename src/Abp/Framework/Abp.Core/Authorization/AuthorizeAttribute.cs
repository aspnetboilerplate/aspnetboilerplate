using Abp.Application;
using System;
using Abp.Utils.Extensions;
using System.Linq;
using System.Threading;

namespace Abp.Authorization
{
    public class AbpCoreAuthorizeAttribute : Attribute
    {
        public string[] Features { get; set; }

        public string[] Roles { get; set; }

        public string[] Users { get; set; }

        public AbpCoreAuthorizeAttribute()
        {
            Features = new string[0];
            Roles = new string[0];
            Users = new string[0];
        }

        public virtual bool Authorize()
        {
            if (!IsCurrentEditionDefinedOneOfTheFeatures())
            {
                return false;
            }

            if (!IsCurrentUserIsOneOfTheUsers())
            {
                return false;
            }

            if (!IsCurrentUserInOneOfTheRoles())
            {
                return false;
            }

            return false;
        }

        protected virtual bool IsCurrentEditionDefinedOneOfTheFeatures()
        {
            if (Edition.Current == null)
            {
                //TODO: Throw exception?
                return false;
            }

            if (Features.IsNullOrEmpty())
            {
                return true;
            }

            foreach (var feature in Features)
            {
                if (Edition.Current.HasFeature(feature))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsCurrentUserIsOneOfTheUsers()
        {
            if (Thread.CurrentPrincipal == null)
            {
                //TODO: Throw exception?
                return false;
            }

            if (Users.IsNullOrEmpty())
            {
                return true;
            }

            if (Users.Contains(Thread.CurrentPrincipal.Identity.Name))
            {
                return true;
            }

            return false;
        }

        protected virtual bool IsCurrentUserInOneOfTheRoles()
        {
            if (Thread.CurrentPrincipal == null)
            {
                //TODO: Throw exception?
                return false;
            }

            if (Roles.IsNullOrEmpty())
            {
                return true;
            }

            if (Roles.Any(role => Thread.CurrentPrincipal.IsInRole(role))) //TODO: How to check role!
            {
                return true;
            }

            return false;
        }
    }
}
