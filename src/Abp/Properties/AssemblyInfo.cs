using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Adorable;

[assembly: AssemblyTitle("ASP.NET Adorable")]
[assembly: AssemblyDescription("ASP.NET Adorable")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ASP.NET Adorable")]
[assembly: AssemblyProduct("Adorable")]
[assembly: AssemblyCopyright("Copyright © 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: InternalsVisibleTo("Adorable.Application")]
[assembly: InternalsVisibleTo("Adorable.EntityFramework")]
[assembly: InternalsVisibleTo("Adorable.NHibernate")]
[assembly: InternalsVisibleTo("Adorable.Web")]
[assembly: InternalsVisibleTo("Adorable.Web.Api")]
[assembly: InternalsVisibleTo("Adorable.Web.Mvc")]
[assembly: InternalsVisibleTo("Adorable.Web.Resources")]

[assembly: InternalsVisibleTo("Adorable.Tests")]
[assembly: InternalsVisibleTo("Adorable.EntityFramework.Tests")]
[assembly: InternalsVisibleTo("Adorable.RedisCache.Tests")]
[assembly: InternalsVisibleTo("Adorable.Web.Tests")]
[assembly: InternalsVisibleTo("Adorable.Web.Api.Tests")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7b50eb47-4993-4a14-b65c-b61714a607b0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(AbpConsts.CurrentVersion)]
[assembly: AssemblyFileVersion(AbpConsts.CurrentVersion)]
