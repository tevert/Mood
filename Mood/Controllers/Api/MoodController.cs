using Mood.Migrations;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mood.Controllers.Api
{
    public class MoodsController : ApiController
    {
        private IApplicationDBContext db;

        public MoodsController(IApplicationDBContext db)
        {
            this.db = db;
        }

        // POST: Mood
        [HttpPost]
        public async Task<IHttpActionResult> Post(int id)
        {
            var mood = await db.Moods.FirstOrDefaultAsync(m => m.Id == id);
            if (mood == null)
            {
                return NotFound();
            }

            mood.Count++;

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}