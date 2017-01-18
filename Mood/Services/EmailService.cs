using Mood.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mood.Services
{
    public class EmailService : IEmailService
    {
        private string apiKey;
        private string systemEmail;

        public EmailService(string apiKey, string systemEmail)
        {
            this.apiKey = apiKey;
            this.systemEmail = systemEmail;
        }

        public async Task<dynamic> SendEmailAsync(string recipient, string subjectLine, string content)
        {
            dynamic sg = new SendGridAPIClient(apiKey); // SendGrid's client is a giant tree of dynamics, wtf

            Email from = new Email(systemEmail);
            string subject = subjectLine;
            Email to = new Email(recipient);
            Content contentBlock = new Content("text/html", content);
            Mail mail = new Mail(from, subject, to, contentBlock);

            return await sg.client.mail.send.post(requestBody: mail.Get());
        }
    }
}