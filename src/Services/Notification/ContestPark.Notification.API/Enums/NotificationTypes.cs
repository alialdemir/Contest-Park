namespace ContestPark.Notification.API.Enums
{
    public enum NotificationTypes : byte
    {
        Follow = 1,
        PostLike = 2,
        PostComment = 3,
        // Contest = 1,//Seninle {yarisma} yarışmasında düello yaptı. Ona karşı koy!
        //  ContestResist = 6,// {yarisma} yarışmasında düellona karşı koydu!
        //      ContestTakeABeating = 7, // {yarisma} yarışmasında yenilmeyi seçti.
    }
}
