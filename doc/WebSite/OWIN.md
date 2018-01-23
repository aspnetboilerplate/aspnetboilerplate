If you are using both **ASP.NET MVC** and **ASP.NET Web API** in your
application, you need to add the
[**Abp.Owin**](https://www.nuget.org/packages/Abp.Owin) NuGet package to
your project.

### Installation

Add the [**Abp.Owin**](https://www.nuget.org/packages/Abp.Owin) NuGet
package to your host project (generally, to the **Web** project).

    Install-Package Abp.Owin

### Usage

Then call the **UseAbp()** extension method in your OWIN **Startup** file as
shown below:

    [assembly: OwinStartup(typeof(Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAbp();

            //your other configuration...
        }
    }

If you are only using OWIN (say, in a self hosted Web API project), you
can use the override of UseAbp which takes a startup module to initialize
ABP framework. Note that this should only be done if ABP is not
initialized in another way.
