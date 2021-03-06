﻿using Mood.Migrations;
using Mood.Models;
using Mood.Services;
using Mood.Util;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Mood.Controllers.Api
{
    public class AutomationController : ApiController
    {
        private IEmailService emailService;
        private IApplicationDBContext db;
        private IDateTimeService time;

        public AutomationController(IEmailService emailService, IApplicationDBContext db, IDateTimeService time)
        {
            this.emailService = emailService;

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
        }

        // POST api/<controller>
        public async Task Post()
        {
            var owners = db.Users.ToList();
            foreach (var owner in owners)
            {
                await SendEmail(owner);
            }
        }

        private async Task SendEmail(ApplicationUser owner)
        {
            var surveys = db.Surveys.Where(s => s.Owner.Id == owner.Id).ToList();
            if (!surveys.Any())
            {
                return;
            }

            var previousDate = time.Now().AddDays(-1);
            var subject = $"Mood Daily Summary for {previousDate.ToShortDateString()}";
            var content = $"<html><head></head><body><h1>Mood summary for {previousDate.ToShortDateString()}</h1>";
            
            foreach (var survey in surveys)
            {
                var comments = new List<string>();
                var dailyAnswers = new List<Answer>();
                var averageTotal = 0;

                var potentialAnswers = db.Answers.ToList();
                foreach (var answer in potentialAnswers)
                {
                    if (answer.SurveyId == survey.Id && answer.Time.Value.Date == previousDate.Date)
                    {
                        dailyAnswers.Add(answer);
                    }
                }

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                var url = urlHelper.Action("Results", "Answer", new { id = survey.Id }, urlHelper.RequestContext.HttpContext.Request.Url.Scheme);
                content += $"<h2>{survey.Description} - <a href=\"{url}\">{url}</a></h2><h3>Average Rating</h3>";
                foreach (var answer in dailyAnswers)
                {
                    averageTotal += answer.MoodId.Value;
                    if (answer.Details != null && answer.Details != "")
                    {
                        comments.Add(answer.Details);
                    }
                }
                if (dailyAnswers.Count != 0)
                {
                    var average = (decimal)averageTotal / (decimal)dailyAnswers.Count;
                    content += $"<p>{Math.Round(average, 2)}</p>";
                }
                else
                {
                    content += "<p>No responses recorded today</p>";
                }
                content += "<h3>Comments</h3>";
                foreach (var comment in comments)
                {
                    content += $"<p>{comment}</p>";
                }
            }
            content += "</body></html>";
            await emailService.SendEmailAsync(owner.Email, subject, content);
        }
    }
}
