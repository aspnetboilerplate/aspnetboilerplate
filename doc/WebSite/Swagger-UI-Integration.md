### Introduction

From it's web site: "....with a Swagger-enabled API, you get
**interactive documentation**, client SDK generation and
discoverability."

### ASP.NET Core

#### Install NuGet Package

Install the
**[Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)**
NuGet package to your **Web** project.

#### Configure

Add the following configuration code for Swagger into the **ConfigureServices** method of
your **Startup.cs**

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        //your other code...
        
          services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info { Title = "AbpZeroTemplate API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                });
        
        //your other code...
    }

Then, to use swagger, add this code into the **Configure** method of **Startup.cs** 

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        //your other code...

         app.UseSwagger();
                //Enable middleware to serve swagger - ui assets(HTML, JS, CSS etc.)
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AbpZeroTemplate API V1");
                }); //URL: /swagger 
                
        //your other code...
    }

#### Test

That's it! You can now browse the swagger ui under "**/swagger**".

### ASP.NET 5.x

#### Install NuGet Package

Install the
**[Swashbuckle.Core](https://www.nuget.org/packages/Swashbuckle.Core/)**
NuGet package to your **WebApi** project (or Web project).

#### Configure

Add the configuration code for Swagger into the
[Initialize](/Pages/Documents/Module-System) method of your module.
Example:

    public class SwaggerIntegrationDemoWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            //your other code...

            ConfigureSwaggerUi();
        }

        private void ConfigureSwaggerUi()
        {
            Configuration.Modules.AbpWebApi().HttpConfiguration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SwaggerIntegrationDemo.WebApi");
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                })
                .EnableSwaggerUi(c =>
                {
                    c.InjectJavaScript(Assembly.GetAssembly(typeof(AbpProjectNameWebApiModule)), "AbpCompanyName.AbpProjectName.Api.Scripts.Swagger-Custom.js");
                });
        }
    }

Note that we inject a JavaScript file named "**Swagger-Custom.js**"
while configuring the swagger ui. This script file is used to add a CSRF token
to requests while testing api services in the ui. You may also need to
add this file to your WebApi project as an **embedded resource** and use
it's Logical Name in the InjectJavaScript method while injecting it.

**IMPORTANT**: The code above will be slightly different for your
project (Namespace will not be AbpCompanyName.AbpProjectName... and
AbpProjectNameWebApiModule will be *YourProjectName*WebApiModule).

Here's the content of the **Swagger-Custom.js**:

    var getCookieValue = function(key) {
        var equalities = document.cookie.split('; ');
        for (var i = 0; i < equalities.length; i++) {
            if (!equalities[i]) {
                continue;
            }

            var splitted = equalities[i].split('=');
            if (splitted.length !== 2) {
                continue;
            }

            if (decodeURIComponent(splitted[0]) === key) {
                return decodeURIComponent(splitted[1] || '');
            }
        }

        return null;
    };

    var csrfCookie = getCookieValue("XSRF-TOKEN");
    var csrfCookieAuth = new SwaggerClient.ApiKeyAuthorization("X-XSRF-TOKEN", csrfCookie, "header");
    swaggerUi.api.clientAuthorizations.add("X-XSRF-TOKEN", csrfCookieAuth);

See the Swashbuckle
[documentation](https://github.com/domaindrivendev/Swashbuckle) for more
configuration options.

#### Test

That's it! Let's browse to **/swagger/ui/index**:

<img src="images/swagger-ui.png" alt="Swagger UI" class="img-thumbnail" />

You can see all the Web API Controllers (and also the [dynamic web
api](/Pages/Documents/Dynamic-Web-API) controllers) and test them.
