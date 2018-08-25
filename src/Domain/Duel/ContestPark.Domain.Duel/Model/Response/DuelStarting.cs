using ContestPark.Core.Domain;

namespace ContestPark.Domain.Duel.Model.Response
{
    public class DuelStarting
    {
        public int DuelId { get; set; }

        // Founder
        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath { get; set; } = DefaultImages.DefaultProfilePicture;

        public string FounderCoverPicturePath { get; set; } = DefaultImages.DefaultCoverPicture;

        public string FounderUserId { get; set; }

        // Opponent
        public string OpponentFullName { get; set; }

        public string OpponentProfilePicturePath { get; set; } = DefaultImages.DefaultProfilePicture;

        public string OpponentCoverPicturePath { get; set; } = DefaultImages.DefaultCoverPicture;

        public string OpponentUserId { get; set; }
    }
}