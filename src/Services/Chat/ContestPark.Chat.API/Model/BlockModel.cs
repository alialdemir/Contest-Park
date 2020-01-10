namespace ContestPark.Chat.API.Model
{
    public class BlockModel
    {
        public string UserName { get; set; }

        public bool IsBlocked { get; set; } = true;
        public string UserId { get; set; }
    }
}
