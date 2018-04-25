using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Extensions;
using Abp.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests.Domain.Entities
{
    public class TrackableExtendableObject_Tests
    {
        [Fact]
        public void Should_Throw_When_Value_Exists_And_Property_Is_Not_Virtual()
        {
            var entity = new MyEntity();
            entity.SetData("Name", "John");

            var exception = Should.Throw<InvalidOperationException>(
                () => { entity.GetTrackableData<string>("Name"); });
            exception.Message.ShouldMatch(
                @"Property Chars is not virtual\. Can't track classes with non-virtual properties\.");
        }

        [Fact]
        public void Should_Set_And_Get_Complex_POCOs()
        {
            // Arrange
            var entity = new MyEntity();
            var originalData = new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "B", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            };
            entity.SetData("ComplexData", originalData);

            // Act
            var gettedData = entity.GetTrackableData<MyComplexType>("ComplexData");

            // Assert
            /* Trackable data has been added some extra properties to track object state, so
             * when converted to json, it's not equal to to the origin. That means that you
             * can't use a trackable data as the parameter of SetData<T>
             */
            // gettedData.ToJsonString().ShouldBe(originalData.ToJsonString());
            gettedData.Name.ShouldBe(originalData.Name);
            gettedData.Age.ShouldBe(originalData.Age);
            gettedData.Inner.Count.ShouldBe(originalData.Inner.Count);
            for (var i = 0; i < gettedData.Inner.Count; i++)
            {
                var left = gettedData.Inner[i];
                var right = originalData.Inner[i];
                if (right == null)
                {
                    left.ShouldBe(null);
                }
                else
                {
                    left.Value1.ShouldBe(right.Value1);
                    left.Value2.ShouldBe(right.Value2);
                }
            }

            // Assert2
            entity.SetData("ComplexData", (MyComplexType)null);
            entity.GetTrackableData<MyComplexType>("ComplexData").ShouldBe(null);
        }

        [Fact]
        public void Should_Get_Null_If_Not_Present()
        {
            var entity = new MyEntity();
            entity.GetTrackableData<MyComplexType>("ComplexData").ShouldBe(null);
            entity.GetTrackableData<string>("NotExist").ShouldBe(null);
        }

        [Fact]
        public void Should_Get_New_Data_When_Tracked_Data_Changed()
        {
            // Arrange
            var entity = new MyEntity();
            entity.SetData("ComplexData", new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "B", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            });

            // Act
            var trackable = entity.GetTrackableData<MyComplexType>("ComplexData");
            trackable.Name = "Liu";
            trackable.Inner[1].Value1 = "C";

            // Assert
            var obj = entity.GetData<MyComplexType>("ComplexData");
            var target = new MyComplexType
            {
                Name = "Liu",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "C", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            };

            obj.ToJsonString().ShouldBe(target.ToJsonString());
        }

        [Fact]
        public void Should_Get_New_Data_When_Inner_Object_Been_Replaced()
        {
            // Arrange
            var entity = new MyEntity();
            entity.SetData("ComplexData", new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "B", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            });

            // Act
            var trackable = entity.GetTrackableData<MyComplexType>("ComplexData");
            trackable.Inner = new List<MyComplexTypeInner>
            {
                new MyComplexTypeInner {Value1 = "Z", Value2 = 1}
            };

            // Assert
            var obj = entity.GetData<MyComplexType>("ComplexData");
            var target = new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "Z", Value2 = 1}
                }
            };

            obj.ToJsonString().ShouldBe(target.ToJsonString());
        }

        [Fact]
        public void Should_Get_New_Data_When_Inner_Data_Been_Replaced_And_Changed()
        {
            // Arrange
            var entity = new MyEntity();
            entity.SetData("ComplexData", new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "A", Value2 = 2},
                    new MyComplexTypeInner {Value1 = "B", Value2 = null},
                    new MyComplexTypeInner {Value1 = null, Value2 = null},
                    null
                }
            });

            // Act
            var trackable = entity.GetTrackableData<MyComplexType>("ComplexData");
            trackable.Inner = new List<MyComplexTypeInner>
            {
                new MyComplexTypeInner {Value1 = "Z", Value2 = 1}
            };
            trackable.Inner.Add(new MyComplexTypeInner { Value1 = "ZZ", Value2 = 11 });

            // Assert
            var obj = entity.GetData<MyComplexType>("ComplexData");
            var target = new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    new MyComplexTypeInner {Value1 = "Z", Value2 = 1},
                    new MyComplexTypeInner {Value1 = "ZZ", Value2 = 11}
                }
            };

            obj.ToJsonString().ShouldBe(target.ToJsonString());
        }

        [Fact]
        public void Should_Get_New_Data_When_Inner_List_Changed()
        {
            // Arrange
            var entity = new MyEntity();
            entity.SetData("ComplexData", new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    null
                }
            });

            // Act
            var trackable = entity.GetTrackableData<MyComplexType>("ComplexData");
            trackable.Inner.Add(new MyComplexTypeInner { Value1 = "ZZ", Value2 = 11 });

            // Assert
            var obj = entity.GetData<MyComplexType>("ComplexData");
            var target = new MyComplexType
            {
                Name = "John",
                Age = 42,
                Inner = new List<MyComplexTypeInner>
                {
                    null,
                    new MyComplexTypeInner {Value1 = "ZZ", Value2 = 11}
                }
            };

            obj.ToJsonString().ShouldBe(target.ToJsonString());
        }

        [Fact]
        public void Should_Get_New_Data_When_Dictionary_Changed()
        {
            // Arrange
            var entity = new MyEntity();
            entity.SetData("Dict", new Dictionary<string, int>
            {
                ["A"] = 1,
                ["B"] = 2,
            });

            // Act
            var dict = entity.GetTrackableData<Dictionary<string, int>>("Dict");
            dict["C"] = 3;
            dict["A"] = 4;

            // Arrange
            var obj = entity.GetData<IDictionary<string, int>>("Dict");
            var target = new Dictionary<string, int>
            {
                ["A"] = 4,
                ["B"] = 2,
                ["C"] = 3,
            };
            obj.ToJsonString().ShouldBe(target.ToJsonString());
        }

        private class MyEntity : Entity, IExtendableObject
        {
            public string ExtensionData { get; set; }
        }

        public class MyComplexType
        {
            public virtual string Name { get; set; }
            public virtual byte Age { get; set; }
            public virtual List<MyComplexTypeInner> Inner { get; set; }
        }

        public class MyComplexTypeInner
        {
            public virtual string Value1 { get; set; }
            public virtual int? Value2 { get; set; }
        }
    }
}
