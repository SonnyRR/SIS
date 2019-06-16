using Panda.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Panda.Models
{
    public class Package
    {
        public Package()
        {
            this.PackageStatus = PackageStatus.Pending;
        }

        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(20)]
        public string Description { get; set; }

        public decimal Weight { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public PackageStatus PackageStatus { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        [Required]
        public string RecipientId { get; set; }
        public User Recipient { get; set; }


    }
}
