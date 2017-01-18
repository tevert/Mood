using System.Web.Optimization;

namespace Mood
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var depRoot = "~/node_modules";
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                $"{depRoot}/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                $"{depRoot}/knockout/build/output/knockout-latest.js"));

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
                $"{depRoot}/bootstrap/dist/js/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 $"{depRoot}/bootstrap/dist/css/bootstrap.css",
                 "~/Content/Site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
