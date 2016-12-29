using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mood.Models
{
    public class Answer
    {
        public int Id { get; set; }

        public DateTime? Time { get; set; }
        
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        public Guid? SurveyId { get; set; }
        
        [ForeignKey("MoodId")]
        public Mood Mood { get; set; }

        public int? MoodId { get; set; }

        public string Details { get; set; }
    }
}