namespace ContestPark.Domain.Signalr.Model.Request
{
    public class DuelStart
    {
        public int DuelId { get; set; }

        // Founder
        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath { get; set; }

        public string FounderCoverPicturePath { get; set; }

        public string FounderConnectionId { get; set; }

        // Opponent
        public string OpponentFullName { get; set; }

        public string OpponentProfilePicturePath { get; set; }

        public string OpponentCoverPicturePath { get; set; }

        public string OpponentConnectionId { get; set; }

        public DuelStart(int duelId,
            string founderFullName,
            string founderProfilePicturePath,
            string founderCoverPicturePath,
            string founderConnectionId,

            string opponentFullName,
            string opponentProfilePicturePath,
            string opponentCoverPicturePath,
            string opponentConnectionId)
        {
            DuelId = duelId;

            FounderFullName = founderFullName;
            FounderProfilePicturePath = founderProfilePicturePath;
            FounderCoverPicturePath = founderCoverPicturePath;
            FounderConnectionId = founderConnectionId;

            OpponentFullName = opponentFullName;
            OpponentProfilePicturePath = opponentProfilePicturePath;
            OpponentCoverPicturePath = opponentCoverPicturePath;
            OpponentConnectionId = opponentConnectionId;
        }
    }
}