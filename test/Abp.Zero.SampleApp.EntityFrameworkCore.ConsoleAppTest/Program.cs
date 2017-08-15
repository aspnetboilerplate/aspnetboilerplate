using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;
using Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas;
using Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas;
using Abp.Zero.SampleApp.MultiTenancy;
using Castle.Facilities.Logging;
using Castle.LoggingFacility.MsLogging;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Abp.Zero.SampleApp.EntityFrameworkCore.ConsoleAppTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var abpBootstrapper = AbpBootstrapper.Create<EfCoreTestConsoleAppModule>())
            {
                abpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );

                abpBootstrapper.Initialize();
                abpBootstrapper.IocManager.Using<MigratorRunner>(migrateTester => migrateTester.Run());
            }

            Console.WriteLine("Press Enter to EXIT!");
            Console.ReadLine();
        }
    }

    [DependsOn(typeof(SampleAppEntityFrameworkCoreModule))]
    public class EfCoreTestConsoleAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Server=localhost; Database=AbpZeroMigrateTest; Trusted_Connection=True;";

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEntityFrameworkSqlServer();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var castleLoggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (castleLoggerFactory != null)
            {
                serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                    .AddCastleLogger(castleLoggerFactory);
            }

            Configuration.Modules.AbpEfCore().AddDbContext<AppDbContext>(configuration =>
            {
                configuration.DbContextOptions
                    .UseInternalServiceProvider(serviceProvider)
                    .UseSqlServer(configuration.ConnectionString);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

    public class MigratorRunner : ITransientDependency
    {
        private readonly AppTestMigrator _appTestMigrator;

        public MigratorRunner(AppTestMigrator appTestMigrator)
        {
            _appTestMigrator = appTestMigrator;
        }

        public void Run()
        {
            List<Tenant> tenants = null;

            _appTestMigrator.CreateOrMigrateForHost(context =>
            {
                new HostDataBuilder(context).Build();
                tenants = context.Tenants.ToList();
            });

            foreach (var tenant in tenants)
            {
                _appTestMigrator.CreateOrMigrateForTenant(tenant, context =>
                {
                    new TenantDataBuilder(context).Build(tenant.Id);
                });
            }
        }
    }

    public class AppTestMigrator : AbpZeroDbMigrator<AppDbContext>
    {
        public AppTestMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
            : base(unitOfWorkManager,
                  connectionStringResolver,
                  dbContextResolver)
        {

        }
    }
}
