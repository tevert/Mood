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
    public class AnswerController : Controller
    {
        private IApplicationDBContext db;
        private ISurveyService surveys;
        private IDateTimeService time;
        private ISecurity security;

        public AnswerController(IApplicationDBContext db, ISurveyService surveys, IDateTimeService time, ISecurity security)
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

            if (surveys == null)
            {
                throw new ArgumentNullException(nameof(surveys));
            }
            this.surveys = surveys;
        }

        [HttpPut]
        public async Task<ActionResult> Edit(int id, string details)
        {
            var answer = await db.Answers.FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrWhiteSpace(details))
            {
                return new HttpStatusCodeResult(500, "'details' field is required");
            }

            answer.Details = details;
            await db.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPut]
        public async Task<ActionResult> Create(string surveyId, int moodId, string details)
        {
            // First dig up our survey
            var survey = await surveys.FindAsync(surveyId);
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
            var answer = new Answer() { Mood = mood, Survey = survey, Time = time.Now(), Details = details };
            db.Answers.Add(answer);
            await db.SaveChangesAsync();

            return Json(answer);
        }

        [HttpGet]
        public async Task<ActionResult> Results(string surveyId)
        {
            var survey = await surveys.FindAsync(surveyId);
            if (survey == null)
            {
                return HttpNotFound();
            }

            if (!survey.PublicResults)
            {
                if (!security.IsAuthenticated)
                {
                    // do auth and come back
                    return RedirectToAction("ExternalLogin", "Account", new { returnUrl = HttpContext.Request.Url });
                }

                if (survey.Owner.UserName != security.UserName)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            var answers = await db
                .Answers
                .Include(a => a.Mood)
                .Where(a => a.SurveyId == survey.Id)
                .OrderByDescending(a => a.Time)
                .ToListAsync();

            var moods = await db.Moods.ToListAsync();

            return View(new ResultsViewModel() { Survey = survey, Answers = answers, Moods = moods });
        }
    }
}