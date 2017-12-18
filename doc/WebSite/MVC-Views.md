### Introduction

ASP.NET Boilerplate is integrated to MVC Views via **Abp.Web.Mvc** nuget
package. You can create regular MVC Views as you always do.

### AbpWebViewPage Base Class

ASP.NET Boilerplate also provides **AbpWebViewPage**, which defines some
useful properties and methods. If you created your project using
[startup templates](/Templates) then all your views are automatically
inherited from this base class.

AbpWebViewPage defines **L** method for
[localization](/Pages/Documents/Localization), **IsGranted** method for
[authorization](/Pages/Documents/Authorization), **IsFeatureEnabled**
and **GetFeatureValue** methods for [feature
management](/Pages/Documents/Feature-Management) and so on.
