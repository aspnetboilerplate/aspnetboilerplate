using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using Xunit;

namespace Abp.Tests.Experimental
{
    public class GenericMethodCallTests
    {
        public void Method_BaseEvent_BaseArg(EntityEventData<Person> data)
        {
        }

        public void Method_BaseEvent_DerivedArg(EntityEventData<Student> data)
        {
        }

        public void Method_DerivedEvent_BaseArg(EntityUpdatedEventData<Person> data)
        {
        }

        public void Method_DerivedEvent_DerivedArg(EntityUpdatedEventData<Student> data)
        {
        }

        public class Person : Entity
        {
        }

        public class Student : Person
        {
        }

        [Fact]
        public void Test_Method_BaseEvent_BaseArg()
        {
            Method_BaseEvent_BaseArg(new EntityEventData<Person>(new Person())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityEventData<Person>(new Student())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityUpdatedEventData<Person>(new Person())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityUpdatedEventData<Person>(new Student())); //TODO: <Student>
        }

        [Fact]
        public void Test_Method_BaseEvent_DerivedArg()
        {
            Method_BaseEvent_DerivedArg(new EntityEventData<Student>(new Student()));
            Method_BaseEvent_DerivedArg(new EntityUpdatedEventData<Student>(new Student()));
        }

        [Fact]
        public void Test_Method_DerivedEvent_BaseArg()
        {
            Method_DerivedEvent_BaseArg(new EntityUpdatedEventData<Person>(new Person()));
            Method_DerivedEvent_BaseArg(new EntityUpdatedEventData<Person>(new Student()));
        }

        [Fact]
        public void Test_Method_DerivedEvent_DerivedArg()
        {
            Method_DerivedEvent_DerivedArg(new EntityUpdatedEventData<Student>(new Student()));
        }
    }
}