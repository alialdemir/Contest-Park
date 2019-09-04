using ContestPark.Identity.API.Enums;

namespace ContestPark.Identity.API.Data.Tables
{
    public class Picture
    {
        public int PictureId { get; set; }
        public string UserId { get; set; }

        public string PictureUrl { get; set; }
        public PictureTypes PictureType { get; set; }
    }
}
