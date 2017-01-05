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
        public async Task<ActionResult> Get(string id)
        {
            var survey = await FindSurveyAsync(id);
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

            return RedirectToAction("Get", new { id = survey.Identifer });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Name(string id, string newName)
        {
            var survey = await FindSurveyAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (survey.Owner.UserName != security.UserName)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            newName = newName.Trim();
            if (String.IsNullOrWhiteSpace(newName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var blockingSurvey = await FindSurveyAsync(newName);
            if (blockingSurvey != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            survey.Name = newName;
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Results(string id)
        {
            var survey = await FindSurveyAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (survey.Owner.UserName != security.UserName)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var answers = await db
                .Answers
                .Include(a => a.Mood)
                .Where(a => a.SurveyId == survey.Id)
                .OrderByDescending(a => a.Time)
                .ToListAsync();

            return View(new ResultsViewModel() { Survey = survey, Answers = answers });
        }

        [Authorize]
        public async Task<ActionResult> Delete(string id)
        {
            var survey = await FindSurveyAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (survey.Owner.UserName != security.UserName)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
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
        public async Task<ActionResult> Answer(string id, int moodId)
        {
            // First dig up our survey
            var survey = await FindSurveyAsync(id);
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
            var answer = new Answer() { Mood = mood, Survey = survey, Time = time.Now() };
            db.Answers.Add(answer);
            await db.SaveChangesAsync();

            return Json(answer);
        }

        private async Task<Survey> FindSurveyAsync(string id)
        {
            Survey result = null;
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                result = await db.Surveys.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == guid);
            }
            else
            {
                result = await db.Surveys.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Name == id);
            }

            return result;
        }
    }
}