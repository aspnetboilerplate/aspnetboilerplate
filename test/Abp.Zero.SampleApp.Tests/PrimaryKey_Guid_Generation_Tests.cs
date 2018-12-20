using System;
using Abp.Zero.SampleApp.BookStore;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests
{
    public class PrimaryKey_Guid_Generation_Tests : SampleAppTestBase
    {
        [Fact]
        public void Guid_Id_Should_Not_Be_Generated_By_GuidGenerator_When_DatabaseGeneratedOption_Identity_Is_Used()
        {
            var guid = Guid.NewGuid();

            UsingDbContext(context =>
            {
                var testGuidGenerator = new TestGuidGenerator(guid);
                context.GuidGenerator = testGuidGenerator;

                var book = context.Set<Book>().Add(new Book
                {
                    Name = "Hitchhiker's Guide to the Galaxy"
                });

                context.SaveChanges();

                testGuidGenerator.CreateCalled.ShouldBeFalse();
                guid.ShouldNotBe(book.Id);
            });

        }

        [Fact]
        public void Guid_Id_Should_Not_Be_Generated_By_GuidGenerator_When_DatabaseGenerated_Identity_Attribute_Is_Used()
        {
            var guid = Guid.NewGuid();

            UsingDbContext(context =>
            {
                var testGuidGenerator = new TestGuidGenerator(guid);
                context.GuidGenerator = testGuidGenerator;

                var author = context.Set<Author>().Add(new Author
                {
                    Name = "Douglas Adams"
                });

                context.SaveChanges();

                testGuidGenerator.CreateCalled.ShouldBeFalse();
                guid.ShouldNotBe(author.Id);
            });

        }

        [Fact]
        public void Guid_Id_Should_Be_Generated_By_GuidGenerator_When_Id_Field_Has_Different_Name()
        {
            var guid = Guid.NewGuid();

            UsingDbContext(context =>
            {
                var testGuidGenerator = new TestGuidGenerator(guid);
                context.GuidGenerator = testGuidGenerator;

                var store = context.Set<Store>().Add(new Store
                {
                    Name = "Tesk book store"
                });

                context.SaveChanges();

                testGuidGenerator.CreateCalled.ShouldBeTrue();
                guid.ShouldBe(store.Id);
            });

        }

        internal class TestGuidGenerator : IGuidGenerator
        {
            private readonly Guid _guid;

            public TestGuidGenerator(Guid guid)
            {
                _guid = guid;
            }

            public Guid Create()
            {
                CreateCalled = true;
                return _guid;
            }

            public bool CreateCalled { get; private set; }
        }
    }
}
