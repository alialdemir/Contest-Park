using ContestPark.Core.Enums;
using System;

namespace ContestPark.Domain.Signalr.Model.Request
{
    public class DuelStartingScreen
    {
        public int DuelId { get; set; }
        public Int16 SubCategoryId { get; set; }

        // Founder
        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath { get; set; }

        public string FounderCoverPicturePath { get; set; }

        public string FounderConnectionId { get; set; }

        public string FounderUserId { get; set; }

        // Opponent
        public string OpponentFullName { get; set; }

        public string OpponentProfilePicturePath { get; set; }

        public string OpponentCoverPicturePath { get; set; }

        public string OpponentConnectionId { get; set; }

        public string OpponentUserId { get; set; }

        public Languages FounderLanguage { get; set; }

        public Languages OpponentLanguage { get; set; }

        public DuelStartingScreen(int duelId,
            Int16 subCategoryId,
            string founderFullName,
            string founderProfilePicturePath,
            string founderCoverPicturePath,
            string founderConnectionId,
            string founderUserId,
            Languages founderLanguage,

            string opponentFullName,
            string opponentProfilePicturePath,
            string opponentCoverPicturePath,
            string opponentConnectionId,
            string opponentUserId,
            Languages opponentLanguage)
        {
            DuelId = duelId;
            SubCategoryId = subCategoryId;

            FounderFullName = founderFullName;
            FounderProfilePicturePath = founderProfilePicturePath;
            FounderCoverPicturePath = founderCoverPicturePath;
            FounderConnectionId = founderConnectionId;
            FounderUserId = founderUserId;
            FounderLanguage = founderLanguage;

            OpponentFullName = opponentFullName;
            OpponentProfilePicturePath = opponentProfilePicturePath;
            OpponentCoverPicturePath = opponentCoverPicturePath;
            OpponentConnectionId = opponentConnectionId;
            OpponentUserId = opponentUserId;
            OpponentLanguage = opponentLanguage;
        }
    }
}