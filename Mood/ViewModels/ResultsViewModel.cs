using Mood.Models;
using System.Collections.Generic;

namespace Mood.ViewModels
{
    public class ResultsViewModel
    {
        public Survey Survey { get; set; }
        public IEnumerable<Answer> Answers { get; set; }

        public IEnumerable<Models.Mood> Moods { get; set; }
    }
}