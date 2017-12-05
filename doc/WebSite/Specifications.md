### Introduction

***Specification pattern*** *is a particular software design pattern,
whereby business rules can be recombined by chaining the business rules
together using boolean logic*
([Wikipedia](https://en.wikipedia.org/wiki/Specification_pattern)).

In pratical, it's mostly used **to define reusable filters** for
entities or other business objects.

#### Example

In this section, we will see **the need for specification pattern**.
This section is generic and not related to ABP's implementation.

Assume that you have a service method that calculates total count of
your customers as shown below:

    public class CustomerManager
    {
        public int GetCustomerCount()
        {
            //TODO...
            return 0;
        }
    }

You probably will want to get customer count by a filter. For example,
you may have **premium customers** (which have balance more than
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

Thus, we can get any object as parameter that implements
**ISpecification&lt;Customer&gt;** interface which is defined as shown
below:

    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T obj);
    }

And we can call **IsSatisfiedBy** with a customer to test if this
customer is intented. Thus, we can use same GetCustomerCount with
different filters **without changing the method** itself.

While this solution is pretty fine in theory, it should be improved to
better work in C\#. For instance, it's **not efficient** to get all
customers from database to check if they satisfy the given
specification/condition. In the next section, we will see ABP's
implementation which overcome this problem.

### Creating Specification Classes

ABP defines the ISpecification interface as shown below:

    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T obj);

        Expression<Func<T, bool>> ToExpression();
    }

Adds a **ToExpression()** method which returns an expression and used to
better integrate with **IQueryable** and **Expression trees**. Thus, we
can easily pass a specification to a repository to apply a filter in the
database level.

We generally inherit from **Specification&lt;T&gt; class** instead of
directly implementing ISpecification&lt;T&gt; interface. Specification
class automatically implements IsSatisfiedBy method. So, we only need to
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

As you see, we just implemented simple **lambda expressions** to define
specifications. Let's use these specifications to get count of
customers:

    count = customerManager.GetCustomerCount(new PremiumCustomerSpecification());
    count = customerManager.GetCustomerCount(new CustomerRegistrationYearSpecification(2017));

### Using Specification With Repository

Now, we can **optimize** CustomerManager to **apply filter in the
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

It's that simple. We can pass any specification to repositories since
[repositories](Repositories.md) can work with expressions as filters.
In this example, CustomerManager is unnecessary since we could directly
use repository with the specification to query database. But think that
we want to execute a business operation on some customers. In that case,
we could use specifications with a domain service to specify customers
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

**AndSpecification** is a subclass of **Specification** class which
satisfies only if both specifications are satisfied. Then we can use
NewPremiumCustomersSpecification just like any other specification:

    var count = customerManager.GetCustomerCount(new NewPremiumCustomersSpecification());

### Discussion

While specification pattern is older than C\# lambda expressions, it's
generally compared to expressions. Some developers may think it's not
needed anymore and we can directly pass expressions to a repository or
to a domain service as shown below:

    var count = _customerRepository.Count(c => c.Balance > 100000 && c.CreationYear == 2017);

Since ABP's [Repository](Repositories.md) supports expessions, this is
completely a valid usage. You don't have to define or use any
specification in your application and you can go with expressions. So,
what's the point of specification? Why and when should we consider to
use them?

#### When To Use?

Some benefits of using specifications:

-   **Reusabe**: Think that you need to PremiumCustomer filter in many
    places in your code base. If you go with expressions and not create
    a specification, what happens if you later change "Premium Customer"
    definition (say, you want to change minimum balance from $100,000 to
    $250,000 and add another condition like to be a customer older than
    3). If you used specification, you just change a single class. If
    you used (copy/paste) same expression, you need to change all of
    them.
-   **Composable**: You can combine multiple specification to create new
    specifications. This is another type of reusability.
-   **Named**: PremiumCustomerSpecification better explains the intent
    rather than a complex expression. So, if you have an expression that
    is meaningful in your business, consider to use specifications.
-   **Testable**: A specification is separately (and easily) testable
    object.

#### When To Not Use?

-   **Non business expressions**: You can consider to not use
    specifications for non business related expressions and operations.
-   **Reporting**: If you are just creating a report do not create
    specifications, but directly use IQueryable. Actually, you can even
    use plain SQL, Views or another tool for reporting. DDD does not
    care on reporting much and getting querying benefits of underlying
    data store can be important from performance point of view.
