using Mood.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mood.Controllers.Api
{
    public class AutomationController : ApiController
    {
        private IEmailService emailService;

        public AutomationController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        // POST api/<controller>
        public async Task<dynamic> Post()
        {
            return await emailService.SendEmailAsync("test@example.com", "CONTENT");
        }
    }
}