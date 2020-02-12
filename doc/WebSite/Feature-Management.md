### Introduction

Most **SaaS** (multi-tenant) applications have **editions** (packages)
that have different **features**. This way, they can provide different
**price and feature options** to their tenants (customers).

ASP.NET Boilerplate provides a **feature system** to make it easier. You
can **define** features, **check** if a feature is **enabled** for a
tenant, and **integrate** the feature system to other ASP.NET Boilerplate
concepts (like [authorization](Authorization.md) and
[navigation](Navigation.md)).

#### About IFeatureValueStore

The feature system uses the **IFeatureValueStore** to get the values of features.
While you can implement it in your own way, it's fully implemented in the
**Module Zero** project. If it's not implemented, NullFeatureValueStore
is used to return null for all features (so the default feature values are
used in this case).

### Feature Types

There are two fundamental feature types.

#### Boolean Feature

Can be "true" or "false". This type of a feature can be **enabled** or
**disabled** (for an edition or for a tenant).

#### Value Feature

Can be an **arbitrary value**. While it's stored and retrieved as a string,
numbers also can be stored as strings.

For example, our application may be a task management application and we
may have a limit for creating tasks in a month. Imagine that we have two
different editions/packages; one allows for creating 1,000 tasks per month,
while the other allows for creating 5,000 tasks per month. This feature
should be stored as a value, not simply as true or false.

### Defining Features

A feature should be defined before it is checked. A
[module](/Pages/Documents/Module-System) can define its own features by
deriving from the **FeatureProvider** class. Here's a very simple feature
provider that defines 3 features:

    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var sampleBooleanFeature = context.Create("SampleBooleanFeature", defaultValue: "false");
            sampleBooleanFeature.CreateChildFeature("SampleNumericFeature", defaultValue: "10");
            context.Create("SampleSelectionFeature", defaultValue: "B");
        }
    }

After creating a feature provider, we must register it in our module's
[PreInitialize](/Pages/Documents/Module-System#preinitialize) method
as shown below:

    Configuration.Features.Providers.Add<AppFeatureProvider>();

#### Basic Feature Properties

A feature definition requires at least two properties:

-   **Name**: A unique name (string) to identify the feature.
-   **Default value**: A default value. This is used when we need the
    value of the feature and it's not available for current tenant.

Here, we defined a boolean feature named "SampleBooleanFeature", with the
default value of "false" (not enabled). We also defined two value
features. Note that SampleNumericFeature is defined as a child of
SampleBooleanFeature.

Tip: Create a const string for a feature name and use it everywhere to
prevent typing errors.

#### Other Feature Properties

While the unique name and default value properties are required, there are
some optional properties for more fine-tuned control.

-   **Scope**: A value in the FeatureScopes enum. It can be **Edition** (if
    this feature can be set only for edition level), **Tenant** (if this
    feature can be set only for tenant level) or **All** (if this
    feature can be set for editions and tenants, where a tenant setting
    overrides its edition's setting). Default value is **All**.
-   **DisplayName**: A localizable string to show the feature's name to
    users.
-   **Description**: A localizable string to show the feature's detailed
    description to users.
-   **InputType**: A UI input type for the feature. This can be defined,
    and then used while creating an automatic feature screen.
-   **Attributes**: An arbitrary custom dictionary of key-value pairs
    that are related to the feature.

Let's see some more detailed definitions for the features above:

    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var sampleBooleanFeature = context.Create(
                AppFeatures.SampleBooleanFeature,
                defaultValue: "false",
                displayName: L("Sample boolean feature"),
                inputType: new CheckboxInputType()
                );

            sampleBooleanFeature.CreateChildFeature(
                AppFeatures.SampleNumericFeature,
                defaultValue: "10",
                displayName: L("Sample numeric feature"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(1, 1000000))
                );

            context.Create(
                AppFeatures.SampleSelectionFeature,
                defaultValue: "B",
                displayName: L("Sample selection feature"),
                inputType: new ComboboxInputType(
                    new StaticLocalizableComboboxItemSource(
                        new LocalizableComboboxItem("A", L("Selection A")),
                        new LocalizableComboboxItem("B", L("Selection B")),
                        new LocalizableComboboxItem("C", L("Selection C"))
                        )
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroTemplateConsts.LocalizationSourceName);
        }
    }

Note that the Input type definitions are not used by ASP.NET Boilerplate.
They can be used by applications to create inputs for features.
ASP.NET Boilerplate just provides the infrastructure to make it easier.

#### Feature Hierarchy

As shown in the sample feature providers, a feature can have **child features**. 
A Parent feature is generally defined as a **boolean**
feature. Child features will be available only if the parent is enabled.
ASP.NET Boilerplate **does not** enforce this, but we recommend it.
The application should take care of it.

### Checking Features

We define a feature to check its value in the application to allow or
block some application features per tenant. There are different ways of
checking it.

#### Using RequiresFeature Attribute

We can use the **RequiredFeature** attribute for a method or a class as
shown below:

    [RequiresFeature("ExportToExcel")]
    public async Task<FileDto> GetReportToExcel(...)
    {
        ...
    }

This method is executed only if the "ExportToExcel" feature is enabled for
the **current tenant** (current tenant is obtained from
[IAbpSession](/Pages/Documents/Abp-Session)). If it's not enabled, an
**AbpAuthorizationException** is thrown automatically.

As such, the RequiresFeature attribute should only be used for **boolean type
features**. Otherwise, you may get exceptions.

##### RequiresFeature attribute notes

ASP.NET Boilerplate uses the power of dynamic method interception for
feature checking. There are some restrictions for the methods that can use the
RequiresFeature attribute.

-   You can not use it for private methods.
-   You can not use it for static methods.
-   You can not use it for methods of a non-injected class (We must use
    [dependency injection](/Pages/Documents/Dependency-Injection)).

Also,

-   You can use it for any **public** method if the method is called over an
    **interface** (like Application Services used over interface).
-   A method should be **virtual** if it's called directly from a class
    reference (like ASP.NET MVC or Web API Controllers).
-   A method should be **virtual** if it's **protected**.

#### Using IFeatureChecker

We can inject and use IFeatureChecker to check a feature manually (it's
automatically injected and directly usable for application services, MVC,
and Web API controllers).

##### IsEnabled

This is used to simply check if a given feature is enabled or not. Example:

    public async Task<FileDto> GetReportToExcel(...)
    {
        if (await FeatureChecker.IsEnabledAsync("ExportToExcel"))
        {
            throw new AbpAuthorizationException("You don't have this feature: ExportToExcel");
        }

        ...
    }

The IsEnabledAsync and other methods also have sync versions.

The IsEnabled method should be used for **boolean type features**,
otherwise you may get exceptions.

If you just want to check a feature and throw an exception as shown in the
example, you can use the **CheckEnabled** method.

##### GetValue

Used to get the current value of a feature for value-type features. Example:

    var createdTaskCountInThisMonth = GetCreatedTaskCountInThisMonth();
    if (createdTaskCountInThisMonth >= FeatureChecker.GetValue("MaxTaskCreationLimitPerMonth").To<int>())
    {
        throw new AbpAuthorizationException("You exceed task creation limit for this month, sorry :(");
    }

The FeatureChecker methods also have overrides to check features not only for the
current tenantId, but for a **specified** tenantId as well.

#### Client Side

On the client side (JavaScript), we can use the **abp.features** namespace
to get the current values of features.

##### isEnabled
```csharp
    var isEnabled = abp.features.isEnabled('SampleBooleanFeature');
```
##### getValue
```csharp
    var value = abp.features.getValue('SampleNumericFeature');
```

#### Ignore Feature Check For Host Users

If you enabled Multi-Tenancy, then you can also ignore feature check for host users by configuring it in PreInitialize method of our module as shown below:

    Configuration.MultiTenancy.IgnoreFeatureCheckForHostUsers = true;
    
**Note:** `IgnoreFeatureCheckForHostUsers` default value is `false`;

### Feature Manager

If you need the definitions of features, you can inject and use
**IFeatureManager**.

### A Note For Editions

The ASP.NET Boilerplate framework does not have a built-in edition system because
such a system requires a database (to store editions, edition features,
tenant-edition mappings and so on...). Therefore, the edition system is
implemented in [Module Zero](/Pages/Documents/Zero/Edition-Management).
You can use it as a ready-made edition system or implement
one yourself.
