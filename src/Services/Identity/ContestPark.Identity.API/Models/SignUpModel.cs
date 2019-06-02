﻿using ContestPark.Identity.API.Validations;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Models
{
    public class SignUpModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "EmailFormating")]
        [MaxLength(255, ErrorMessage = "EmailMaxLength")]
        [Required(ErrorMessage = "EmailRequired")]
        [NotTurkishCharacter(ErrorMessage = "NotTurkishCharacter")]
        public string Email { get; set; }

        [MinLength(3, ErrorMessage = "FullNameMinLength")]
        [MaxLength(255, ErrorMessage = "FullNameMaxLength")]
        [Required(ErrorMessage = "FullNameReqired")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "LanguageCodeReqired")]
        public string LanguageCode { get; set; }

        [MinLength(6, ErrorMessage = "PasswordMinLength")]
        [MaxLength(32, ErrorMessage = "PasswordMaxLength")]
        [Required(ErrorMessage = "PasswordReqired")]
        public string Password { get; set; }

        [MinLength(3, ErrorMessage = "UserNameMinLength")]
        [MaxLength(255, ErrorMessage = "UserNameMaxLength")]
        [Required(ErrorMessage = "UserNameReqired")]
        [NotTurkishCharacter(ErrorMessage = "NotTurkishCharacter")]
        public string UserName { get; set; }
    }
}