ASP.NET Boilerplate provides some common utility functions.

#### abp.utils.createNamespace

Used to create deep namespaces at once. Assume that we have a base 'abp'
namespace and want to create or get 'abp.utils.strings.formatting'
namespace. Instead of this:

    //Create or get namespace
    abp.utils = abp.utils || {};
    abp.utils.strings = abp.utils.strings || {};
    abp.utils.strings.formatting = abp.utils.strings.formatting || {};

    //Add a function to the namespace
    abp.utils.strings.formatting.format = function() { ... };

We can write like that:

    var formatting = abp.utils.createNamespace(abp, 'utils.strings.formatting';

    //Add a function to the namespace
    formatting.format = function() { ... };

This simplifies safely creating deep namespaces. Notice that first
argument is the root namespace that must be exists.

#### abp.utils.formatString

Similar to string.Format in C\#. Example usage:

    var str = abp.utils.formatString('Hello {0}!', 'World'); //str = 'Hello World!'
    var str = abp.utils.formatString('{0} number is {1}.', 'Secret', 42); //str = 'Secret number is 42'
