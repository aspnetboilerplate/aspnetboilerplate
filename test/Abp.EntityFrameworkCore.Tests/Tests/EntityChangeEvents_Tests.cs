using System;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class EntityChangeEvents_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IEventBus _eventBus;

        public EntityChangeEvents_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            _eventBus = Resolve<IEventBus>();
        }

        [Fact]
        public void Complex_Event_Test()
        {
            var blogName = Guid.NewGuid().ToString("N");

            var creatingEventTriggered = false;
            var createdEventTriggered = false;
            var updatingEventTriggered = false;
            var updatedEventTriggered = false;
            var blogUrlChangedEventTriggered = false;

            _eventBus.Register<EntityCreatingEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeFalse();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                creatingEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);

                /* Want to change url from http:// to https:// (ensure to save https url always)
                 * Expect to trigger EntityUpdatingEventData, EntityUpdatedEventData and BlogUrlChangedEventData events */
                data.Entity.Url.ShouldStartWith("http://");
                data.Entity.ChangeUrl(data.Entity.Url.Replace("http://", "https://"));
            });

            _eventBus.Register<EntityCreatedEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeTrue();
                updatingEventTriggered.ShouldBeTrue();
                blogUrlChangedEventTriggered.ShouldBeTrue();

                createdEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
            });

            _eventBus.Register<EntityUpdatingEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeFalse();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                updatingEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
                data.Entity.Url.ShouldStartWith("https://");
            });

            _eventBus.Register<EntityUpdatedEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                //createdEventTriggered.ShouldBeTrue();
                updatingEventTriggered.ShouldBeTrue();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeTrue();

                updatedEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
                data.Entity.Url.ShouldStartWith("https://");
            });

            _eventBus.Register<BlogUrlChangedEventData>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeTrue();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                blogUrlChangedEventTriggered = true;

                data.Blog.IsTransient().ShouldNotBeNull();
                data.Blog.Name.ShouldBe(blogName);
                data.Blog.Url.ShouldStartWith("https://");
            });

            _blogRepository.Insert(new Blog(blogName, "http://aspnetboilerplate.com"));
        }
    }
}