using Microsoft.Practices.Unity;
using Mood.Migrations;
using System;
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

            container.RegisterType<IApplicationDBContext, ApplicationDBContext>();

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