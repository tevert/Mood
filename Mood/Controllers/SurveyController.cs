using Mood.Migrations;
using Mood.Models;
using Mood.Services;
using Mood.Services.Exceptions;
using Mood.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mood.Controllers
{
    public class SurveyController : Controller
    {
        private IApplicationDBContext db;
        private ISecurity security;
        private ISurveyService surveys;

        public SurveyController(IApplicationDBContext db, ISurveyService surveys, ISecurity security)
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

            if (surveys == null)
            {
                throw new ArgumentNullException(nameof(surveys));
            }
            this.surveys = surveys;
        }

        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            var survey = await surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            var moods = await db.Moods.ToListAsync();

            return View(new SurveyViewModel() { Survey = survey, Moods = moods });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            var user = await db.Users.Where(u => u.UserName == security.UserName).FirstAsync();
            var guid = Guid.NewGuid();
            var survey = new Survey() { Description = description, Owner = user, Id = guid };

            await surveys.AddAsync(survey);

            return RedirectToRoute("SurveyRoute", new { id = survey.Identifier });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, [System.Web.Http.FromBody] SurveyEditViewModel editDetails)
        {
            var survey = await surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (survey.Owner.UserName != security.UserName)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                await surveys.EditAsync(survey, editDetails);
            }
            catch (SurveyException e)
            {
                return Json(new { error = e.Message });
            }

            return Json(new { success = "saved" });
        }

        [Authorize]
        public async Task<ActionResult> Delete(string id)
        {
            var survey = await surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (survey.Owner.UserName != security.UserName)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            await surveys.DeleteAsync(survey);

            return RedirectToAction("Index", "Home");
        }
    }
}