using System.Collections.Generic;

namespace Mood.ViewModels
{
    public class SurveyEditViewModel
    {
        public SurveyEditViewModel()
        {
            SharedUsers = new List<string>(); // so empty arrays don't produce NPEs
        }

        public string Name { get; set; }

        public bool PublicResults { get; set; }

        public IEnumerable<string> SharedUsers { get; set; }
    }
}