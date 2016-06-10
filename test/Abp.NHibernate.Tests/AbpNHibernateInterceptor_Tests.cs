using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.NHibernate.Tests.Entities;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class AbpNHibernateInterceptor_Tests : NHibernateTestBase
    {
        private readonly IRepository<Hotel> _hotelRepository;

        public AbpNHibernateInterceptor_Tests()
        {
            _hotelRepository = Resolve<IRepository<Hotel>>();
            UsingSession(session =>
            {
                session.Save(new Hotel
                {
                    Name = "Villa Borghese",
                    CreationDate = new DateTime(2016, 04, 27, 14, 0, 0, DateTimeKind.Utc),
                    ModificationDate = new DateTime(2017, 04, 27, 14, 0, 0, DateTimeKind.Local),
                    Headquarter = new Location
                    {
                        Name = "IT Office",
                        CreationDate = new DateTime(2016, 05, 27, 14, 0, 0, DateTimeKind.Local),
                        ModificationDate = new DateTime(2017, 05, 27, 14, 0, 0, DateTimeKind.Local)
                    }
                });

            });
        }

        [Fact]
        public void Intercepter_Should_Normalize_DateTime_Kind_Properties()
        {
            var hotel = _hotelRepository.GetAllList().First();
            hotel.CreationDate.Kind.ShouldBe(Clock.Kind);
            hotel.ModificationDate.Value.Kind.ShouldBe(Clock.Kind);
            hotel.Headquarter.CreationDate.Kind.ShouldBe(Clock.Kind);
            hotel.Headquarter.ModificationDate.Value.Kind.ShouldBe(Clock.Kind);
        }
    }
}
