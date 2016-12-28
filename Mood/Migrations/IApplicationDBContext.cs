using Mood.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mood.Migrations
{
    public interface IApplicationDBContext
    {
        IDbSet<Models.Mood> Moods { get; set; }
        IDbSet<Survey> Surveys { get; set; }
        IDbSet<Answer> Answers { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }
        Task<int> SaveChangesAsync();
    }
}
