namespace Panda.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(5), MaxLength(20)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Package> Packages { get; set; } = new HashSet<Package>();
        public ICollection<Receipt> Receipts { get; set; } = new HashSet<Receipt>();
    }
}
