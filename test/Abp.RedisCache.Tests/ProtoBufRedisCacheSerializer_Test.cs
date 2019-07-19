using Abp.Runtime.Caching.Redis;
using Abp.Tests;
using ProtoBuf;
using Shouldly;
using Xunit;

namespace Abp.RedisCache.Tests
{
    [ProtoContract]
    public class ClassToSerialize
    {
        [ProtoMember(2)]
        public int Age { get; set; }

        [ProtoMember(1)]
        public string Name { get; set; }
    }

    public class ProtoBufRedisCacheSerializer_Test : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Simple_Serialize_Deserialize_Test()
        {
            //Arrange
            var protoBufSerializer = new ProtoBufRedisCacheSerializer();
            var objectToSerialize = new ClassToSerialize {Age = 10, Name = "John"};

            //Act
            string classSerializedString = protoBufSerializer.Serialize(
                objectToSerialize
            );

            var classUnSerialized = protoBufSerializer.Deserialize<ClassToSerialize>(classSerializedString);

            //Assert
            classUnSerialized.Age.ShouldBe(10);
            classUnSerialized.Name.ShouldBe("John");
        }
    }
}