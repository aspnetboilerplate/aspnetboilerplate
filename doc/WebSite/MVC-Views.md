### Introduction

ASP.NET Boilerplate is integrated in to MVC Views via the **Abp.Web.Mvc** NuGet
package. You can create regular MVC Views as you always do.

### AbpWebViewPage Base Class

ASP.NET Boilerplate also provides the **AbpWebViewPage** base class, which defines some
useful properties and methods. If you created your project using the
[startup templates](/Templates) then all your views are automatically
inherited from this base class.

AbpWebViewPage defines an **L** method for
[localization](/Pages/Documents/Localization), **IsGranted** method for
[authorization](/Pages/Documents/Authorization), **IsFeatureEnabled**
and **GetFeatureValue** methods for [feature
management](/Pages/Documents/Feature-Management) and more.
