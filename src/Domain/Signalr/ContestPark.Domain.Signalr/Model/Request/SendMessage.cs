namespace ContestPark.Domain.Signalr.Model.Request
{
    public class SendMessage
    {
        public string MethodName { get; set; }

        public string ConnectionId { get; set; }

        public object Param { get; set; }
    }
}