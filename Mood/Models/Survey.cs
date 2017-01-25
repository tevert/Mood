using System;
using System.Collections.Generic;
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
        [InverseProperty(nameof(ApplicationUser.OwnedSurveys))]
        public ApplicationUser Owner { get; set; }

        [InverseProperty(nameof(ApplicationUser.SharedSurveys))]
        public virtual ICollection<ApplicationUser> SharedUsers { get; set; }

        public string Description { get; set; }

        public bool PublicResults { get; set; }

        public string Identifier { get { return Name ?? Id.ToString(); } }
    }
}