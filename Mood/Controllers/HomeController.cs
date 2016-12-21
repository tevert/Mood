using Mood.Migrations;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Mood.Models;
using System.Collections.Generic;

namespace Mood.Controllers
{
    public class HomeController : Controller
    {
        public IApplicationDBContext db;

        public HomeController(IApplicationDBContext db)
        {
            this.db = db;
        }

        public async Task<ActionResult> Index()
        {
            var surveys = new List<Survey>();
            if (User != null)
            {
                surveys = await db.Surveys.Where(s => s.Owner.UserName == User.Identity.Name).ToListAsync();
            }

            return View(surveys);
        }
    }
}
