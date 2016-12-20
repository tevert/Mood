using System.Data.Entity;

namespace Mood.Migrations
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        public IDbSet<Models.Mood> Moods { get; set; }

        public ApplicationDBContext() : base("DefaultConnection")
        {
        }
    }
}