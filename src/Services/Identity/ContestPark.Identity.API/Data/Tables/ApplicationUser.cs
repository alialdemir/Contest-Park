using ContestPark.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Data.Tables
{
    public class ApplicationUser : IdentityUser//, IEntity
    {
        public string CoverPicturePath { get; set; }

        public DateTime CreatedDate { get; set; }

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

        public DateTime? ModifiedDate { get; set; }

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

        public bool IsPrivateProfile { get; set; }
    }
}
