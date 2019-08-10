using ContestPark.Identity.API.Resources;
using ContestPark.Identity.API.Validations;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Models
{
    public class UpdateUserInfoModel
    {
        //[DataType(DataType.EmailAddress, ErrorMessage = "EmailFormating")]
        //[MaxLength(255, ErrorMessage = "EmailMaxLength")]
        //[Required(ErrorMessage = "EmailRequired")]
        //[NotTurkishCharacter(ErrorMessageResourceType = typeof(IdentityResource), ErrorMessageResourceName = "NotTurkishCharacter")]
        //public string Email { get; set; }

        [MinLength(3, ErrorMessage = "FullNameMinLength")]
        [MaxLength(255, ErrorMessage = "FullNameMaxLength")]
        [Required(ErrorMessage = "FullNameReqired")]
        public string FullName { get; set; }

        [MinLength(3, ErrorMessage = "UserNameMinLength")]
        [MaxLength(255, ErrorMessage = "UserNameMaxLength")]
        [Required(ErrorMessage = "UserNameReqired")]
        [NotTurkishCharacter(ErrorMessageResourceType = typeof(IdentityResource), ErrorMessageResourceName = "NotTurkishCharacter")]
        public string UserName { get; set; }
    }
}
