using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.Spotify
{
    public class SpotifyModel
    {
        public List<ArtistModel> Artists { get; set; }

        public List<AlbumModel> Albums { get; set; } = new List<AlbumModel>();
        public List<TrackModel> Tracks { get; set; } = new List<TrackModel>();
    }
}
