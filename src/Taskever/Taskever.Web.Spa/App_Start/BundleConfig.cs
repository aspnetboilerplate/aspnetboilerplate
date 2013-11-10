using System;
using System.Web.Optimization;

namespace Taskever.Web.App_Start
{
    /// <summary>
    /// TODO: Optimization and minifying best practices!
    /// </summary>
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new StyleBundle("~/styles/basestyles")
                    .Include("~/Abp/Framework/styles/utils/ie10fix.css")
                    .Include("~/Content/bootstrap/bootstrap.metro.min.css") //TODO: Currently testing metro bootstrap
                    //.Include("~/Content/bootstrap/bootstrap-theme.css")
                    .Include("~/Content/font-awesome.min.css")
                    .Include("~/Content/durandal.css")
                    .Include("~/Scripts/dropzone/css/dropzone.css")
                    .Include("~/Content/toastr.min.css")
                    .Include("~/App/_Common/styles/main.css")
                    .Include("~/Abp/Framework/styles/abp.css")
                );

            bundles.Add(
                new StyleBundle("~/styles/taskever")
                    .Include("~/App/Main/styles/briefFriendList.min.css")
                );

            bundles.Add(
                new ScriptBundle("~/Scripts/modernizr")
                    .Include("~/Scripts/modernizr-{version}.js") //TODO: Use minimized
                );

            bundles.Add(
                new ScriptBundle("~/Scripts/baselibs")
                    .Include("~/Scripts/json2.min.js")
                    .Include("~/Scripts/jquery-2.0.3.js") //TODO: Use with {version} and minimized
                    .Include("~/Scripts/knockout-2.3.0.js") //TODO: Use with {version} and minimized
                    .Include("~/Scripts/knockout.mapping-latest.js")
                    .Include("~/Scripts/bootstrap.js") //TODO: Use minimized
                    .Include("~/Scripts/jquery.validate.min.js")
                    .Include("~/Scripts/jquery.form.min.js")
                    .Include("~/Scripts/jquery.blockUI.min.js")
                    .Include("~/Scripts/moment-with-langs.min.js")
                    .Include("~/Scripts/toastr.min.js")
                );

            bundles.Add(
                new ScriptBundle("~/Scripts/abp")
                    .Include("~/abp/framework/scripts/abp.js")
                    .Include("~/abp/framework/scripts/abp.jquery.js")
                    .Include("~/abp/framework/scripts/abp.toastr.js")
                    .Include("~/abp/framework/scripts/abp.blockUI.js")
                    .Include("~/abp/framework/scripts/abp.spin.js")
                    .Include("~/abp/framework/scripts/abp.localization.js")
                );
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
        }
    }
}