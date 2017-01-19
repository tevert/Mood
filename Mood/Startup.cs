using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Web.Helpers;
using Mood.Migrations;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System;
using Mood.Models;
using ApplicationInsights.OwinExtensions;

[assembly: OwinStartup(typeof(Mood.Startup))]

namespace Mood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseApplicationInsights();

            // Configure the db context, user manager and signin manager to use a single instance per request
            var di = UnityConfig.GetConfiguredContainer();
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationDBContext>());
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "925301553938-vghfj8ghjudcuoj82is06cd8dcscqao9.apps.googleusercontent.com",
                ClientSecret = "xVFEP82UdJNOHmhEfY2n9ZKk"
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
