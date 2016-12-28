using Mood.Migrations;
using Mood.Models;
using Mood.Services;
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
        private IDateTimeService time;
        private ISecurity security;

        public SurveyController(IApplicationDBContext db, IDateTimeService time, ISecurity security)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            this.db = db;

            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }

            this.time = time;

            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }
            this.security = security;
        }

        [HttpGet]
        public async Task<ActionResult> View(Guid id)
        {
            var survey = await db.Surveys.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (survey == null)
            {
                return HttpNotFound();
            }

            var moods = await db.Moods.ToListAsync();

            return View(new SurveyViewModel() { Survey = survey, Moods = moods });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(string description = "Default")
        {
            var user = await db.Users.Where(u => u.UserName == security.UserName).FirstAsync();
            var guid = Guid.NewGuid();
            var survey = new Survey() { Description = description, Owner = user, Id = guid };

            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            return RedirectToAction("View", new { id = survey.Id });
        }

        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var survey = await db.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            var answers = await db.Answers.Where(a => a.SurveyId == survey.Id).ToListAsync();
            foreach (var answer in answers)
            {
                db.Answers.Remove(answer);
            }
            db.Surveys.Remove(survey);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPut]
        public async Task<ActionResult> Answer(Guid id, int moodId)
        {
            // First dig up our survey
            var survey = await db.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            // Next check the mood
            var mood = await db.Moods.FirstOrDefaultAsync(m => m.Id == moodId);
            if (mood == null)
            {
                return HttpNotFound();
            }

            // OK, log the hit
            db.Answers.Add(new Answer() { Mood = mood, Survey = survey, Time = time.Now() });
            await db.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}