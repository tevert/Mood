using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mood.Models
{
    public class Survey
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public ApplicationUser Owner { get; set; }

        public string Description { get; set; }
    }
}