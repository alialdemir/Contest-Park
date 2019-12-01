namespace ContestPark.Admin.API.Model.Spotify
{
    public class AlbumModel
    {
        public string AlbumName { get; set; }
        public string AlbumImage { get; set; }
        public string AlbumId { get; set; }
        public int ReleaseYear { get; set; }
        public string ArtistName { get; internal set; }
    }
}
