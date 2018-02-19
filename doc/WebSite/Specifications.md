### Introduction

The ***specification pattern*** *is a particular software design pattern,
whereby business rules can be recombined by chaining the business rules
together using boolean logic*
([Wikipedia](https://en.wikipedia.org/wiki/Specification_pattern)).

In pratical terms, it's used mostly **to define reusable filters** for
entities or other business objects.

#### Example

In this section, we will see **the need for the specification pattern**.
This section is generic and not related to ABP's implementation.

Assume that you have a service method that calculates the total count of
your customers as shown below:

    public class CustomerManager
    {
        public int GetCustomerCount()
        {
            //TODO...
            return 0;
        }
    }

You probably will want to get a customer count with a filter. For example,
you may have **premium customers** (which have a balance of more than
$100,000) or you may want to filter customers just by **registration
year**. Then you can create other methods like
*GetPremiumCustomerCount()*, *GetCustomerCountRegisteredInYear(int
year)*, *GetPremiumCustomerCountRegisteredInYear(int year)* and more. As
you have more criteria, it's not possible to create a combination for
every possibility.

One solution to this problem is the **specification pattern**. We could
create a single method that gets **a parameter as the filter**:

    public class CustomerManager
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerManager(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public int GetCustomerCount(ISpecification<Customer> spec)
        {
            var customers = _customerRepository.GetAllList();

            var customerCount = 0;
            
            foreach (var customer in customers)
            {
                if (spec.IsSatisfiedBy(customer))
                {
                    customerCount++;
                }
            }

            return customerCount;
        }
    }

This way, we can get any object as a parameter that implements the
**ISpecification&lt;Customer&gt;** interface which is defined as shown
below:

    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T obj);
    }

We can call **IsSatisfiedBy** with a customer to test if this
customer is intended. This way, we can use same GetCustomerCount with
different filters **without changing the method** itself.

While this solution is pretty fine in theory, it could be improved to
work better in C\#. For instance, it's **not efficient** to get all
customers from a database to check if they satisfy the given
specification/condition. In the next section, we will see ABP's
implementation which overcomes this problem.

### Creating Specification Classes

ABP defines the ISpecification interface as shown below:

    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T obj);

        Expression<Func<T, bool>> ToExpression();
    }

Includes a **ToExpression()** method which returns an expression and is used to
better integrate with **IQueryable** and **Expression trees**. This way, we
can easily pass a specification to a repository to apply a filter at the
database level.

We generally inherit from the **Specification&lt;T&gt; class** instead of
directly implementing the ISpecification&lt;T&gt; interface. A Specification
class automatically implements the IsSatisfiedBy method, so we only need to
define ToExpression. Let's create some specification classes:

    //Customers with $100,000+ balance are assumed as PREMIUM customers.
    public class PremiumCustomerSpecification : Specification<Customer>
    {
        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return (customer) => (customer.Balance >= 100000);
        }
    }

    //A parametric specification example.
    public class CustomerRegistrationYearSpecification : Specification<Customer>
    {
        public int Year { get; }

        public CustomerRegistrationYearSpecification(int year)
        {
            Year = year;
        }

        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return (customer) => (customer.CreationYear == Year);
        }
    }

As you can see, we just implemented simple **lambda expressions** to define the
specifications. Let's use these specifications to get the counts of the
customers:

    count = customerManager.GetCustomerCount(new PremiumCustomerSpecification());
    count = customerManager.GetCustomerCount(new CustomerRegistrationYearSpecification(2017));

### Using a Specification With a Repository

We can now **optimize** CustomerManager to **apply the filter in the
database**:

    public class CustomerManager
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerManager(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public int GetCustomerCount(ISpecification<Customer> spec)
        {
            return _customerRepository.Count(spec.ToExpression());
        }
    }

It's that simple. We can pass any specification to the repositories since the
[repositories](Repositories.md) can work with the expressions as filters.
In this example, CustomerManager is unnecessary since we could directly
use the repository with the specification to query the database. But imagine that
we want to execute a business operation on some customers. In that case,
we could use the specifications with a domain service to specify the customers
to work on.

### Composing Specifications

One powerful feature of specifications is that they are **composable**
with **And, Or, Not** and **AndNot** extension methods. Example:

    var count = customerManager.GetCustomerCount(new PremiumCustomerSpecification().And(new CustomerRegistrationYearSpecification(2017)));

We can even create a new specification class from existing
specifications:

    public class NewPremiumCustomersSpecification : AndSpecification<Customer>
    {
        public NewPremiumCustomersSpecification() 
            : base(new PremiumCustomerSpecification(), new CustomerRegistrationYearSpecification(2017))
        {
        }
    }

**AndSpecification** is a subclass of the **Specification** class which
satisfies only if both specifications are satisfied. We can then use
NewPremiumCustomersSpecification just like any other specification:

    var count = customerManager.GetCustomerCount(new NewPremiumCustomersSpecification());

### Discussion

While the specification pattern is older than C\# lambda expressions, it's
generally compared to expressions. Some developers may think it's not
needed anymore and we can directly pass expressions to a repository or
to a domain service as shown below:

    var count = _customerRepository.Count(c => c.Balance > 100000 && c.CreationYear == 2017);

Since ABP's [Repository](Repositories.md) supports expessions, this is a
completely valid use. You don't have to define or use any
specification in your application and you can go with expressions. 

So, what's the point of a specification? Why and when should we consider
using them?

#### When To Use?

Some benefits of using specifications:

-   **Reusabe**: Imagine that you need the PremiumCustomer filter in many
    places in your code base. If you go with expressions and do not create
    a specification, what happens if you later change the "Premium Customer"
    definition? Say you want to change the minimum balance from $100,000 to
    $250,000 and add another condition to be a customer older than 3. 
    If you used a specification, you just change a single class. If
    you repeated (copy/pasted) the same expression, you need to change all of
    them.
-   **Composable**: You can combine multiple specifications to create new
    specifications. This is another type of reusability.
-   **Named**: PremiumCustomerSpecification better explains the intent
    rather than a complex expression. So, if you have an expression that
    is meaningful in your business, consider using specifications.
-   **Testable**: A specification is a separately (and easily) testable
    object.

#### When To Not Use?

-   **Non business expressions**: Do not use
    specifications for non business-related expressions and operations.
-   **Reporting**: If you are just creating a report do not create
    specifications, but directly use IQueryable. You can even
    use plain SQL, Views or another tool for reporting. DDD does not necessarily care about
    reporting, so the way you query the underlying data store can be important
    from a performance perspective.