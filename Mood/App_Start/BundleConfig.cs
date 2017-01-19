using System.Web.Optimization;

namespace Mood
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var jsDepRoot = "~/Scripts/node_modules";
            var cssDepRoot = "~/Content/node_modules";
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                $"{jsDepRoot}/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                $"{jsDepRoot}/knockout/build/output/knockout-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/surveyApp").Include(
                "~/Scripts/surveyApp/mood.viewmodel.js",
                "~/Scripts/surveyApp/surveyApp.viewmodel.js",
                "~/Scripts/surveyApp/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/homeApp").Include(
                "~/Scripts/homeApp/survey.viewmodel.js",
                "~/Scripts/homeApp/homeApp.viewmodel.js",
                "~/Scripts/homeApp/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/util").Include(
                "~/Scripts/util/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                $"{jsDepRoot}/bootstrap/dist/js/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 $"{cssDepRoot}/bootstrap/dist/css/bootstrap.css",
                 "~/Content/Site.css"));

            //bundles.Add(new Bundle("~/bundles/audio").Include(
            //    "~/Content/Sounds/ping.mp3"));
        }
    }
}
