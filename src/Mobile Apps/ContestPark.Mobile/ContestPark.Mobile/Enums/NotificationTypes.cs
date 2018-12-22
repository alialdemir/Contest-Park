namespace ContestPark.Mobile.Enums
{
    public enum NotificationTypes : byte
    {
        Contest = 1,//Seninle {yarisma} yarışmasında düello yaptı. Ona karşı koy!
        Follow = 2,
        LinkLike = 3,
        Comment = 4,
        ContestResist = 6,// {yarisma} yarışmasında düellona karşı koydu!
        ContestTakeABeating = 7, // {yarisma} yarışmasında yenilmeyi seçti.
    }
}