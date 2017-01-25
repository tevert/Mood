using Mood.Models;
using System.Collections.Generic;

namespace Mood.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Survey> MySurveys { get; set; }
        public IEnumerable<Survey> SharedSurveys { get; set; }
    }
}