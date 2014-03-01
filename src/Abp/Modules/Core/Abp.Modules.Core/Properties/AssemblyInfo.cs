using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Abp;

[assembly: AssemblyTitle("ASP.NET Boilerplate - Core Module")]
[assembly: AssemblyDescription("ASP.NET Boilerplate - Core Module")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ASP.NET Boilerplate")]
[assembly: AssemblyProduct("Abp.Modules.Core")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: InternalsVisibleTo("Abp.Modules.Core.Application")]
[assembly: InternalsVisibleTo("Abp.Modules.Core.Infrastructure.NHibernate")]
[assembly: InternalsVisibleTo("Abp.Modules.Core.Web")]
[assembly: InternalsVisibleTo("Abp.Modules.Core.Web.Api")]
[assembly: InternalsVisibleTo("Abp.Modules.Core.Web.Mvc")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a73e9bd0-393c-4450-a6b0-42ec1a8bfb90")]

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
[assembly: AssemblyVersion(AbpCoreModuleConst.CurrentVersion)]
[assembly: AssemblyFileVersion(AbpCoreModuleConst.CurrentVersion)]
