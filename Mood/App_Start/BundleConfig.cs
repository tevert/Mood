using System.Web.Optimization;

namespace Mood
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var jsDepRoot = "~/Scripts/vendor";
            var cssDepRoot = "~/Content/vendor";
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                $"{jsDepRoot}/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                $"{jsDepRoot}/knockout/knockout-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
                $"{jsDepRoot}/moment/moment.min.js",
                $"{jsDepRoot}/chart.js/Chart.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                $"{jsDepRoot}/moment/moment.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/resultsApp").Include(
                "~/Scripts/resultsApp/answer.viewmodel.js",
                "~/Scripts/resultsApp/resultsApp.viewmodel.js",
                "~/Scripts/resultsApp/_run.js"));

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
                $"{jsDepRoot}/bootstrap/bootstrap.js",
                $"{jsDepRoot}/bootstrap-tagsinput/bootstrap-tagsinput.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 $"{cssDepRoot}/bootstrap.css",
                 $"{cssDepRoot}/bootstrap-tagsinput.css",
                 "~/Content/Site.css"));
        }
    }
}
