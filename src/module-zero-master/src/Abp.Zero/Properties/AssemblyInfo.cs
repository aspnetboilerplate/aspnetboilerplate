using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Abp.Zero;

[assembly: AssemblyTitle("ASP.NET Boilerplate - Iteration Zero")]
[assembly: AssemblyDescription("ASP.NET Boilerplate - Iteration Zero")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ASP.NET Boilerplate")]
[assembly: AssemblyProduct("Abp.Zero")]
[assembly: AssemblyCopyright("Copyright © 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: InternalsVisibleTo("Abp.Zero.NHibernate")]
[assembly: InternalsVisibleTo("Abp.Zero.EntityFramework")]
[assembly: InternalsVisibleTo("Abp.Zero.Application")]
[assembly: InternalsVisibleTo("Abp.Zero.Web.Api")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("436fea78-a954-4902-8874-530de9e48d61")]

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
[assembly: AssemblyVersion(AbpZeroCoreModule.CurrentVersion)]
[assembly: AssemblyFileVersion(AbpZeroCoreModule.CurrentVersion)]
