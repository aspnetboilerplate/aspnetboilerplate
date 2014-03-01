using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Abp;

[assembly: AssemblyTitle("ASP.NET Boilerplate")]
[assembly: AssemblyDescription("ASP.NET Boilerplate")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ASP.NET Boilerplate")]
[assembly: AssemblyProduct("Abp")]
[assembly: AssemblyCopyright("Copyright © 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: InternalsVisibleTo("Abp.Application")]
[assembly: InternalsVisibleTo("Abp.Infrastructure.NHibernate")]
[assembly: InternalsVisibleTo("Abp.Web")]
[assembly: InternalsVisibleTo("Abp.Web.Api")]
[assembly: InternalsVisibleTo("Abp.Web.Mvc")]
[assembly: InternalsVisibleTo("Abp.Tests")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7b50eb47-4993-4a14-b65c-b61714a607b9")]

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
[assembly: AssemblyVersion(AbpConst.CurrentVersion)]
[assembly: AssemblyFileVersion("0.1.0.0")]
