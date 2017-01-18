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
        bool isMapperInit;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Migrations.Configuration.Initialize(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]);
            isMapperInit = false;
        }

        protected void Application_BeginRequest(object source, EventArgs e)
        {
            var initLock = new object();
            if (!isMapperInit)
            {
                lock (initLock)
                {
                    if (!isMapperInit)
                    {
                        var baseUrl = Context.Request.Url.GetLeftPart(UriPartial.Authority);
                        UrlConfig.Url = baseUrl;
                    }
                    isMapperInit = true;
                }
            }
        }
    }
}
