using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.Crm;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.EntityFramework
{
    public class ObjectMaterialize_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Hotel> _hotelRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ObjectMaterialize_Tests()
        {
            _companyRepository = Resolve<IRepository<Company>>();
            _hotelRepository = Resolve<IRepository<Hotel>>();

            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            // Note: The code below is cancelled because it is used only for DateTime_Kind_Property_Should_Be_Normalized_On_Ef_ObjectMaterialition test.
            // SetRandomClockProvider();
        }

        private static void SetRandomClockProvider()
        {
            if (RandomHelper.GetRandomOf(new[] {1, 2}) == 1)
            {
                Clock.Provider = ClockProviders.Local;
            }
            else
            {
                Clock.Provider = ClockProviders.Utc;
            }
        }

        // Note: The test below is cancelled since Effort does not work well with ObjectMaterialized event.
        // Even if we restore the state of an entity in ObjectMaterialized of EfUnitOfWork, Effort still thinks that this entity is changed.
        // [Fact]
        public void DateTime_Kind_Property_Should_Be_Normalized_On_Ef_ObjectMaterialition()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var companies = _companyRepository.GetAll().Include(c => c.Branches).ToList();

                foreach (var company in companies)
                {
                    company.CreationTime.Kind.ShouldBe(Clock.Kind);

                    company.BillingAddress.CreationTime.Kind.ShouldBe(Clock.Kind);
                    company.BillingAddress.LastModifier.ModificationTime.Value.Kind.ShouldBe(Clock.Kind);

                    company.ShippingAddress.CreationTime.Kind.ShouldBe(Clock.Kind);
                    company.ShippingAddress.LastModifier.ModificationTime.Value.Kind.ShouldBe(Clock.Kind);

                    foreach (var branch in company.Branches)
                    {
                        branch.CreationTime.Kind.ShouldBe(Clock.Kind);
                    }
                }

                uow.Complete();
            }
        }

        [Fact]
        public void DisableDateTimeNormalizationAttribute_Ef_ObjectMaterialition_Test()
        {
            var sampleCreationTime = new DateTime(2016, 03, 16, 0, 0, 0);
            UsingDbContext(context =>
            {
                context.Hotels.Add(new Hotel
                {
                    Name = "Grand Hotel",
                    CreationTime = sampleCreationTime,
                    BillingAddress = new Address
                    {
                        City = "New York",
                        Country = "United States of America",
                        FullAddress = "Herkimer St, Brooklyn, NY",
                        CreationTime = sampleCreationTime,
                        LastModifier = new Modifier
                        {
                            Name = "Neal",
                            ModificationTime = new DateTime(2016, 03, 16, 15, 0, 0)
                        }
                    },
                    Location = new Location
                    {
                        Lat = "41.9028° N",
                        Lng = "12.4964° E",
                        CreationTime = sampleCreationTime
                    },
                    Rooms = new List<Room>
                    {
                        new Room
                        {
                            Name = "Single",
                            Capacity = 1,
                            CreationTime = sampleCreationTime
                        },
                        new Room
                        {
                            Name = "Double",
                            Capacity = 2,
                            CreationTime = sampleCreationTime
                        }
                    }
                });

                context.SaveChanges();
            });

            using (var uow = _unitOfWorkManager.Begin())
            {
                var hotels = _hotelRepository.GetAll().Include(c => c.Rooms).ToList();

                foreach (var hotel in hotels)
                {
                    hotel.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);

                    hotel.BillingAddress.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);
                    hotel.BillingAddress.LastModifier.ModificationTime.Value.Kind.ShouldBe(DateTimeKind.Unspecified);

                    hotel.Location.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);

                    foreach (var room in hotel.Rooms)
                    {
                        room.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);
                    }
                }

                uow.Complete();
            }
        }
    }
}
