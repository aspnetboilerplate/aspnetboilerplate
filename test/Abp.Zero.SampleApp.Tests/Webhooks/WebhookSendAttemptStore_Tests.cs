using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO.Ports;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Abp.Linq;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Linq;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookSendAttemptStore_Tests : WebhookTestBase
    {
        private IWebhookSendAttemptStore _webhookSendAttemptStore;
        private IWebhookEventStore _webhookEventStore;
        public WebhookSendAttemptStore_Tests()
        {
#if DEBUG
            LocalIocManager.IocContainer.Register(
                Component.For<IAsyncQueryableExecuter>()
                    .ImplementedBy<FakeAsyncQueryableExecuter>()
                    .LifestyleSingleton()
                    .IsDefault()
                );
#endif

            _webhookSendAttemptStore = Resolve<IWebhookSendAttemptStore>();
            _webhookEventStore = Resolve<IWebhookEventStore>();
        }

        private Guid CreateAndGetIdWebhookEvent()
        {
            return _webhookEventStore.InsertAndGetId(new WebhookEvent()
            {
                Data = "test",
                WebhookName = "Test"
            });
        }

        private WebhookSendAttempt CreateAndGetWebhookSendAttempt()
        {
            var sendAttempt = new WebhookSendAttempt()
            {
                WebhookEventId = CreateAndGetIdWebhookEvent(),
                WebhookSubscriptionId = new Guid()
            };

            _webhookSendAttemptStore.Insert(sendAttempt);
            return sendAttempt;
        }

        #region  Async
        public static IEnumerable<object[]> InsertTestData =>
            new List<object[]>
            {
                new object[]
                {
                    new WebhookSendAttempt(),
                    typeof(DbUpdateException)
                },

                new object[]
                {
                    new WebhookSendAttempt()
                    {
                        WebhookEventId = new Guid(),
                        WebhookSubscriptionId = Guid.Empty
                    },
                    typeof(DbUpdateException)
                },

                new object[]
                {
                    new WebhookSendAttempt()
                    {
                        WebhookEventId = Guid.Empty,
                        WebhookSubscriptionId = Guid.NewGuid()
                    },
                    typeof(DbUpdateException)
                },

                new object[]
                {
                    new WebhookSendAttempt()
                    {
                        WebhookEventId = new Guid(), //there is no item with that event id
                        WebhookSubscriptionId = new Guid()
                    },
                    typeof(DbUpdateException)
                }
            };

        [Theory]
        [MemberData(nameof(InsertTestData))]
        public async Task Tests_Insert_Async(WebhookSendAttempt webhookSendAttempt, Type typeOfException = null)
        {
            if (typeOfException != null)
            {
                await Should.ThrowAsync(async () =>
                 {
                     await _webhookSendAttemptStore.InsertAsync(webhookSendAttempt);
                 }, typeOfException);
            }
            else
            {
                await _webhookSendAttemptStore.InsertAsync(webhookSendAttempt);
            }
        }

        [Fact]
        public async Task Should_Insert_Async()
        {
            await _webhookSendAttemptStore.InsertAsync(new WebhookSendAttempt()
            {
                WebhookEventId = CreateAndGetIdWebhookEvent(),
                WebhookSubscriptionId = Guid.NewGuid()
            });
        }

        public static IEnumerable<object[]> UpdateTestData =>
            new List<object[]>
            {
                new object[] {Guid.Empty, Guid.Empty, typeof(DbUpdateException)},
                new object[] {Guid.Empty, Guid.NewGuid(), typeof(DbUpdateException)},
                new object[] {Guid.NewGuid(), Guid.Empty, typeof(DbUpdateException)},
                new object[] {Guid.NewGuid(),Guid.NewGuid(), typeof(DbUpdateException)},//webhookevent not exists
                new object[] {}
            };

        [Theory]
        [MemberData(nameof(UpdateTestData))]
        public async Task Tests_Update_Async(Guid? webhookEventId = null, Guid? webhookSubscriptionId = null, Type typeOfException = null)
        {
            var sendAttempt = CreateAndGetWebhookSendAttempt();
            sendAttempt.Response = "Test";

            if (webhookEventId.HasValue)
            {
                sendAttempt.WebhookEventId = webhookEventId.Value;
            }

            if (webhookSubscriptionId.HasValue)
            {
                sendAttempt.WebhookSubscriptionId = webhookSubscriptionId.Value;
            }

            if (typeOfException != null)
            {
                await Should.ThrowAsync(async () =>
                {
                    await _webhookSendAttemptStore.UpdateAsync(sendAttempt);

                }, typeOfException);
            }
            else
            {
                await _webhookSendAttemptStore.UpdateAsync(sendAttempt);
                (await _webhookSendAttemptStore.GetAsync(sendAttempt.TenantId, sendAttempt.Id)).Response.ShouldBe("Test");
            }
        }

        [Fact]
        public async Task Should_Get_Attempt_Count_Async()
        {
            var webhookEventId = CreateAndGetIdWebhookEvent();
            var sendAttempt = new WebhookSendAttempt()
            {
                WebhookEventId = webhookEventId,
                WebhookSubscriptionId = Guid.NewGuid()
            };

            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            (await _webhookSendAttemptStore.GetSendAttemptCountAsync(sendAttempt.TenantId, sendAttempt.WebhookEventId,
                    sendAttempt.WebhookSubscriptionId)).ShouldBe(3);
        }

        [Fact]
        public async Task Should_Get_Has_Any_Successful_Attempt_In_Last_X_Record_Async()
        {
            (await _webhookSendAttemptStore.HasXConsecutiveFailAsync(null,//if there is no record should return true
                Guid.NewGuid(), 2)).ShouldBe(false);

            var webhookEventId = CreateAndGetIdWebhookEvent();
            var sendAttempt = new WebhookSendAttempt()
            {
                WebhookEventId = webhookEventId,
                WebhookSubscriptionId = Guid.NewGuid(),
                ResponseStatusCode = HttpStatusCode.OK
            };

            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            sendAttempt.ResponseStatusCode = HttpStatusCode.Forbidden;

            sendAttempt.Id = Guid.Empty;
            sendAttempt.CreationTime = default;
            Thread.Sleep(1000);
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            sendAttempt.CreationTime = default;
            Thread.Sleep(1000);
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            (await _webhookSendAttemptStore.HasXConsecutiveFailAsync(sendAttempt.TenantId,
                sendAttempt.WebhookSubscriptionId, 2)).ShouldBe(true);

            (await _webhookSendAttemptStore.HasXConsecutiveFailAsync(sendAttempt.TenantId,
                sendAttempt.WebhookSubscriptionId, 3)).ShouldBe(false);
        }

        [Fact]
        public async Task Should_Get_All_Send_Attempts_By_Webhook_Event_Id_Async()
        {
            (await _webhookSendAttemptStore.HasXConsecutiveFailAsync(null,//if there is no record should return true
                Guid.NewGuid(), 2)).ShouldBe(false);

            var webhookEventId = CreateAndGetIdWebhookEvent();
            var sendAttempt = new WebhookSendAttempt()
            {
                TenantId = 1,
                WebhookEventId = webhookEventId,
                WebhookSubscriptionId = Guid.NewGuid(),
                ResponseStatusCode = HttpStatusCode.OK
            };

            (await _webhookSendAttemptStore.GetAllSendAttemptsByWebhookEventIdAsync(sendAttempt.TenantId,
                sendAttempt.WebhookEventId)).Count.ShouldBe(0);

            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            (await _webhookSendAttemptStore.GetAllSendAttemptsByWebhookEventIdAsync(sendAttempt.TenantId,
                sendAttempt.WebhookEventId)).Count.ShouldBe(1);

            sendAttempt.Id = Guid.Empty;
            sendAttempt.CreationTime = default;
            Thread.Sleep(1000);
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            sendAttempt.CreationTime = default;
            Thread.Sleep(1000);
            await _webhookSendAttemptStore.InsertAsync(sendAttempt);

            (await _webhookSendAttemptStore.GetAllSendAttemptsByWebhookEventIdAsync(sendAttempt.TenantId,
                sendAttempt.WebhookEventId)).Count.ShouldBe(3);
        }
        #endregion Async


        #region Sync

        [Theory]
        [MemberData(nameof(InsertTestData))]
        public void Tests_Insert_Sync(WebhookSendAttempt webhookSendAttempt, Type typeOfException = null)
        {
            if (typeOfException != null)
            {
                Should.Throw(() =>
               {
                   _webhookSendAttemptStore.Insert(webhookSendAttempt);
               }, typeOfException);
            }
            else
            {
                _webhookSendAttemptStore.Insert(webhookSendAttempt);
            }
        }

        [Fact]
        public void Should_Insert_Sync()
        {
            _webhookSendAttemptStore.Insert(new WebhookSendAttempt()
            {
                WebhookEventId = CreateAndGetIdWebhookEvent(),
                WebhookSubscriptionId = Guid.NewGuid()
            });
        }

        [Theory]
        [MemberData(nameof(UpdateTestData))]
        public void Tests_Update_Sync(Guid? webhookEventId = null, Guid? webhookSubscriptionId = null, Type typeOfException = null)
        {
            var sendAttempt = CreateAndGetWebhookSendAttempt();
            sendAttempt.Response = "Test";

            if (webhookEventId.HasValue)
            {
                sendAttempt.WebhookEventId = webhookEventId.Value;
            }

            if (webhookSubscriptionId.HasValue)
            {
                sendAttempt.WebhookSubscriptionId = webhookSubscriptionId.Value;
            }

            if (typeOfException != null)
            {
                Should.Throw(() =>
              {
                  _webhookSendAttemptStore.Update(sendAttempt);

              }, typeOfException);
            }
            else
            {
                _webhookSendAttemptStore.Update(sendAttempt);
                _webhookSendAttemptStore.Get(sendAttempt.TenantId, sendAttempt.Id).Response.ShouldBe("Test");
            }
        }

        [Fact]
        public void Should_Get_Attempt_Count_Sync()
        {
            var webhookEventId = CreateAndGetIdWebhookEvent();
            var sendAttempt = new WebhookSendAttempt()
            {
                WebhookEventId = webhookEventId,
                WebhookSubscriptionId = Guid.NewGuid()
            };

            _webhookSendAttemptStore.Insert(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            _webhookSendAttemptStore.Insert(sendAttempt);

            sendAttempt.Id = Guid.Empty;
            _webhookSendAttemptStore.Insert(sendAttempt);

            _webhookSendAttemptStore.GetSendAttemptCount(sendAttempt.TenantId, sendAttempt.WebhookEventId,
                sendAttempt.WebhookSubscriptionId).ShouldBe(3);
        }

        #endregion

    }
}
