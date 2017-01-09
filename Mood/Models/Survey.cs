using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mood.Models
{
    public class Survey
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        [Required]
        public ApplicationUser Owner { get; set; }

        public string Description { get; set; }

        public bool PublicResults { get; set; }

        public string Identifer { get { return Name ?? Id.ToString(); } }
    }
}