### Introduction

From it's web site: "....with a Swagger-enabled API, you get
**interactive documentation**, client SDK generation and
discoverability."

### ASP.NET Core

#### Install Nuget Package

Install
**[Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)**
nuget package to your **Web** project.

#### Configure

Add configuration code for Swagger into **ConfigureServices** method of
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

Then, add below code into **Configure** method of **Startup.cs** to use
Swagger

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

That's all. You can browse swagger ui under "**/swagger**".

### ASP.NET 5.x

#### Install Nuget Package

Install
**[Swashbuckle.Core](https://www.nuget.org/packages/Swashbuckle.Core/)**
nuget package to your **WebApi** project (or Web project).

#### Configure

Add configuration code for Swagger into
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

Notice that, we inject a javascript file named "**Swagger-Custom.js**"
while configuring swagger ui. This script file is used to add CSRF token
to requests while testing api services on swagger ui. You also need to
add this file to your WebApi project as **embedded resource** and use
it's Logical Name in InjectJavaScript method while injecting it.

**IMPORTANT**: The code above will be slightly different for your
project (Namespace will not be AbpCompanyName.AbpProjectName... and
AbpProjectNameWebApiModule will be *YourProjectName*WebApiModule).

Content of the **Swagger-Custom.js** here:

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

See Swashbuckle
[documentation](https://github.com/domaindrivendev/Swashbuckle) for more
configuration options.

#### Test

That's all. Let's browse **/swagger/ui/index**:

<img src="images/swagger-ui.png" alt="Swagger UI" class="img-thumbnail" />

You can see all Web API Controllers (and also [dynamic web
api](/Pages/Documents/Dynamic-Web-API) controllers) and test them.
