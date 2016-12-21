using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using Mood.Migrations;
using Mood.Models;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Unity.WebApi;

namespace Mood
{
    public static class UnityConfig
    {
        private static IUnityContainer container;

        public static IUnityContainer RegisterComponents()
        {
            if (container != null)
            {
                throw new InvalidOperationException("Unity has already been configured!");
            }

			container = new UnityContainer();

            container.RegisterType<DbContext, ApplicationDBContext>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<UserManager<ApplicationUser>>();
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(x => HttpContext.Current.GetOwinContext().Authentication));

            DependencyResolver.SetResolver(new Microsoft.Practices.Unity.Mvc.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            return container;
        }

        public static IUnityContainer GetConfiguredContainer()
        {
            return container;
        }
    }
}