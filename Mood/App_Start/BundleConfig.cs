using System.Web.Optimization;

namespace Mood
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/dep/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/surveyApp").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/surveyApp/mood.viewmodel.js",
                "~/Scripts/surveyApp/surveyApp.viewmodel.js",
                "~/Scripts/surveyApp/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/homeApp").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/homeApp/survey.viewmodel.js",
                "~/Scripts/homeApp/homeApp.viewmodel.js",
                "~/Scripts/homeApp/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/util").Include(
                "~/Scripts/util/*.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/Site.css"));
        }
    }
}
