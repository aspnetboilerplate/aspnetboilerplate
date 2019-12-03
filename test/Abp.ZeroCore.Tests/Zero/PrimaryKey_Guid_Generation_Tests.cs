using System;
using System.Collections.Generic;
using System.Text;
using Abp.ZeroCore.SampleApp.Core.BookStore;
using Shouldly;
using Xunit;

namespace Abp.Zero
{
    public class PrimaryKey_Guid_Generation_Tests : AbpZeroTestBase
    {
        [Fact]
        public void Guid_Id_ShouldBe_Generated_By_GuidGenerator_When_DatabaseGeneratedOption_None_Is_Used()
        {
            var guid = Guid.NewGuid();

            UsingDbContext(context =>
            {
                var testGuidGenerator = new TestGuidGenerator(guid);
                context.GuidGenerator = testGuidGenerator;

                var book = new Book
                {
                    Name = "Hitchhiker's Guide to the Galaxy"
                };

                context.Set<Book>().Add(book);
                context.SaveChanges();

                testGuidGenerator.CreateCalled.ShouldBeTrue();
                guid.ShouldBe(book.Id);
            });

        }

        [Fact]
        public void Guid_Id_ShouldBe_Generated_By_GuidGenerator_When_DatabaseGenerated_None_Attribute_Is_Used()
        {
            var guid = Guid.NewGuid();

            UsingDbContext(context =>
            {
                var testGuidGenerator = new TestGuidGenerator(guid);
                context.GuidGenerator = testGuidGenerator;

                var author = new Author
                {
                    Name = "Douglas Adams"
                };

                context.Set<Author>().Add(author);

                context.SaveChanges();

                testGuidGenerator.CreateCalled.ShouldBeTrue();
                guid.ShouldBe(author.Id);
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

                var store = new Store
                {
                    Name = "Tesk book store"
                };

                context.Set<Store>().Add(store);

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
