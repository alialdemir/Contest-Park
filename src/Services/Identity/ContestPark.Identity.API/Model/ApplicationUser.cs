using ContestPark.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Model
{
    public class ApplicationUser : IdentityUser//, IEntity
    {
        public string FaceBookId { get; set; }
        public string ProfilePicturePath { get; set; }
        public string CoverPicturePath { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public string LanguageCode { get; set; }

        public Languages Language { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}