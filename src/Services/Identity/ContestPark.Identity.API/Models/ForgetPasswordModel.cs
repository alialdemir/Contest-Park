using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Models
{
    public class ForgetPasswordModel
    {
        public string UserNameOrEmail { get; set; }
    }
}