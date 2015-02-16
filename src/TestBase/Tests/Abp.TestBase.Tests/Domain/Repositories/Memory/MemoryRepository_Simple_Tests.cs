using Abp.Domain.Entities;
using Abp.TestBase.Domain.Repositories.Memory;
using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests.Domain.Repositories.Memory
{
    public class MemoryRepository_Simple_Tests
    {
        private readonly MemoryDatabase _database;
        private readonly MemoryRepository<MyEntity> _repository;

        public MemoryRepository_Simple_Tests()
        {
            _database = new MemoryDatabase();

            _repository = new MemoryRepository<MyEntity>(_database);

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
