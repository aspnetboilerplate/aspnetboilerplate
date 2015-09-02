using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using Xunit;

namespace Abp.Tests.Experimental
{
    public class GenericMethodCallTests
    {
        [Fact]
        public void Test_Method_BaseEvent_BaseArg()
        {
            Method_BaseEvent_BaseArg(new EntityEventData<Person>(new Person())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityEventData<Person>(new Student())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityUpdatedEventData<Person>(new Person())); //TODO: <Student>
            Method_BaseEvent_BaseArg(new EntityUpdatedEventData<Person>(new Student())); //TODO: <Student>
        }

        public void Method_BaseEvent_BaseArg(EntityEventData<Person> data)
        {

        }

        [Fact]
        public void Test_Method_BaseEvent_DerivedArg()
        {
            Method_BaseEvent_DerivedArg(new EntityEventData<Student>(new Student()));
            Method_BaseEvent_DerivedArg(new EntityUpdatedEventData<Student>(new Student()));
        }

        public void Method_BaseEvent_DerivedArg(EntityEventData<Student> data)
        {

        }

        [Fact]
        public void Test_Method_DerivedEvent_BaseArg()
        {
            Method_DerivedEvent_BaseArg(new EntityUpdatedEventData<Person>(new Person()));
            Method_DerivedEvent_BaseArg(new EntityUpdatedEventData<Person>(new Student()));
        }

        public void Method_DerivedEvent_BaseArg(EntityUpdatedEventData<Person> data)
        {

        }

        [Fact]
        public void Test_Method_DerivedEvent_DerivedArg()
        {
            Method_DerivedEvent_DerivedArg(new EntityUpdatedEventData<Student>(new Student()));
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
    }
}