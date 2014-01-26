using System;
using System.Web.Optimization;

namespace Taskever.Web.Mvc
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new StyleBundle("~/styles/basestyles")
                    .Include("~/Abp/Framework/styles/utils/ie10fix.css")
                    .Include("~/Content/bootstrap.metro.min.css", new CssRewriteUrlTransform()) //TODO: Currently testing metro bootstrap
                //.Include("~/Content/bootstrap/bootstrap-theme.css")
                    .Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/durandal.css", new CssRewriteUrlTransform())
                    .Include("~/Content/toastr.min.css", new CssRewriteUrlTransform())
                    .Include("~/App/_Common/styles/main.css", new CssRewriteUrlTransform())
                    .Include("~/Abp/Framework/styles/abp.css", new CssRewriteUrlTransform())
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
                    .Include("~/Scripts/jquery-2.1.0.min.js") //TODO: Use with {version} and minimized
                    .Include("~/Scripts/underscore.min.js") //TODO: Use with {version} and minimized
                    .Include("~/Scripts/knockout-3.0.0.js") //TODO: Use with {version} and minimized
                    .Include("~/Scripts/knockout.mapping-latest.js")
                    .Include("~/Scripts/bootstrap.min.js")
                    .Include("~/Scripts/jquery.validate.min.js")
                    .Include("~/Scripts/jquery.form.min.js")
                    .Include("~/Scripts/jquery.blockUI.min.js")
                    .Include("~/Scripts/moment-with-langs.min.js")
                    .Include("~/Scripts/libs/livestamp/livestamp.min.js")
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

            bundles.Add(
                new ScriptBundle("~/Scripts/taskever")
                    .Include("~/App/Main/scripts/taskever.js")
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
