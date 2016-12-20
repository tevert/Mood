using System.Data.Entity;
using System.Threading.Tasks;

namespace Mood.Migrations
{
    public interface IApplicationDBContext
    {
        IDbSet<Models.Mood> Moods { get; set; }

        Task<int> SaveChangesAsync();
    }
}
