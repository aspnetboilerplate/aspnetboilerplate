using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ExamCenter.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new ScriptBundle("~/Scripts/modernizr")
                    .Include("~/Scripts/modernizr-{version}.js"));

            bundles.Add(
              new ScriptBundle("~/Scripts/baselibs")
                .Include("~/Scripts/jquery-2.0.3.js") //TODO: Use with {version}
                .Include("~/Scripts/knockout-2.3.0.js") //TODO: Use with {version}
              );

            //bundles.Add(
            // new StyleBundle("~/Content/css")
            //    .Include("~/Content/ie10mobile.css") // Must be first. IE10 mobile viewport fix
            //    .Include("~/Content/bootstrap.min.css")
            //    .Include("~/Content/bootstrap-responsive.min.css")
            //    .Include("~/Content/font-awesome.min.css")
            //    .Include("~/Content/styles.css")
            // );
        }

        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
            {
                throw new ArgumentNullException("ignoreList");
            }

            ignoreList.Clear();
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*.debug.js");
            ignoreList.Ignore("*-vsdoc.js");
            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}