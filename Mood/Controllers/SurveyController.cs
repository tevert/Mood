using Mood.Migrations;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mood.Controllers
{
    public class SurveyController : Controller
    {
        private IApplicationDBContext db;

        public SurveyController(IApplicationDBContext db)
        {
            this.db = db;
        }

        public async Task<ActionResult> View(Guid id)
        {
            var survey = await db.Surveys.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (survey == null)
            {
                return HttpNotFound();
            }

            return View(survey);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }
    }
}