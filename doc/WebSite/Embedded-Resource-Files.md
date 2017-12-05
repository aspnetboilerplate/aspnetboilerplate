### Introduction

ASP.NET Boilerplate provides an easy way of using embedded **Razor
views** (.cshtml files) and **other resources** (css, js, img... files)
in your web application. You can use this feature to create
[plugins/modules](Module-System.md) that contains UI functionality.

### Create Embedded Files

First, we should create a file and mark it as **embedded resource**. Any
assembly can contain embedded resource files. The progress changes based
on your project format.

#### xproj/project.json Format

Assume that we have a project, named EmbeddedPlugIn, as shown below:

<img src="images/embedded-resource-project-xproj.png" alt="Embedded resource sample project" class="img-thumbnail" />

To make **all files** embedded resource under **Views** folder, we can
add such a configuration to **project.json**:

      "buildOptions": {
        "embed": {
          "include": [
            "Views/**/*.*"
          ]
        }
      }

#### csproj Format

Assume that we have a project, named EmbeddedPlugIn, as shown below:

 <img src="images/embedded-resource-project-csproj.png" alt="Embedded resource project structure" class="img-thumbnail" />

I select **Index.cshtml** file, go to properties window (F4 as shortcut)
and change it's **Build Action** to **Embedded Resource**.

<img src="images/embedded-resource-sample-file-csproj.png" alt="Embedding a file into a c# project" class="img-thumbnail" />

You should change build action to **embedded resource** for **all**
files you want to use in a web application.

### Add To Embedded Resource Manager

Once we embed our files into the assembly, we can use [startup
configuration](Startup-Configuration.md) to add them to embedded
resource manager. You can add such a line to PreInitialize method of
your [module](Module-System.md):

    Configuration.EmbeddedResources.Sources.Add(
        new EmbeddedResourceSet(
            "/Views/",
            Assembly.GetExecutingAssembly(),
            "EmbeddedPlugIn.Views"
        )
    );

Let's explain parameters:

-   First parameter defines **root folder** for files (like
    http://yourdomain.com**/Views/** here). It matches to root
    namespace.
-   Second parameter defines the **Assembly** contains files. This code
    should be located in the assembly containing embedded files.
    Otherwise, you should change this parameter accordingly.
-   And the last one defines **root namespace** of files in the
    assembly. This is the default namespace (generally, the assembly
    name) plus 'folders in the assembly' joined by a dot.

### Consume Embedded Views

For **.cshtml** files, it's straightforward to return them from a
Controller Action. BlogController in the EmbeddedPlugIn assembly is
shown below:

    using Abp.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc;

    namespace EmbeddedPlugIn.Controllers
    {
        public class BlogController : AbpController
        {
            public ActionResult Index()
            {
                return View();
            }
        }
    }

As you see, it's same as regular controllers and works as expected.

### Consume Embedded Resources

To consume embedded resources (js, css, img...), we can just use them in
our views as we normally do:

    @section Styles {
        <link href="~/Views/Blog/Index.css" rel="stylesheet" />
    }

    @section Scripts
    {
        <script src="~/Views/Blog/Index.js"></script>
    }

    <h2 id="BlogTitle">Blog plugin!</h2>

I assumes that the main application has Styles and Scripts sections. We
can also use othe files (like images) as normally we do.

#### ASP.NET Core Configuration

ASP.NET MVC 5.x projects will automatically integrate to embedded
resource manager throught Owin (if your startup file contains
app.UseAbp() as expected). For ASP.NET Core projects, we should manually
add **app.UseEmbeddedFiles()** to the Startup class, just after
app.UseStaticFiles(), as shown below:

    app.UseStaticFiles();
    app.UseEmbeddedFiles(); //Allows to expose embedded files to the web!

#### Ignored Files

Normally, **all files** in the embedded resource manager can be directly
consumed by clients as if they were static files. You can ignore some
file extensions for security and other purposes. **.cshtml** and
**.config** files are ignored by default (for direct requests from
clients). You can add more extensions in PreInitialize of your module as
shown below:

    Configuration.Modules.AbpWebCommon().EmbeddedResources.IgnoredFileExtensions.Add("exe");

### Override Embedded Files

One important feature of embedded resource files is that they **can be
overrided** by higher modules. That means you can create a file with
same name in the same folder in your web application to override an
embedded file (your file in the web application does not require to be
embedded resource, because static files have priority over embedded
files). Thus, you can override css, js or view files of your
modules/plugins in the application. Also, if module A depends on module
B and module A defines an embedded resource with the same path, it can
override an embedded resource file of module B.

Notice that: For ASP.NET Core projects, you should put overriding files
to the wwwroot folder as the root path.
