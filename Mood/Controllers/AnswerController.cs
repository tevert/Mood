using Mood.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mood.Controllers
{
    public class AnswerController : Controller
    {
        private IApplicationDBContext db;

        public AnswerController(IApplicationDBContext db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            this.db = db;
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
    }
}