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

        public int ForgetPasswordCode { get; set; }

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

        /// <summary>
        /// Takip ettiklerim
        /// </summary>
        public int FollowingCount { get; set; }

        /// <summary>
        /// Takip edenler
        /// </summary>
        public int FollowersCount { get; set; }

        public int GameCount { get; set; }

        /// <summary>
        /// Takip ettiklerim
        /// </summary>
        public string DisplayFollowingCount { get; set; }

        /// <summary>
        /// Takip edenler
        /// </summary>
        public string DisplayFollowersCount { get; set; }

        public string DisplayGameCount { get; set; }
    }
}
