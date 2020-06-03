using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.DynamicEntityParameters;
using Abp.Localization;
using Abp.Modules;
using Abp.Threading;
using Abp.UI.Inputs;
using Abp.Zero.SampleApp.EntityHistory;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class DynamicEntityParametersTestBase : SampleAppTestBase<DynamicEntityParametersTestModule>
    {
        public const string TestPermission = "Abp.Zero.TestPermission";
        public string TestEntityFullName => typeof(Country).FullName;

        protected readonly IDynamicParameterStore DynamicParameterStore;
        protected readonly IEntityDynamicParameterStore EntityDynamicParameterStore;

        public DynamicEntityParametersTestBase()
        {
            DynamicParameterStore = Resolve<IDynamicParameterStore>();
            EntityDynamicParameterStore = Resolve<IEntityDynamicParameterStore>();

            AbpSession.UserId = 1;
            AbpSession.TenantId = 1;

            var user = AsyncHelper.RunSync(() => UserManager.FindByIdAsync(AbpSession.UserId.Value));
            AsyncHelper.RunSync(() => GrantPermissionAsync(user, TestPermission));
        }

        protected void RunAndCheckIfPermissionControlled(Action function, string requiredPermission = TestPermission)
        {
            var user = AsyncHelper.RunSync(() => UserManager.FindByIdAsync(AbpSession.UserId.Value));
            AsyncHelper.RunSync(() => ProhibitPermissionAsync(user, requiredPermission));

            bool isExceptionThrown = false;
            try
            {
                WithUnitOfWork(function.Invoke);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(requiredPermission) || e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
                {
                    isExceptionThrown = true;
                }
                else
                {
                    throw;
                }
            }

            if (!isExceptionThrown)
            {
                throw new Exception("Should throw exception for unauthorized users");
            }

            AsyncHelper.RunSync(() => GrantPermissionAsync(user, requiredPermission));

            WithUnitOfWork(function.Invoke);
        }

        protected async Task RunAndCheckIfPermissionControlledAsync(Func<Task> function, string requiredPermission = TestPermission)
        {
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.Value);

            await ProhibitPermissionAsync(user, requiredPermission);

            bool isExceptionThrown = false;
            try
            {
                await WithUnitOfWorkAsync(function.Invoke);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(requiredPermission) || e.InnerException != null && e.InnerException.Message.Contains(requiredPermission))
                {
                    isExceptionThrown = true;
                }
                else
                {
                    throw;
                }
            }

            if (!isExceptionThrown)
            {
                throw new Exception("Should throw exception for unauthorized users");
            }

            await GrantPermissionAsync(user, requiredPermission);

            await WithUnitOfWorkAsync(function.Invoke);
        }

        protected string GetRandomAllowedInputType()
        {
            return Resolve<IDynamicEntityParameterDefinitionManager>().GetAllAllowedInputTypeNames().First();
        }

        protected DynamicParameter CreateAndGetDynamicParameterWithTestPermission()
        {
            var rnd = new Random();

            string parameterNameRandomAppender = rnd.Next().ToString();
            Thread.Sleep(100);
            parameterNameRandomAppender += rnd.Next().ToString();

            var dynamicParameter = new DynamicParameter()
            {
                InputType = GetRandomAllowedInputType(),
                ParameterName = "City" + parameterNameRandomAppender,
                Permission = TestPermission,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                DynamicParameterStore.Add(dynamicParameter);
            });

            return dynamicParameter;
        }

        protected EntityDynamicParameter CreateAndGetEntityDynamicParameter()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                EntityDynamicParameterStore.Add(entityDynamicParameter);
            });
            return entityDynamicParameter;
        }

        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }
    }

    public class MyDynamicEntityParameterDefinitionProvider : DynamicEntityParameterDefinitionProvider
    {
        public override void SetDynamicEntityParameters(IDynamicEntityParameterDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();

            context.Manager.AddEntity<Country, int>();
        }
    }

    public class DynamicEntityParametersTestAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(DynamicEntityParametersTestBase.TestPermission,
                new FixedLocalizableString(DynamicEntityParametersTestBase.TestPermission));
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class DynamicEntityParametersTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DynamicEntityParametersTestModule).Assembly);

            Configuration.Authorization.Providers.Add<DynamicEntityParametersTestAuthorizationProvider>();
            Configuration.DynamicEntityParameters.Providers.Add<MyDynamicEntityParameterDefinitionProvider>();

        }
    }
}
