namespace SULS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Problem
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(20)]
        public string Name { get; set; }

        [Range(50, 300)]
        public int Points { get; set; }

        // FIXME maybe ?!? 
        public ICollection<Submission> Submissions { get; set; }
    }
}
