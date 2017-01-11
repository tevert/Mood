using Mood.Models;
using Mood.ViewModels;
using System.Threading.Tasks;

namespace Mood.Services
{
    public interface ISurveyService
    {
        Task<Survey> FindAsync(string identifier);

        Task AddAsync(Survey newSurvey);

        Task EditAsync(Survey survey, SurveyEditViewModel newValues);

        Task DeleteAsync(Survey survey);
    }
}
