using NUnit.Framework;

namespace Abp.Tests.Entities
{
    [TestFixture]
    public class Test_Entities
    {
        [Test]
        public void Equality_Operator_Works()
        {
            var w1 = new Worker { Id = 5, Name = "Halil ibrahim Kalkan" };
            var w2 = new Worker { Id = 5, Name = "Halil ibrahim Kalkan" };

            Assert.IsTrue(w1 == w2, "Same class with same Id must be equal");
            Assert.IsTrue(w1.Equals(w2), "Same class with same Id must be equal");
            
            Worker w3 = null;
            Worker w4 = null;

            Assert.IsTrue(w3 == w4, "Null objects in same class must be equal");

            var m1 = new Manager { Id = 5, Name = "Halil ibrahim Kalkan", Title = "Software Architect" };

            Assert.IsTrue(m1 == w1, "Derived classes must be equal if their Ids are equal");
            
            var d1 = new Department {Id = 5, Name = "IVR"};

            Assert.IsFalse(m1 == d1, "Different classes must not be considered as equal even if their Ids are equal!");

            var w5 = w1;
            w5.Id = 6;

            Assert.IsTrue(w5 == w1, "Same object instance must be equal.");
        }

        [Test]
        public void IsTransient_Works()
        {
            var w1 = new Worker { Name = "Halil ibrahim Kalkan" };
            var w2 = new Worker { Id = 5, Name = "Halil ibrahim Kalkan" };

            Assert.IsTrue(w1.IsTransient());
            Assert.IsFalse(w2.IsTransient());
        }
    }
}
