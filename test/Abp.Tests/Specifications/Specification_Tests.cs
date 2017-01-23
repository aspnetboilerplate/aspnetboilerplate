using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Specifications;
using Shouldly;
using Xunit;
using Abp.Domain.Specifications;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace Abp.Tests.Specifications
{
    public class Specification_Tests
    {
        private readonly IQueryable<Customer> _customers;

        public Specification_Tests()
        {
            _customers = new List<Customer>
            {
                new Customer(1,"John", 17, 47000, "England"),
                new Customer(2,"Tuana", 2, 500, "Turkey"),
                new Customer(3,"Martin", 43, 16000, "USA"),
                new Customer(4,"Lee", 32, 24502, "China"),
                new Customer(5,"Douglas", 42, 42000, "England"),
                new Customer(6,"Abelard", 14, 2332, "German"),
                new Customer(7,"Neo", 16, 120000, "USA"),
                new Customer(8,"Daan", 39, 6000, "Netherlands"),
                new Customer(9,"Alessandro", 22, 8271, "Italy"),
                new Customer(10,"Noah", 33, 82192, "Belgium")
            }.AsQueryable();
        }

        [Fact]
        public void Any_Should_Return_All()
        {
            _customers
                .Where(new AnySpecification<Customer>()) //Implicitly converted to Expression!
                .Count()
                .ShouldBe(_customers.Count());
        }

        [Fact]
        public void None_Should_Return_Empty()
        {
            _customers
                .Where(new NoneSpecification<Customer>().ToExpression())
                .Count()
                .ShouldBe(0);
        }

        [Fact]
        public void Not_Should_Return_Reverse_Result()
        {
            _customers
                .Where(new EuropeanCustomerSpecification().Not().ToExpression())
                .Count()
                .ShouldBe(3);
        }

        [Fact]
        public void Should_Support_Native_Expressions_And_Combinations()
        {
            _customers
                .Where(new DirectSpecification<Customer>(c => c.Age >= 18).ToExpression())
                .Count()
                .ShouldBe(6);

            _customers
                .Where(new EuropeanCustomerSpecification().And(new DirectSpecification<Customer>(c => c.Age >= 18)).ToExpression())
                .Count()
                .ShouldBe(4);
        }

        [Fact]
        public void CustomSpecification_Test()
        {
            _customers
                .Where(new EuropeanCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(7);

            _customers
                .Where(new Age18PlusCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(6);

            _customers
                .Where(new BalanceCustomerSpecification(10000, 30000).ToExpression())
                .Count()
                .ShouldBe(2);

            _customers
                .Where(new PremiumCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(3);
        }

        [Fact]
        public void IsSatisfiedBy_Tests()
        {
            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer(1, "David", 49, 55000, "USA")).ShouldBeTrue();

            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer(2, "David", 49, 200, "USA")).ShouldBeFalse();
            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer(3, "David", 12, 55000, "USA")).ShouldBeFalse();
        }

        [Fact]
        public void CustomSpecification_Composite_Tests()
        {
            _customers
                .Where(new EuropeanCustomerSpecification().And(new Age18PlusCustomerSpecification()).ToExpression())
                .Count()
                .ShouldBe(4);

            _customers
               .Where(new EuropeanCustomerSpecification().Not().And(new Age18PlusCustomerSpecification()).ToExpression())
               .Count()
               .ShouldBe(2);

            _customers
                .Where(new Age18PlusCustomerSpecification() & !(new EuropeanCustomerSpecification()))
                .Count()
                .ShouldBe(2);
        }

        [Fact]
        public void EntitySpecification_Tests()
        {
            var spec = CustomerSpecification.Any();
            _customers.Where(spec).Count().ShouldBe(10);

            spec = CustomerSpecification.IsKey(2);
            _customers.Where(spec).Count().ShouldBe(1);

            spec = CustomerSpecification.InKeys(new List<int>() { 1, 3, 5 });
            _customers.Where(spec).Count().ShouldBe(3);

            spec = CustomerSpecification.FromEngland()
                & CustomerSpecification.Grown();
            _customers.Where(spec).Count().ShouldBe(1);

            spec = CustomerSpecification.GrownBigCustomer();
            _customers.Where(spec).Count().ShouldBe(4);
        }

        [Fact]
        public void EntitySpecification_Operater_Tests()
        {

            var spec = CustomerSpecification.FromEngland()
                 & CustomerSpecification.Grown();
            _customers.Where(spec).Count().ShouldBe(1);

            spec = CustomerSpecification.FromEngland()
                | CustomerSpecification.FromUSA();
            _customers.Where(spec).Count().ShouldBe(4);

            spec = !CustomerSpecification.FromEngland()
                & CustomerSpecification.Grown();
            _customers.Where(spec).Count().ShouldBe(5);
        }

        [Fact]
        public void EntitySpecification_Condition_Tests()
        {
            //Define Condition,we alawys use Nullable<T> to ensure this condition need to be set in search
            string nameStartWith = "A";
            int? ageMaxThan = null;
            int? balanceMaxThan = 5000;

            //Construct Spec
            var spec = CustomerSpecification.Any()
                .AndIf(!nameStartWith.IsNullOrEmpty(), CustomerSpecification.NameStartWith(nameStartWith))
                .AndIf(ageMaxThan.HasValue, () => { return CustomerSpecification.AgeMaxThan(ageMaxThan.Value); })
                .AndIf(balanceMaxThan.HasValue, () => { return CustomerSpecification.BalanceMaxThan(balanceMaxThan.Value); });

            _customers.Where(spec).Count().ShouldBe(1);


            nameStartWith = "";
            ageMaxThan = 20;
            balanceMaxThan = 5000;

            spec = CustomerSpecification.Any()
                .AndIf(!nameStartWith.IsNullOrEmpty(), CustomerSpecification.NameStartWith(nameStartWith))
                .AndIf(ageMaxThan.HasValue, () => { return CustomerSpecification.AgeMaxThan(ageMaxThan.Value); })
                .AndIf(balanceMaxThan.HasValue, () => { return CustomerSpecification.BalanceMaxThan(balanceMaxThan.Value); });

            _customers.Where(spec).Count().ShouldBe(6);
        }
    }

    public class Customer : Entity<int>
    {

        public string Name { get; private set; }

        public byte Age { get; private set; }

        public long Balance { get; private set; }

        public string Location { get; private set; }

        public Customer(int id, string name, byte age, long balance, string location)
        {
            Id = id;
            Name = name;
            Age = age;
            Balance = balance;
            Location = location;
        }
    }

    public class EuropeanCustomerSpecification : Specification<Customer>
    {
        public override Expression<Func<Customer, bool>> SatisfiedBy()
        {
            return c => c.Location == "England" ||
                        c.Location == "Turkey" ||
                        c.Location == "German" ||
                        c.Location == "Netherlands" ||
                        c.Location == "Italy" ||
                        c.Location == "Belgium";
        }
    }

    public class Age18PlusCustomerSpecification : Specification<Customer>
    {
        public override Expression<Func<Customer, bool>> SatisfiedBy()
        {
            return c => c.Age >= 18;
        }
    }

    public class BalanceCustomerSpecification : Specification<Customer>
    {
        public int MinBalance { get; }

        public int MaxBalance { get; }

        public BalanceCustomerSpecification(int minBalance, int maxBalance)
        {
            MinBalance = minBalance;
            MaxBalance = maxBalance;
        }

        public override Expression<Func<Customer, bool>> SatisfiedBy()
        {
            return c => c.Balance >= MinBalance && c.Balance <= MaxBalance;
        }
    }

    public class PremiumCustomerSpecification : Specification<Customer>
    {
        private Specification<Customer> _RightSideSpecification = null;
        private Specification<Customer> _LeftSideSpecification = null;

        public PremiumCustomerSpecification()
        {
            _RightSideSpecification = new Age18PlusCustomerSpecification();
            _LeftSideSpecification = new BalanceCustomerSpecification(20000, int.MaxValue);
        }

        public override Expression<Func<Customer, bool>> SatisfiedBy()
        {
            return (_RightSideSpecification & _LeftSideSpecification).SatisfiedBy();
        }
    }

    public class CustomerSpecification : EntitySpecification<Customer, int>
    {
        public static Specification<Customer> AgeMaxThan(int age)
        {
            return new DirectSpecification<Customer>(c => c.Age >= age);
        }

        public static Specification<Customer> Grown()
        {
            return new DirectSpecification<Customer>(c => c.Age >= 18);
        }

        public static Specification<Customer> FromEngland()
        {
            return new DirectSpecification<Customer>(c => c.Location == "England");
        }

        public static Specification<Customer> FromUSA()
        {
            return new DirectSpecification<Customer>(c => c.Location == "USA");
        }

        public static Specification<Customer> NameStartWith(string nameIndex)
        {
            return new DirectSpecification<Customer>(c => c.Name.StartsWith(nameIndex));
        }

        public static Specification<Customer> GrownBigCustomer()
        {
            return new DirectSpecification<Customer>(c => c.Age >= 18 && c.Balance >= 10000);
        }

        public static Specification<Customer> BalanceMaxThan(int balance)
        {
            return new DirectSpecification<Customer>(c => c.Balance >= balance);
        }
    }
}
