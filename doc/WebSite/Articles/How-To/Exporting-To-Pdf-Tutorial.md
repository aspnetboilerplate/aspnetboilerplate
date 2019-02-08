### Introduction

In this tutorial, we will implement a Pdf exporter functionality. We will use open source **[DinkToPdf](https://github.com/rdvojmoc/DinkToPdf)**  library to convert html to pdf.

### Using DinkToPdf Library

First, we install DinkToPdf package to **.application** project.

![DinkToPdf-nuget-install](D:\github\aspnetboilerplate\doc\WebSite\images\DinkToPdf-nuget-install.png)

Then, we need to download the library from **[GitHub repository](https://github.com/rdvojmoc/DinkToPdf)** and copy **v0.12.4** folder to inside of "/aspnet-core/src/{ProjectName}.Web.Mvc/**wkhtmltox**" folder that we have created.

![DinkToPdf-download](D:\github\aspnetboilerplate\doc\WebSite\images\DinkToPdf-download.png)

![DinkToPdf-Copy-files](D:\github\aspnetboilerplate\doc\WebSite\images\DinkToPdf-Copy-files.png)



Finally, we need to add these lines to {ProjectName}.Web.Mvc.csproj:


```xml
  <ItemGroup>
    <None Update="log4net.config">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\32 bit\libwkhtmltox.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\32 bit\libwkhtmltox.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\32 bit\libwkhtmltox.so">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\64 bit\libwkhtmltox.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\64 bit\libwkhtmltox.dylib">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="wkhtmltox\v0.12.4\64 bit\libwkhtmltox.so">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="wwwroot\**\*">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```



### Loading Assemblies

We need to load the assemblies, that we have added to **wkhtmltox** folder, when application is started. Firstly, we will create a new class named "CustomAssemblyLoadContext" in startup folder of Mvc project like below:


```csharp
using System;
using System.Runtime.Loader;
using System.Reflection;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        throw new NotImplementedException();
    }
}
```

And then, use it in ConfigureServices method of startup.cs:

```csharp
using DinkToPdf;
using DinkToPdf.Contracts;
//....

   public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
            _hostingEnvironment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Other codes...

            var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            var wkHtmlToPdfPath = Path.Combine(_hostingEnvironment.ContentRootPath, $"wkhtmltox\\v0.12.4\\{architectureFolder}\\libwkhtmltox");
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            //Other codes...
        }

        //Other codes...
    }
```

Now we are ready to implement the functionality.

### Generating a Pdf file from html

We will convert user list to pdf in this tutorial. So respectively we need to get user list, place it in a html table and convert to pdf. We will create **UserListPdfExporter** class for this:


```csharp
using System.Collections.Generic;
using System.Text;
using ModuleZeroPdfCreationDemo.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace ModuleZeroPdfCreationDemo.Users.exporting
{
    public class UserListPdfExporter : ITransientDependency
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IConverter _converter;

        public UserListPdfExporter(IRepository<User, long> userRepository, IConverter converter)
        {
            _userRepository = userRepository;
            _converter = converter;
        }

        public async Task<FileDto> GetUsersAsPdfAsync() {
            var users = await _userRepository.GetAllListAsync();
            var html = ConvertUserListToHtmlTable(users);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = html
                    }
                }
            };

            return new FileDto("UserList.pdf", _converter.Convert(doc));
        }

        private string ConvertUserListToHtmlTable(List<User> users) {

            var header1 = "<th>Username</th>";
            var header2 = "<th>Name</th>";
            var header3 = "<th>Surname</th>";
            var header4 = "<th>Email Address</th>";

            var headers = $"<tr>{header1}{header2}{header3}{header4}</tr>";

            var rows = new StringBuilder();

            foreach (var user in users)
            {
                var column1 = $"<td>{user.UserName}</td>";
                var column2 = $"<td>{user.Name}</td>";
                var column3 = $"<td>{user.Surname}</td>";
                var column4 = $"<td>{user.EmailAddress}</td>";

                var row = $"<tr>{column1}{column2}{column3}{column4}</tr>";

                rows.Append(row);
            }

            return $"<table>{headers}{rows.ToString()}</table>";
        }
    }

    public class FileDto {

        public string FileName { get; set; }

        public byte[] FileBytes { get; set; }

        public FileDto(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }
    }
}
```



### Download User List as Pdf

To download the pdf that we created using **UserListPdfExporter**, we will add a new method to **UsersController**:

```csharp
using ModuleZeroPdfCreationDemo.Users.exporting;
//...

namespace ModuleZeroPdfCreationDemo.Web.Controllers
{
    public class UsersController : ModuleZeroPdfCreationDemoControllerBase
    {
        //...
        private readonly UserListPdfExporter _userListPdfExporter;**

        public UsersController(/*Other codes....*/ , UserListPdfExporter userListPdfExporter)
        {
            //...
            _userListPdfExporter = userListPdfExporter;
        }

        //Other codes....
        
        public async Task<ActionResult> DownloadAsPdfAsync()
        {
            var file = await _userListPdfExporter.GetUsersAsPdfAsync();

            return File(file.FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.FileName);
        }
    }
}
```


And then we can add a new element to UI for pdf export button.

In Index.cshtml:

```csharp
<button type="button" class="btn btn-primary btn-circle waves-effect waves-circle waves-float pull-right" id="ExportToPdfButton">
       <i class="material-icons">cloud_download</i>
 </button>
```

In Index.js:

```csharp
 $('#ExportToPdfButton').click(function () {
      location.href = abp.appPath + 'Users/DownloadAsPdfAsync';
 });
```

And we are ready to use the new functionality.

![DinkToPdf-ui](D:\github\aspnetboilerplate\doc\WebSite\images\DinkToPdf-ui.png)

![DinkToPdf-pdf](D:\github\aspnetboilerplate\doc\WebSite\images\DinkToPdf-pdf.png)