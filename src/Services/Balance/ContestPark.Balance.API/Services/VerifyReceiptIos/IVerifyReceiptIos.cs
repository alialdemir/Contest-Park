using ContestPark.Balance.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Services.VerifyReceiptIos
{
    public interface IVerifyReceiptIos
    {
        Task<bool> Veriftasync(string token, ReceiptModel receipt);
    }
}
