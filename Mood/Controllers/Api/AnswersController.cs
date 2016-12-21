using Mood.Migrations;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using Mood.Models;
using System;

namespace Mood.Controllers.Api
{
    public class AnswersController : ApiController
    {
        private IApplicationDBContext db;

        public AnswersController(IApplicationDBContext db)
        {
            this.db = db;
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Answer answer)
        {
            // First dig up our survey
            var survey = await db.Surveys.FirstOrDefaultAsync(s => s.Id == answer.SurveyId);
            if (survey == null)
            {
                return NotFound();
            }

            // Next check the mood
            var mood = await db.Moods.FirstOrDefaultAsync(m => m.Id == answer.MoodId);
            if (mood == null)
            {
                return NotFound();
            }

            // OK, log the hit
            db.Answers.Add(new Answer() { Mood = mood, Survey = survey, Time = DateTime.Now });
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}