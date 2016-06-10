using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.MemoryDb.Repositories;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.MemoryDb.Tests.Repositories
{
    public class MemoryRepository_Simple_Tests
    {
        private readonly MemoryDatabase _database;
        private readonly IRepository<MyEntity> _repository;

        public MemoryRepository_Simple_Tests()
        {
            _database = new MemoryDatabase();

            var databaseProvider = Substitute.For<IMemoryDatabaseProvider>();
            databaseProvider.Database.Returns(_database);

            _repository = new MemoryRepository<MyEntity>(databaseProvider);

            //Testing Insert by creating initial data
            _repository.Insert(new MyEntity("test-1"));
            _repository.Insert(new MyEntity("test-2"));
            _database.Set<MyEntity>().Count.ShouldBe(2);
        }

        [Fact]
        public void Count_Test()
        {
            _repository.Count().ShouldBe(2);
        }

        [Fact]
        public void Delete_Test()
        {
            var test1 = _repository.FirstOrDefault(e => e.Name == "test-1");
            test1.ShouldNotBe(null);

            _repository.Delete(test1);
            
            test1 = _repository.FirstOrDefault(e => e.Name == "test-1");
            test1.ShouldBe(null);
        }

        public class MyEntity : Entity
        {
            public string Name { get; set; }

            public MyEntity()
            {
                
            }

            public MyEntity(string name)
            {
                Name = name;
            }
        }
    }
}
