﻿namespace SULS.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();        

    }
}