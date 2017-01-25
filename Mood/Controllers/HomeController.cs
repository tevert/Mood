using Mood.Migrations;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using Mood.Services;
using Mood.ViewModels;

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

            var me = db.Users
                .Include(u => u.SharedSurveys)
                .Include(u => u.OwnedSurveys)
                .First(u => u.UserName == security.UserName);

            return View(new HomeViewModel() { MySurveys = me.OwnedSurveys, SharedSurveys = me.SharedSurveys });
        }
    }
}
