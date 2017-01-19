using Mood.Util;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mood
{
    public class MvcApplication : HttpApplication
    {
        private bool isUrlConfigDone = false;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Migrations.Configuration.Initialize(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]);
            AppInsightsConfig.Initialize();
        }

        protected void Application_BeginRequest(object source, EventArgs e)
        {
            var initLock = new object();
            if (!isUrlConfigDone)
            {
                lock (initLock)
                {
                    if (!isUrlConfigDone)
                    {
                        var baseUrl = Context.Request.Url.GetLeftPart(UriPartial.Authority);
                        UrlConfig.Url = baseUrl;
                    }
                    isUrlConfigDone = true;
                }
            }
        }
    }
}
