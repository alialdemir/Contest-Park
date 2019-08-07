using ContestPark.Mobile.Models.ErrorModel;
using System.Net;

namespace ContestPark.Mobile.Models.RequestProvider
{
    public class ResponseModel<TResult>
    {
        public TResult Data { get; set; }

        public ValidationResultModel Error { get; set; }

        public bool IsSuccess { get; set; } = false;

        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.BadRequest;
    }
}
