using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Abp.TestBase.SampleApplication.ContacLists;
using Abp.TestBase.SampleApplication.Crm;
using Abp.TestBase.SampleApplication.EntityFramework;
using Abp.TestBase.SampleApplication.Messages;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.Shop;
using Castle.MicroKernel.Registration;
using EntityFramework.DynamicFilters;

namespace Abp.TestBase.SampleApplication.Tests
{
    public abstract class SampleApplicationTestBase : AbpIntegratedTestBase<SampleApplicationTestModule>
    {
        protected SampleApplicationTestBase()
        {
            CreateInitialData();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            //Fake DbConnection using Effort!
            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );
        }

        protected override void PostInitialize()
        {
            //Commented out, since Effort.DbConnection does not provide Sql Text while interception time.
            //DbInterception.Add(Resolve<WithNoLockInterceptor>());
        }

        protected virtual void CreateInitialData()
        {
            UsingDbContext(
                context =>
                {
                    context.ContactLists.Add(
                        new ContactList
                        {
                            TenantId = 1,
                            Name = "List of Tenant-1",
                            People = new List<Person>
                                     {
                                         new Person {Name = "halil", CreatorUserId = 42 },
                                         new Person {Name = "emre", IsDeleted = true}
                                     }
                        });

                    context.ContactLists.Add(
                        new ContactList
                        {
                            TenantId = 2,
                            Name = "List of Tenant-2",
                            People = new List<Person>
                                     {
                                         new Person {Name = "asimov"},
                                     }
                        });

                    context.ContactLists.Add(
                        new ContactList
                        {
                            TenantId = 3,
                            Name = "List-1 of Tenant-3",
                            People = new List<Person>
                                     {
                                         new Person {Name = "John Doe"},
                                     }
                        });

                    context.ContactLists.Add(
                        new ContactList
                        {
                            TenantId = 3,
                            Name = "List-2 of Tenant-3",
                            People = new List<Person>()
                        });
                });

            UsingDbContext(
                context =>
                {
                    context.Messages.Add(
                        new Message
                        {
                            TenantId = null,
                            Text = "host-message-1"
                        });

                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-1"
                        });

                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-2"
                        });
                });

            UsingDbContext(
              context =>
              {
                  AddCompany(context,
                      "Volosoft",
                      "Turkey",
                      "Istanbul",
                      "Denizkoskler Mah. Avcilar",
                      "Halil",
                      "Gumuspala Mah. Avcilar",
                      "Ismail",
                      "Headquarter",
                      "Europe Headquarter");

                  AddCompany(context,
                      "Microsoft",
                      "USA",
                      "New York",
                      "Herkimer St, Brooklyn, NY",
                      "Neal",
                      "Vinegar Hill, Brooklyn, NY",
                      "Peter",
                      "Main Office",
                      "IT Office");
              });

            UsingDbContext(
                context =>
                {
                    var product1 = new Product
                    {
                        Price = 10,
                        Stock = 1000
                    };

                    var product2 = new Product
                    {
                        Price = 99,
                        Stock = 1000
                    };

                    var product3 = new Product
                    {
                        Price = 15,
                        Stock = 500
                    };

                    context.Products.Add(product1);
                    context.Products.Add(product2);
                    context.Products.Add(product3);
                    context.SaveChanges();

                    //Product1 translations
                    var product1_en = new ProductTranslation { CoreId = product1.Id, Language = "en", Name = "Watch" };
                    var product1_tr = new ProductTranslation { CoreId = product1.Id, Language = "tr", Name = "Saat" };

                    context.ProductTranslations.Add(product1_en);
                    context.ProductTranslations.Add(product1_tr);

                    //Product2 translations
                    var product2_en = new ProductTranslation { CoreId = product2.Id, Language = "en", Name = "Bike" };
                    var product2_fr = new ProductTranslation { CoreId = product2.Id, Language = "fr", Name = "Bicyclette" };
                    
                    context.ProductTranslations.Add(product2_en);
                    context.ProductTranslations.Add(product2_fr);

                    //Product3 translations
                    var product3_it = new ProductTranslation { CoreId = product3.Id, Language = "it", Name = "Giornale" };

                    context.ProductTranslations.Add(product3_it);

                    context.SaveChanges();
                });
        }

        private void AddCompany(SampleApplicationDbContext context, string name, string country, string city, string address1, string modifier1, string address2, string modifier2, string branchName1, string branchName2)
        {
            context.Companies.Add(new Company
            {
                Name = name,
                CreationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Utc),
                BillingAddress = new Address
                {
                    Country = country,
                    City = city,
                    FullAddress = address1,
                    CreationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Local),
                    LastModifier = new Modifier
                    {
                        Name = modifier1,
                        ModificationTime = new DateTime(2016, 03, 16, 0, 0, 0, DateTimeKind.Local)
                    }
                },
                ShippingAddress = new Address
                {
                    Country = country,
                    City = city,
                    FullAddress = address2,
                    CreationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Utc),
                    LastModifier = new Modifier
                    {
                        Name = modifier2,
                        ModificationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Utc)
                    }
                },
                Branches = new List<Branch>
                      {
                          new Branch
                          {
                              Name = branchName1,
                              CreationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Local),
                          },
                          new Branch()
                          {
                              Name = branchName2,
                              CreationTime = new DateTime(2017, 03, 16, 0, 0, 0, DateTimeKind.Utc)
                          }
                      }
            });
        }

        public void UsingDbContext(Action<SampleApplicationDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<SampleApplicationDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                context.SaveChanges();
            }
        }

        public T UsingDbContext<T>(Func<SampleApplicationDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<SampleApplicationDbContext>())
            {
                context.DisableAllFilters();
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        public async Task<T> UsingDbContextAsync<T>(Func<SampleApplicationDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<SampleApplicationDbContext>())
            {
                context.DisableAllFilters();
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}