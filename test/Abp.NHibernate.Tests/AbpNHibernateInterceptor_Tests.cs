using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.NHibernate.Tests.Entities;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class AbpNHibernateInterceptor_Tests : NHibernateTestBase
    {
        private readonly IRepository<Hotel> _hotelRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AbpNHibernateInterceptor_Tests()
        {
            _hotelRepository = Resolve<IRepository<Hotel>>();
            _orderRepository = Resolve<IRepository<Order>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

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

            UsingSession(session =>
            {
                var sampleCreationTime = new DateTime(2016, 05, 27, 14, 0, 0);
                var order = new Order
                {
                    CreationTime = sampleCreationTime,
                    TotalPrice = 100
                };

                session.Save(order);

                var orderDetail1 = new OrderDetail
                {
                    Order = order,
                    ItemName = "Watch",
                    Count = 1,
                    Price = 40,
                    TotalPrice = 40,
                    CreationTime = sampleCreationTime
                };

                session.Save(orderDetail1);

                var orderDetail2 = new OrderDetail
                {
                    Order = order,
                    ItemName = "Parfume",
                    Count = 2,
                    Price = 30,
                    TotalPrice = 60,
                    CreationTime = sampleCreationTime
                };

                session.Save(orderDetail2);
            });

            Clock.Provider = ClockProviders.Utc;
        }

        [Fact]
        public void Normalize_DateTime_Kind_Properties_Test()
        {
            var hotel = _hotelRepository.GetAllList().First();
            hotel.CreationDate.Kind.ShouldBe(Clock.Kind);
            hotel.ModificationDate.Value.Kind.ShouldBe(Clock.Kind);
            hotel.Headquarter.CreationDate.Kind.ShouldBe(Clock.Kind);
            hotel.Headquarter.ModificationDate.Value.Kind.ShouldBe(Clock.Kind);
        }

        [Fact]
        public void Dont_Normalize_DateTime_Kind_Properties_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var order = _orderRepository.GetAllIncluding(o => o.Items).First();
                order.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);

                foreach (var orderDetail in order.Items)
                {
                    orderDetail.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);
                }

                uow.Complete();
            }
        }
    }
}
