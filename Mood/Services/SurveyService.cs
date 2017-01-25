using System;
using System.Threading.Tasks;
using Mood.Models;
using Mood.ViewModels;
using Mood.Migrations;
using System.Data.Entity;
using Mood.Services.Exceptions;
using System.Linq;
using System.Collections.Generic;

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
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

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
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }
            if (newValues == null)
            {
                throw new ArgumentNullException(nameof(newValues));
            }

            // Handle the co-admins
            var unknownUsers = newValues.SharedUsers.Except(await db.Users.Select(u => u.UserName).ToListAsync());
            if (unknownUsers.Any())
            {
                throw new SurveyException($"Unknown users: {unknownUsers.Aggregate((s1,s2) => s1 + ", " + s2)}. Please make sure these users have signed into Moodboard before.");
            }
            if (newValues.SharedUsers.Contains(survey.Owner.UserName))
            {
                throw new SurveyException("You cannot be a co-admin on a survey you own.");
            }
            survey.SharedUsers = await db.Users.Where(u => newValues.SharedUsers.Contains(u.UserName)).ToListAsync();

            // Handle the name
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
                result = await db.Surveys
                    .Include(s => s.Owner)
                    .Include(s => s.SharedUsers)
                    .FirstOrDefaultAsync(s => s.Id == guid);
            }
            else
            {
                result = await db.Surveys
                    .Include(s => s.Owner)
                    .Include(s => s.SharedUsers)
                    .FirstOrDefaultAsync(s => s.Name == identifier);
            }

            return result;
        }
    }
}