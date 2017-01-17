using System.Threading.Tasks;

namespace Mood.Services
{
    public interface IEmailService
    {
        Task<dynamic> SendEmailAsync(string recipient, string content);
    }
}
