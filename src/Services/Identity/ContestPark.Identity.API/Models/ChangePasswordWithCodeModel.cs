using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Models
{
    public class ChangePasswordWithCodeModel
    {
        [Required]
        public int Code { get; set; }

        [MinLength(6, ErrorMessage = "PasswordMinLength")]
        [MaxLength(32, ErrorMessage = "PasswordMaxLength")]
        [Required(ErrorMessage = "PasswordReqired")]
        public string Password { get; set; }
    }
}