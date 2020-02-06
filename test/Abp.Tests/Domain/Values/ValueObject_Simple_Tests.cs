using System;
using Xunit;

namespace Abp.Tests.Domain.Values
{
    public class ValueObject_Simple_Tests
    {
        [Fact]
        public void Value_Objects_Should_Be_Same_If_Contains_Same_Data()
        {
            var address1 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
            var address2 = new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);

            Assert.True(address1.ValueEquals(address2));
            Assert.True(address2.ValueEquals(address1));
        }

        [Fact]
        public void Value_Objects_Should_Not_Be_Same_If_Contains_Different_Data()
        {
            Assert.False(
                new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42).ValueEquals(
                    new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5A0"), "Baris Manco Street", 42))
            );

            Assert.False(
                new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Streettt", 42).ValueEquals(
                new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42))
            );

            Assert.False(
                new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42).ValueEquals(
                    new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 45))
            );
        }

        [Fact]
        public void Value_Objects_Should_Not_Be_Same_If_One_Of_Them_Is_Null()
        {
            Assert.False(new Address(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42).ValueEquals(null));
        }

        [Fact]
        public void Value_Object_Nullable_Guid_Property_Test()
        {
            var anAddress = new Address2(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
            var anotherAddress = new Address2(null, "Another street", 42);

            Assert.False(anAddress.ValueEquals(anotherAddress));
            Assert.False(anotherAddress.ValueEquals(anAddress));
        }

        [Fact]
        public void Value_Object_Should_Be_Same_If_Ignored_Property_Diff()
        {
            var address1 = new Address3(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
            var address2 = new Address3(null, "Baris Manco Street", 42);

            Assert.True(address1.ValueEquals(address2));
            Assert.True(address2.ValueEquals(address1));
        }

        [Fact]
        public void Value_Object_Should__Not_Be_Same_If_NotIgnored_Property_Diff()
        {
            var address1 = new Address3(new Guid("21C67A65-ED5A-4512-AA29-66308FAAB5AF"), "Baris Manco Street", 42);
            var address2 = new Address3(null, "Another street", 42);

            Assert.False(address1.ValueEquals(address2));
            Assert.False(address2.ValueEquals(address1));
        }
    }
}
