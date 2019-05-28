using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
    }
}