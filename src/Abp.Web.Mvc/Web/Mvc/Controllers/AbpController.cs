using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Reflection;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Castle.Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// Base class for all MVC Controllers in Abp system.
    /// </summary>
    public abstract class AbpController : Controller
    {
        /// <summary>
        /// Gets current session information.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Reference to the permission manager.
        /// </summary>
        public IPermissionManager PermissionManager { get; set; }

        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        public IPermissionChecker PermissionChecker { protected get; set; }

        /// <summary>
        /// Reference to the feature manager.
        /// </summary>
        public IFeatureManager FeatureManager { protected get; set; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        public IFeatureChecker FeatureChecker { protected get; set; }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { protected get; set; }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this application service.
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets localization source.
        /// It's valid if <see cref="LocalizationSourceName"/> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new AbpException("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }
        private ILocalizationSource _localizationSource;

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets current session information.
        /// </summary>
        [Obsolete("Use AbpSession property instead. CurrentSetting will be removed in future releases.")]
        protected IAbpSession CurrentSession { get { return AbpSession; } }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new AbpException("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }
        private IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork { get { return UnitOfWorkManager.Current; } }

        public IAuditingConfiguration AuditingConfiguration { get; set; }

        public IAuditInfoProvider AuditInfoProvider { get; set; }

        public IAuditingStore AuditingStore { get; set; }

        /// <summary>
        /// This object is used to measure an action execute duration.
        /// </summary>
        private Stopwatch _actionStopwatch;

        private AuditInfo _auditInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpController()
        {
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected Task<bool> IsGrantedAsync(string permissionName)
        {
            return PermissionChecker.IsGrantedAsync(permissionName);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected bool IsGranted(string permissionName)
        {
            return PermissionChecker.IsGranted(permissionName);
        }


        /// <summary>
        /// Checks if given feature is enabled for current tenant.
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns></returns>
        protected virtual Task<bool> IsEnabledAsync(string featureName)
        {
            return FeatureChecker.IsEnabledAsync(featureName);
        }

        /// <summary>
        /// Checks if given feature is enabled for current tenant.
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns></returns>
        protected virtual bool IsEnabled(string featureName)
        {
            return FeatureChecker.IsEnabled(featureName);
        }

        /// <summary>
        /// Json the specified data, contentType, contentEncoding and behavior.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="contentEncoding">Content encoding.</param>
        /// <param name="behavior">Behavior.</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (data == null)
            {
                data = new AjaxResponse();
            }
            else if (!ReflectionHelper.IsAssignableToGenericType(data.GetType(), typeof(AjaxResponse<>)))
            {
                data = new AjaxResponse(data);
            }

            return new AbpJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HandleAuditingBeforeAction(filterContext);

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            HandleAuditingAfterAction(filterContext);
        }

        #region Auditing

        private static MethodInfo GetMethodInfo(ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedActionDescriptor)
            {
                return ((ReflectedActionDescriptor)actionDescriptor).MethodInfo;
            }

            if (actionDescriptor is ReflectedAsyncActionDescriptor)
            {
                return ((ReflectedAsyncActionDescriptor)actionDescriptor).MethodInfo;
            }

            if (actionDescriptor is TaskAsyncActionDescriptor)
            {
                return ((TaskAsyncActionDescriptor)actionDescriptor).MethodInfo;
            }

            throw new AbpException("Could not get MethodInfo for the action '" + actionDescriptor.ActionName + "' of controller '" + actionDescriptor.ControllerDescriptor.ControllerName + "'.");
        }

        private void HandleAuditingBeforeAction(ActionExecutingContext filterContext)
        {
            if (!ShouldSaveAudit(filterContext))
            {
                _auditInfo = null;
                return;
            }

            var methodInfo = GetMethodInfo(filterContext.ActionDescriptor);

            _actionStopwatch = Stopwatch.StartNew();
            _auditInfo = new AuditInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ServiceName = methodInfo.DeclaringType != null
                                ? methodInfo.DeclaringType.FullName
                                : filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                MethodName = methodInfo.Name,
                Parameters = ConvertArgumentsToJson(filterContext.ActionParameters),
                ExecutionTime = Clock.Now
            };
        }

        private void HandleAuditingAfterAction(ActionExecutedContext filterContext)
        {
            if (_auditInfo == null || _actionStopwatch == null)
            {
                return;
            }

            _actionStopwatch.Stop();

            _auditInfo.ExecutionDuration = Convert.ToInt32(_actionStopwatch.Elapsed.TotalMilliseconds);
            _auditInfo.Exception = filterContext.Exception;

            if (AuditInfoProvider != null)
            {
                AuditInfoProvider.Fill(_auditInfo);
            }

            AuditingStore.Save(_auditInfo);
        }

        private bool ShouldSaveAudit(ActionExecutingContext filterContext)
        {
            if (AuditingConfiguration == null)
            {
                return false;
            }

            if (!AuditingConfiguration.MvcControllers.IsEnabled)
            {
                return false;
            }

            if (filterContext.IsChildAction && !AuditingConfiguration.MvcControllers.IsEnabledForChildActions)
            {
                return false;
            }

            return AuditingHelper.ShouldSaveAudit(
                GetMethodInfo(filterContext.ActionDescriptor),
                AuditingConfiguration,
                AbpSession,
                true
                );
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    dictionary[argument.Key] = argument.Value;
                }

                return JsonConvert.SerializeObject(
                    dictionary,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not serialize arguments for method: " + _auditInfo.ServiceName + "." + _auditInfo.MethodName);
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }

        #endregion
    }
}