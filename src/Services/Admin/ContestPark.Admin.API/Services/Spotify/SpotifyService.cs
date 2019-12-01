using ContestPark.Admin.API.Model.Spotify;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        #region Private Variables

        private readonly SpotifyWebAPI _spotifyWebAPI;
        private readonly ILogger<SpotifyService> _logger;

        #endregion Private Variables

        #region Constructor

        public SpotifyService(IOptions<AdminSettings> adminSettings,
                              ILogger<SpotifyService> logger)
        {
            Token token = GetAccessToken(adminSettings.Value.SpotifyClientId, adminSettings.Value.SpotifySecretId);

            _spotifyWebAPI = new SpotifyWebAPI
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };
            _logger = logger;
        }

        #endregion Constructor

        #region Questions

        private List<MusicQuestionModel> ListenQuestions
        {
            get
            {
                return new List<MusicQuestionModel>
                {
                    // MUZİK
                    ////new MusicQuestionModel
                    ////{
                    ////    QuestionTr = "Dinlediğiniz şarkı hangi müzisyene aittir?",
                    ////    QuestionEn = "Which musician is the song you are listening to?",
                    ////    QuestionType = QuestionTypes.Music
                    ////},
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Dinlediğiniz şarkının adı nedir?",
                    //    QuestionEn = "What is the name of the song you're listening to?",
                    //    QuestionType = QuestionTypes.Music
                    //},
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Dinlediğiniz şarkının albümünün adı nedir?",
                    //    QuestionEn = "What is the album name of the song you are listening to?",
                    //    QuestionType = QuestionTypes.Music
                    //},
                    // TEXT
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Sıla hangi tür şarkılar söyler?",
                    //    QuestionEn = "What genres of songs does Sila sing?",
                    //    QuestionType = QuestionTypes.Text
                    //},
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "\"Yeni ay\" albümü hangi şarkıcıya aittir?",
                    //    QuestionEn = "Which singer does \"New Moon\" album belong to?",
                    //    QuestionType = QuestionTypes.Text
                    ////},
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "\"Yeni ay\" albümü hangi yılda çıkmıştır?",
                    //    QuestionEn = "What year did the album \"New Moon\" come out?",
                    //    QuestionType = QuestionTypes.Text
                    //},
                    // RESİM
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Resimdeki albüm kapağının adı nedir?",
                    //    QuestionEn = "What is the name of the album cover in the picture?",
                    //    QuestionType = QuestionTypes.Image
                    //    },
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Resimde ki albüm hangi şarkıcıya aittir?",
                    //    QuestionEn = "Which artist does the album in the picture belong to?",
                    //    QuestionType = QuestionTypes.Image
                    //},
                    //new MusicQuestionModel
                    //{
                    //    QuestionTr = "Resimdeki şarkıcının adı nedir?",
                    //    QuestionEn = "What is the name of the singer in the picture?",
                    //    QuestionType = QuestionTypes.Image
                    //},
                };
            }
        }

        #endregion Questions

        #region Public

        /// <summary>
        /// Şarkıcı, albüm ve şarkı listelerini birleştirp tek model olarak döner
        /// </summary>
        /// <param name="artistId">Şarkıcı id</param>
        /// <returns>Şarkıcı, albüm ve şarkı bilgileri</returns>
        public async Task<SpotifyModel> GetArtistQuestionAsync(string artistId)
        {
            SpotifyModel result = new SpotifyModel
            {
                Artists = new List<ArtistModel> { await GetArtistAsync(artistId) },
                Albums = await GetArtistsAlbumsAsync(artistId),
            };

            if (!result.Albums.Any())
            {
                _logger.LogWarning("Şarkıcı albüm listesi boş geldi. artist Id: {artistId}", artistId);

                return result;
            }

            foreach (var album in result.Albums)
            {
                var tracks = await GetAlbumTracksAsync(album.AlbumId);
                result.Tracks.AddRange(tracks);
            }

            return result;
        }

        #endregion Public

        #region Access token

        /// <summary>
        /// Get Access Token
        /// </summary>
        private Token GetAccessToken(string clientId, string secretId)
        {
            string url5 = "https://accounts.spotify.com/api/token";

            var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientId, secretId)));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url5);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encode_clientid_clientsecret);

            var request = ("grant_type=client_credentials");
            byte[] req_bytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = req_bytes.Length;

            Stream strm = webRequest.GetRequestStream();
            strm.Write(req_bytes, 0, req_bytes.Length);
            strm.Close();

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            string json = "";
            using (Stream respStr = resp.GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(respStr, Encoding.UTF8))
                {
                    //should get back a string i can then turn to json and parse for accesstoken
                    json = rdr.ReadToEnd();
                    rdr.Close();
                }
            }

            return JsonConvert.DeserializeObject<Token>(json);
        }

        #endregion Access token

        #region Playlist

        /// <summary>
        /// Playlist id'ye ait şarkı, albüm ve şarkıcı bilgileri
        /// </summary>
        /// <param name="playlistId">Oynatma listesi id</param>
        /// <returns>Oynatma listesindeki şarkı, albüm ve şarkıcı bilgileri</returns>
        public async Task<SpotifyModel> GetPlaylistQuestions(string playlistId)
        {
            if (string.IsNullOrEmpty(playlistId))
            {
                _logger.LogWarning("Spotift play list id boş geldi");

                return new SpotifyModel();
            }

#pragma warning disable CS0618 // Type or member is obsolete
            var playlists = await _spotifyWebAPI.GetPlaylistTracksAsync(playlistId, limit: 9999);
#pragma warning restore CS0618 // Type or member is obsolete

            SpotifyModel result = new SpotifyModel()
            {
                Artists = new List<ArtistModel>(),
                Albums = new List<AlbumModel>(),
                Tracks = new List<TrackModel>()
            };

            if (playlists == null || playlists.Items == null || !playlists.Items.Any())
            {
                _logger.LogWarning("Playlist bilgisi boş geldi. play list id: {playlistId}", playlistId);

                return result;
            }

            foreach (var playlist in playlists.Items)
            {
                result.Artists.AddRange(playlist.Track.Artists.Select(x => GetArtistAsync(x.Id).Result).ToList());

                result.Albums.AddRange(new List<AlbumModel>
                {
                    new AlbumModel
                    {
                        AlbumImage = playlist.Track.Album.Images.FirstOrDefault(x => x.Width <= 300).Url,
                        ArtistName = UppercaseFirst(playlist.Track.Album.Artists.FirstOrDefault().Name),
                        AlbumName = UppercaseFirst(playlist.Track.Album.Name),
                        AlbumId = playlist.Track.Album.Id,
                        ReleaseYear = playlist.Track.Album.ReleaseDate.Length > 4 ? Convert.ToDateTime(playlist.Track.Album.ReleaseDate).Year : Convert.ToInt32(playlist.Track.Album.ReleaseDate)
                    }
                });

                result.Tracks.AddRange(new List<TrackModel>
                {
                    new TrackModel
                    {
                        AlbulId = playlist.Track.Album.Id,
                        TrackName = UppercaseFirst(playlist.Track.Name),
                        TracksUrl = playlist.Track.PreviewUrl,
                        ArtisName = UppercaseFirst(playlist.Track.Artists.FirstOrDefault().Name),
                    }
                });
            }

            return result;
        }

        #endregion Playlist

        #region Artist

        /// <summary>
        /// Şarkıcı bilgisi
        /// </summary>
        /// <param name="artistId">Şarkıcı id</param>
        /// <returns>Şarkıcı adı, resmi ve türü</returns>
        private async Task<ArtistModel> GetArtistAsync(string artistId)
        {
            if (string.IsNullOrEmpty(artistId))
            {
                _logger.LogWarning("Spotift artis id boş geldi.");

                return new ArtistModel();
            }

            var artist = await _spotifyWebAPI.GetArtistAsync(artistId);
            if (artist == null)
            {
                _logger.LogWarning("Spotift  artis bilgisi boş geldi. artist Id: {artistId}", artistId);

                return new ArtistModel();
            }

            return new ArtistModel
            {
                ArtistName = UppercaseFirst(artist.Name),
                ArtistImage = artist.Images.FirstOrDefault(x => x.Width <= 300) != null
                ? artist.Images.FirstOrDefault(x => x.Width <= 300).Url
                : "",// subcategory resmi
                Genres = UppercaseFirst(string.Join(", ", artist.Genres).ToLower().Replace("turkish", "")),// şarkıcı tarzı(pop, arebest...)
            };
        }

        /// <summary>
        /// Şarkıcıya ait albüler
        /// </summary>
        /// <param name="artistId">Şarkıcı id</param>
        /// <returns>Albüm adı, resmi ve id</returns>
        private async Task<List<AlbumModel>> GetArtistsAlbumsAsync(string artistId)
        {
            if (string.IsNullOrEmpty(artistId))
            {
                _logger.LogWarning("Spotift artis id boş geldi");

                return new List<AlbumModel>();
            }

            var artistAlbum = await _spotifyWebAPI.GetArtistsAlbumsAsync(artistId, limit: 9999, type: AlbumType.Album | AlbumType.Single);
            if (artistAlbum.Items == null)
            {
                _logger.LogWarning("Spotify albüm listesi boş geldi. artist Id: {artistId}", artistId);

                return new List<AlbumModel>();
            }

            _logger.LogInformation("Spotift Toplam {Count} adet şarkıcı albümü bulundu.", artistAlbum.Items.Count);

            return artistAlbum.Items.Select(album => new AlbumModel
            {
                AlbumImage = album.Images.FirstOrDefault(x => x.Width <= 300).Url,
                AlbumName = UppercaseFirst(album.Name),
                AlbumId = album.Id,
                ArtistName = UppercaseFirst(album.Artists.FirstOrDefault().Name),
                ReleaseYear = album.ReleaseDate.Length > 4 ? Convert.ToDateTime(album.ReleaseDate).Year : Convert.ToInt32(album.ReleaseDate)
            }).ToList();
        }

        /// <summary>
        /// Albüme ait şarkıları verir
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        private async Task<List<TrackModel>> GetAlbumTracksAsync(string albumId)
        {
            if (string.IsNullOrEmpty(albumId))
            {
                _logger.LogWarning("Spotift albüm id boş geldi");

                return new List<TrackModel>();
            }

            var albumTracks = await _spotifyWebAPI.GetAlbumTracksAsync(albumId, limit: 9999);
            if (albumTracks.Items == null || !albumTracks.Items.Any())
            {
                _logger.LogWarning("Spotify albüm şarkıları çekilemedi. album Id: {albumId}", albumId);

                return new List<TrackModel>();
            }

            _logger.LogInformation("Spotift Toplam {Count} adet albüm şarkısı bulundu.", albumTracks.Items.Count);

            return albumTracks.Items.Select(x => new TrackModel
            {
                TrackName = UppercaseFirst(x.Name),
                TracksUrl = x.PreviewUrl,
                AlbulId = albumId
            }).ToList();
        }

        #endregion Artist

        #region Helper

        private string UppercaseFirst(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower()).Trim();
        }

        #endregion Helper
    }
}
