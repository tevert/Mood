using Microsoft.AspNet.Identity.EntityFramework;
using Mood.Models;
using System.Data.Entity;

namespace Mood.Migrations
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>, IApplicationDBContext
    {
        public IDbSet<Models.Mood> Moods { get; set; }

        public IDbSet<Survey> Surveys { get; set; }

        public IDbSet<Answer> Answers { get; set; }

        public ApplicationDBContext() : base("DefaultConnection")
        {
        }
    }
}