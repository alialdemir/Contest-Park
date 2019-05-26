using ContestPark.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Models
{
    public class ApplicationUser : IdentityUser//, IEntity
    {
        public string CoverPicturePath { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string FaceBookId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public bool IsBot { get; set; } = false;
        public Languages Language { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string ProfilePicturePath { get; set; }
    }
}