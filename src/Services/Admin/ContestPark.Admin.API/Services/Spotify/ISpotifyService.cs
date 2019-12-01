namespace ContestPark.Admin.API.Services.Spotify
{
    public interface ISpotifyService
    {
        System.Threading.Tasks.Task<Model.Spotify.SpotifyModel> GetArtistQuestionAsync(string artistId);
        System.Threading.Tasks.Task<Model.Spotify.SpotifyModel> GetPlaylistQuestions(string playlistId);
    }
}
