using System;
using System.Web.Optimization;

namespace ExamCenter.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new ScriptBundle("~/scripts/modernizr")
                    .Include("~/scripts/modernizr-{version}.js"));

            bundles.Add(
              new ScriptBundle("~/scripts/base")
                .Include("~/scripts/jquery-2.0.3.min.js") //TODO: Change to {version}
                .Include("~/scripts/bootstrap.min.js")
              );

            bundles.Add(
             new StyleBundle("~/styles/base")
                .Include("~/Content/ie10mobile.css") // Must be first. IE10 mobile viewport fix
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/bootstrap-responsive.min.css")
                .Include("~/Content/font-awesome.min.css")
             );
        }

        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            ignoreList.Clear();
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}