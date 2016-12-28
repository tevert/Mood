using Mood.Migrations;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Mood.Models;
using System.Collections.Generic;
using System;
using Mood.Services;

namespace Mood.Controllers
{
    public class HomeController : Controller
    {
        public IApplicationDBContext db;
        private ISecurity security;

        public HomeController(IApplicationDBContext db, ISecurity security)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            this.db = db;

            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            this.security = security;
        }

        public async Task<ActionResult> Index()
        {
            if (!security.IsAuthenticated)
            {
                return View("IndexNotAuthenticated");
            }
            
            var surveys = await db.Surveys.Where(s => s.Owner.UserName == security.UserName).ToListAsync();
            return View(surveys);
        }
    }
}
