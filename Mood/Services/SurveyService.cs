using System;
using System.Threading.Tasks;
using Mood.Models;
using Mood.ViewModels;
using Mood.Migrations;
using System.Data.Entity;
using Mood.Services.Exceptions;
using System.Linq;

namespace Mood.Services
{
    public class SurveyService : ISurveyService
    {
        private IApplicationDBContext db;

        public SurveyService(IApplicationDBContext db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            this.db = db;
        }

        public async Task AddAsync(Survey newSurvey)
        {
            if (newSurvey == null)
            {
                throw new ArgumentNullException(nameof(newSurvey));
            }

            db.Surveys.Add(newSurvey);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Survey survey)
        {
            var answers = await db.Answers.Where(a => a.SurveyId == survey.Id).ToListAsync();
            foreach (var answer in answers)
            {
                db.Answers.Remove(answer);
            }

            db.Surveys.Remove(survey);
            await db.SaveChangesAsync();
        }

        public async Task EditAsync(Survey survey, SurveyEditViewModel newValues)
        {
            string newName = null;
            if (newValues.Name != null)
            {
                newName = newValues.Name.Trim();
                if (String.IsNullOrWhiteSpace(newName))
                {
                    throw new SurveyException("Name must not be whitespace");
                }

                var blockingSurvey = await FindAsync(newName);
                if (blockingSurvey != null && blockingSurvey != survey)
                {
                    throw new SurveyException("That name is already taken");
                }
            }
            survey.Name = newName;
            survey.PublicResults = newValues.PublicResults;

            await db.SaveChangesAsync();
        }

        public async Task<Survey> FindAsync(string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            Survey result = null;
            Guid guid;
            if (Guid.TryParse(identifier, out guid))
            {
                result = await db.Surveys.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == guid);
            }
            else
            {
                result = await db.Surveys.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Name == identifier);
            }

            return result;
        }
    }
}