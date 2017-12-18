### Introduction

Most **SaaS** (multi-tenant) applications have **editions** (packages)
those have different **features**. Thus, they can provide different
**price and feature options** to thier tenants (customers).

ASP.NET Boilerplate provides a **feature system** to make it easier. We
can **define** features, **check** if a feature is **enabled** for a
tenant and **integrate** feature system to other ASP.NET Boilerplate
concepts (like [authorization](Authorization.md) and
[navigation](Navigation.md)).

#### About IFeatureValueStore

Feature system uses **IFeatureValueStore** to get values of features.
While you can implement it in your own way, it's fully implemented in
**module-zero** project. If it's not implemented, NullFeatureValueStore
is used which returns null for all features (Default feature values are
used in this case).

### Feature Types

There are two fundamental feature types.

#### Boolean Feature

Can be "true" or "false". This type of a feature can be **enabled** or
**disabled** (for an edition or for a tenant).

#### Value Feature

Can be an **arbitrary value**. While it's stored and retrieved a string,
numbers also can be stored as strings.

For example, our application may be a task management application and we
may have a limit for creating tasks in a month. Say that we have two
different edition/package; one allows creating 1,000 tasks per month,
while other allows creating 5,000 tasks per month. So, this feature
should be stored as value, not simply true/false.

### Defining Features

A feature should be defined before checking. A
[module](/Pages/Documents/Module-System) can define it's own features by
deriving from **FeatureProvider** class. Here, a very simple feature
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

After creating a feature provider, we should register it in our module's
[PreInitialize](/Pages/Documents/Module-System#preinitialize) method
as shown below:

    Configuration.Features.Providers.Add<AppFeatureProvider>();

#### Basic Feature Properties

A feature definition requires two properties at least:

-   **Name**: A unique name (as string) to identify the feature.
-   **Default value**: A default value. This is used when we need the
    value of the feature and it's not available for current tenant.

Here, we defined a boolean feature named "SampleBooleanFeature" which's
default value is "false" (not enabled). We also defined two value
features (SampleNumericFeature is defined as child of
SampleBooleanFeature).

Tip: Create a const string for a feature name and use it everywhere to
prevent typing errors.

#### Other Feature Properties

While unique name and default value properties are required, there are
some optional properties for a detailed control.

-   **Scope**: A value in FeatureScopes enum. It can be **Edition** (if
    this feature can be set only for edition level), **Tenant** (if this
    feature can be set only for tenant level) or **All** (if this
    feature can be set for editions and tenants, where tenant setting
    overrides it's edition's setting). Default value is **All**.
-   **DisplayName**: A localizable string to show the feature's name to
    users.
-   **Description**: A localizable string to show the feature's detailed
    description to users.
-   **InputType**: A UI input type for the feature. This can be defined,
    then can be used while creating an automatic feature screen.
-   **Attributes**: An arbitrary custom dictionary of key-value pairs
    those can be related to the feature.

Let's see more detailed definitions for the features above:

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

Note that: Input type definitions are not used by ASP.NET Boilerplate.
They can be used by applications while creating inputs for features.
ASP.NET Boilerplate just provides infrastructure to make it easier.

#### Feature Hierarchy

As shown in the sample feature providers, a feature can have **child
features**. A Parent feature is generally defined as **boolean**
feature. Child features will be available only if the parent enabled.
ASP.NET Boilerplate **does not** enforces but suggests this.
Applications should take care of it.

### Checking Features

We define a feature to check it's value in the application to allow or
block some application features per tenant. There are different ways of
checking it.

#### Using RequiresFeature Attribute

We can use **RequiredFeature** attribute for a method or a class as
shown below:

    [RequiresFeature("ExportToExcel")]
    public async Task<FileDto> GetReportToExcel(...)
    {
        ...
    }

This method is executed only if "ExportToExcel" feature is enabled for
the **current tenant** (current tenant is obtained from
[IAbpSession](/Pages/Documents/Abp-Session)). If it's not enabled, an
**AbpAuthorizationException** is thrown automatically.

Surely, RequiresFeature attribute should be used for **boolean type
features**. Otherwise, you may get exceptions.

##### RequiresFeature attribute notes

ASP.NET Boilerplate uses power of dynamic method interception for
feature checking. So, there is some restrictions for the methods use
RequiresFeature attribute.

-   Can not use it for private methods.
-   Can not use it for static methods.
-   Can not use it for methods of a non-injected class (We must use
    [dependency injection](/Pages/Documents/Dependency-Injection)).

Also,

-   Can use it for any **public** method if the method is called over an
    **interface** (like Application Services used over interface).
-   A method should be **virtual** if it's called directly from class
    reference (like ASP.NET MVC or Web API Controllers).
-   A method should be **virtual** if it's **protected**.

#### Using IFeatureChecker

We can inject and use IFeatureChecker to check a feature manually (it's
automatically injected and directly usable for application services, MVC
and Web API controllers).

##### IsEnabled

Used to simply check if given feature is enabled or not. Example:

    public async Task<FileDto> GetReportToExcel(...)
    {
        if (await FeatureChecker.IsEnabledAsync("ExportToExcel"))
        {
            throw new AbpAuthorizationException("You don't have this feature: ExportToExcel");
        }

        ...
    }

IsEnabledAsync and other methods have also sync versions.

Surely, IsEnabled method should be used for **boolean type features**.
Otherwise, you may get exceptions.

If you just want to check a feature and throw exception as shown in the
example, you can just use **CheckEnabled** method.

##### GetValue

Used to get current value of a feature for value type features. Example:

    var createdTaskCountInThisMonth = GetCreatedTaskCountInThisMonth();
    if (createdTaskCountInThisMonth >= FeatureChecker.GetValue("MaxTaskCreationLimitPerMonth").To<int>())
    {
        throw new AbpAuthorizationException("You exceed task creation limit for this month, sorry :(");
    }

FeatureChecker methods have also overrides to work features for a
specified tenantId, not only for the current tenantId.

#### Client Side

In the client side (javascript), we can use **abp.features** namespace
to get current values of the features.

##### isEnabled

    var isEnabled = abp.features.isEnabled('SampleBooleanFeature');

##### getValue

    var value = abp.features.getValue('SampleNumericFeature');

### Feature Manager

If you need to definitions of features, you can inject and use
**IFeatureManager**.

### A Note For Editions

ASP.NET Boilerplate framework has not a built-in edition system because
such a system requires a database (to store editions, edition features,
tenant-edition mappings and so on...). Therefore, edition system is
implemented in [module zero](/Pages/Documents/Zero/Edition-Management).
You can use it to easily have an edition system, or you can implement
all yourself.
