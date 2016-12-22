using Mood.Models;
using System.Collections.Generic;

namespace Mood.ViewModels
{
    public class SurveyViewModel
    { 
        public Survey Survey { get; set; }

        public IEnumerable<Models.Mood> Moods { get; set; }
    }
}