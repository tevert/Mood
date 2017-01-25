using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Mood.Models
{
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty(nameof(Survey.Owner))]
        public virtual ICollection<Survey> OwnedSurveys { get; set; }

        [InverseProperty(nameof(Survey.SharedUsers))]
        public virtual ICollection<Survey> SharedSurveys { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}