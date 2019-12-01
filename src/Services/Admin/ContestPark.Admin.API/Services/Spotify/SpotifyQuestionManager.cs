using ContestPark.Admin.API.Enums;
using ContestPark.Admin.API.Model.Question;
using ContestPark.Admin.API.Model.Spotify;
using ContestPark.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Admin.API.Services.Spotify
{
    public class SpotifyQuestionManager
    {
        #region Private fields

        private readonly short _subCategoryId;

        #endregion Private fields

        #region Constructor

        public SpotifyQuestionManager(SpotifyModel spotify, short subCategoryId, SpotifyQuestionTypes spotifyQuestionTypes)
        {
            if (spotify == null || spotify.Albums == null || spotify.Artists == null || spotify.Tracks == null || subCategoryId <= 0)
                return;

            _subCategoryId = subCategoryId;
            SpotifyQuestionTypes = spotifyQuestionTypes;

            Question1(spotify.Tracks);
            Question2(spotify.Tracks, spotify.Albums);
            Question3(spotify.Artists);
            Question4(spotify.Albums);
            Question5(spotify.Albums);
            Question6(spotify.Tracks);
            Question7(spotify.Albums);
            Question8(spotify.Albums);
            Question9(spotify.Artists);
        }

        #endregion Constructor

        #region Properties

        private List<string> Genres
        {
            get
            {
                return new List<string>
                {
                    "Acoustic","Afrobeat","Alt-rock","Alternative","Ambient","Anime","Black-metal","Bluegrass","Blues","Bossanova","Brazil","Breakbeat","British","Cantopop","Chicago-house","Children","Chill","Classical","Club","Comedy","Country","Dance","Dancehall","Death-metal","Deep-house","Detroit-techno","Disco","Disney","Drum-and-bass","Dub","Dubstep","Edm","Electro","Electronic","Emo","Folk","Forro","French","Funk","Garage","German","Gospel","Goth","Grindcore","Groove","Grunge","Guitar","Happy","Hard-rock","Hardcore","Hardstyle","Heavy-metal","Hip-hop","Holidays","Honky-tonk","House","Idm","Indian","Indie","Indie-pop","Industrial","Iranian","J-dance","J-idol","J-pop","J-rock","Jazz","K-pop","Kids","Latin","Latino","Malay","Mandopop","Metal","Metal-misc","Metalcore","Minimal-techno","Movies","Mpb","New-age","New-release","Opera","Pagode","Party","Philippines-opm","Piano","Pop","Pop-film","Post-dubstep","Power-pop","Progressive-house","Psych-rock","Punk","Punk-rock","Modern Rock","Pop Punk","R-n-b","Rainy-day","Reggae","Reggaeton","Road-trip","Rock","Rock-n-roll","Rockabilly","Romance","Sad","Salsa","Samba","Sertanejo","Show-tunes","Singer-songwriter","Ska","Sleep","Songwriter","Soul","Soundtracks","Spanish","Study","Summer","Swedish","Synth-pop","Tango","Techno","Trance","Trip-hop","Turkish","Work-out","World-music"
                };
            }
        }

        public List<QuestionSaveModel> Questions { get; set; } = new List<QuestionSaveModel>();

        private SpotifyQuestionTypes SpotifyQuestionTypes { get; }

        #endregion Properties

        #region Şarkıcı sorularını oluşturur

        /// <summary>
        /// Dinlediğiniz şarkının adı nedir?
        /// </summary>
        /// <param name="tracks"></param>
        private void Question1(List<TrackModel> tracks)
        {
            if (tracks == null || !tracks.Any())
                return;

            var question1 = tracks
                               .Select(track => new QuestionSaveModel
                               {
                                   Questions = new List<Question>
                                   {
                                        new Question
                                        {
                                            QuestionType = QuestionTypes.Music,
                                            AnswerTypes = AnswerTypes.Text,
                                            Link = track.TracksUrl,
                                            SubCategoryId = _subCategoryId
                                        },
                                   },
                                   Answers = GetRandomAnswers(tracks.Select(x => x.TrackName).ToList(), track.TrackName),
                                   QuestionLocalized = new List<QuestionLocalized>
                                                                          {
                                                                                new QuestionLocalized
                                                                                {
                                                                                     Language= Languages.Turkish,
                                                                                     Question =  "Dinlediğiniz şarkının adı nedir?",
                                                                                },
                                                                                new QuestionLocalized
                                                                                {
                                                                                    Language = Languages.English,
                                                                                    Question =  "What is the name of the song you're listening to?",
                                                                                }
                                                                          }
                               }).ToList();

            Questions.AddRange(question1);
        }

        /// <summary>
        /// Dinlediğiniz şarkının albümünün adı nedir?
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="albums"></param>
        private void Question2(List<TrackModel> tracks, List<AlbumModel> albums)
        {
            if (tracks == null || !tracks.Any() || albums == null || !albums.Any())
                return;

            var question2 = tracks
                                .Select(track => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                            new Question
                                            {
                                                QuestionType = QuestionTypes.Music,
                                                AnswerTypes = AnswerTypes.Text,
                                                Link = track.TracksUrl,
                                                SubCategoryId = _subCategoryId
                                            },
                                    },
                                    Answers = GetRandomAnswers(albums.Where(a => a.AlbumId != track.AlbulId).Select(x => x.AlbumName),
                                                               correctAnswer: albums.FirstOrDefault(a => a.AlbumId == track.AlbulId).AlbumName),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                         Language= Languages.Turkish,
                                                                                         Question = "Dinlediğiniz şarkının albümünün adı nedir?",
                                                                                    },
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                        Language = Languages.English,
                                                                                        Question = "What is the album name of the song you are listening to?",
                                                                                    }
                                                                           }
                                }).ToList();

            Questions.AddRange(question2);
        }

        /// <summary>
        /// Şarkıcının söylediği türkler(pop, arabest vs)
        /// </summary>
        /// <param name="genres">artistInfo.Artist.Genres</param>
        private void Question3(List<ArtistModel> artists)
        {
            if (artists == null || !artists.Any())
                return;

            var question3 = artists
                                .Select(artis => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                            new Question
                                            {
                                                QuestionType = QuestionTypes.Text,
                                                AnswerTypes = AnswerTypes.Text,
                                                Link = string.Empty,
                                                SubCategoryId = _subCategoryId
                                            },
                                    },
                                    Answers = GetRandomAnswers(Genres.Where(a => !a.Contains(artis.Genres)),
                                                               correctAnswer: artis.Genres),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                         Language= Languages.Turkish,
                                                                                         Question =  $"{artis.ArtistName} hangi tür şarkılar söyler?",
                                                                                    },
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                        Language = Languages.English,
                                                                                        Question = $"What genres of songs does {artis.ArtistName} sing?",
                                                                                    }
                                                                           }
                                }).ToList();

            Questions.AddRange(question3);
        }

        /// <summary>
        /// "Yeni ay" albümü hangi yılda çıkmıştır?
        /// </summary>
        /// <param name="albums">Albümler</param>
        private void Question4(List<AlbumModel> albums)
        {
            if (albums == null || !albums.Any())
                return;

            var question4 = albums
                                .Select(track => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                            new Question
                                            {
                                                QuestionType = QuestionTypes.Text,
                                                AnswerTypes = AnswerTypes.Text,
                                                Link = string.Empty,
                                                SubCategoryId = _subCategoryId
                                            },
                                    },
                                    Answers = GetRandomAnswers(albums.Where(a => a.ReleaseYear != track.ReleaseYear).Select(x => x.ReleaseYear.ToString()).ToList(),
                                                               correctAnswer: track.ReleaseYear.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                         Language= Languages.Turkish,
                                                                                         Question =  $"\"{track.AlbumName}\" albümü hangi yılda çıkmıştır?"
                                                                                    },
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                        Language = Languages.English,
                                                                                        Question = $"What year did the album \"{track.AlbumName}\" come out?"
                                                                                    }
                                                                           }
                                }).ToList();

            Questions.AddRange(question4);
        }

        /// <summary>
        /// Resimdeki albüm kapağının adı nedir?
        /// </summary>
        private void Question5(List<AlbumModel> albums)
        {
            if (albums == null || !albums.Any())
                return;

            var question5 = albums
                                .Select(track => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                            new Question
                                            {
                                                QuestionType = QuestionTypes.Image,
                                                AnswerTypes = AnswerTypes.Text,
                                                Link = track.AlbumImage,
                                                SubCategoryId = _subCategoryId
                                            },
                                    },
                                    Answers = GetRandomAnswers(albums.Where(a => a.AlbumName != track.AlbumName).Select(x => x.AlbumName).ToList(),
                                                               correctAnswer: track.AlbumName.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                         Language= Languages.Turkish,
                                                                                         Question =  "Resimdeki albüm kapağının adı nedir?"
                                                                                    },
                                                                                    new QuestionLocalized
                                                                                    {
                                                                                        Language = Languages.English,
                                                                                        Question = "What is the name of the album cover in the picture?"
                                                                                    }
                                                                           }
                                }).ToList();

            Questions.AddRange(question5);
        }

        private void Question6(List<TrackModel> tracks)
        {
            if (SpotifyQuestionTypes != SpotifyQuestionTypes.Playlist || tracks == null || !tracks.Any())
                return;

            var question5 = tracks
                                .Select(track => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                                new Question
                                                {
                                                    QuestionType = QuestionTypes.Music,
                                                    AnswerTypes = AnswerTypes.Text,
                                                    Link = track.TracksUrl,
                                                    SubCategoryId = _subCategoryId
                                                },
                                    },
                                    Answers = GetRandomAnswers(tracks.Where(a => a.ArtisName != track.ArtisName).Select(x => x.ArtisName).ToList(),
                                                               correctAnswer: track.ArtisName.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                             Language= Languages.Turkish,
                                                                                             Question =  "Dinlediğiniz şarkı hangi müzisyene aittir?"
                                                                                        },
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                            Language = Languages.English,
                                                                                            Question = "Which musician is the song you are listening to?"
                                                                                        }
                                                                           }
                                }).ToList();

            Questions.AddRange(question5);
        }

        private void Question7(List<AlbumModel> albums)
        {
            if (SpotifyQuestionTypes != SpotifyQuestionTypes.Playlist || albums == null || !albums.Any())
                return;

            var question5 = albums
                                .Select(album => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                                new Question
                                                {
                                                    QuestionType = QuestionTypes.Text,
                                                    AnswerTypes = AnswerTypes.Text,
                                                    Link = string.Empty,
                                                    SubCategoryId = _subCategoryId
                                                },
                                    },
                                    Answers = GetRandomAnswers(albums.Where(a => a.ArtistName != album.ArtistName).Select(x => x.ArtistName).ToList(),
                                                               correctAnswer: album.ArtistName.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                             Language= Languages.Turkish,
                                                                                             Question =  $"\"{album.AlbumName}\" albümü hangi şarkıcıya aittir?"
                                                                                        },
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                            Language = Languages.English,
                                                                                            Question = $"Which singer does \"{album.AlbumName}\" album belong to?",
                                                                                        }
                                                                           }
                                }).ToList();

            Questions.AddRange(question5);
        }

        private void Question8(List<AlbumModel> albums)
        {
            if (SpotifyQuestionTypes != SpotifyQuestionTypes.Playlist || albums == null || !albums.Any())
                return;

            var question5 = albums
                                .Select(album => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                                new Question
                                                {
                                                    QuestionType = QuestionTypes.Image,
                                                    AnswerTypes = AnswerTypes.Text,
                                                    Link = album.AlbumImage,
                                                    SubCategoryId = _subCategoryId
                                                },
                                    },
                                    Answers = GetRandomAnswers(albums.Where(a => a.ArtistName != album.ArtistName).Select(x => x.ArtistName).ToList(),
                                                               correctAnswer: album.ArtistName.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                             Language= Languages.Turkish,
                                                                                             Question =  "Resimde ki albüm hangi şarkıcıya aittir?"
                                                                                        },
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                            Language = Languages.English,
                                                                                            Question =  "Which artist does the album in the picture belong to?",
                                                                                        }
                                                                           }
                                }).ToList();

            Questions.AddRange(question5);
        }

        private void Question9(List<ArtistModel> artis)
        {
            if (SpotifyQuestionTypes != SpotifyQuestionTypes.Playlist || artis == null || !artis.Any())
                return;

            var question5 = artis
                                .Where(x => !string.IsNullOrEmpty(x.ArtistImage))
                                .Select(album => new QuestionSaveModel
                                {
                                    Questions = new List<Question>
                                    {
                                                new Question
                                                {
                                                    QuestionType = QuestionTypes.Image,
                                                    AnswerTypes = AnswerTypes.Text,
                                                    Link = album.ArtistImage,
                                                    SubCategoryId = _subCategoryId
                                                },
                                    },
                                    Answers = GetRandomAnswers(artis.Where(a => a.ArtistName != album.ArtistName).Select(x => x.ArtistName).ToList(),
                                                               correctAnswer: album.ArtistName.ToString()),
                                    QuestionLocalized = new List<QuestionLocalized>
                                                                           {
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                             Language= Languages.Turkish,
                                                                                             Question =  "Resimdeki şarkıcının adı nedir?",
                                                                                        },
                                                                                        new QuestionLocalized
                                                                                        {
                                                                                            Language = Languages.English,
                                                                                            Question = "What is the name of the singer in the picture?",
                                                                                        }
                                                                           }
                                }).ToList();

            Questions.AddRange(question5);
        }

        #endregion Şarkıcı sorularını oluşturur

        #region Helper

        /// <summary>
        /// Rastgele cevap listesi verir
        /// </summary>
        /// <param name="correctAnswer">Doğru cevap</param>
        /// <param name="correctAnswer">Doğru cevap</param>
        /// <returns>Cevap listesi</returns>
        private List<AnswerSaveModel> GetRandomAnswers(IEnumerable<string> answers, string correctAnswer)
        {
            var answersRandomSort = answers
                             .Where(x => x != correctAnswer)
                             .OrderBy(x => Guid.NewGuid())
                             .Take(3)
                             .ToList();

            if (answersRandomSort == null || answersRandomSort.Count < 3)
                return new List<AnswerSaveModel>();

            return new List<AnswerSaveModel>
            {
                    new AnswerSaveModel
                {
                    Language = Languages.Turkish,
                    CorrectStylish = correctAnswer,
                    Stylish1 = answersRandomSort[0],
                    Stylish2 = answersRandomSort[1],
                    Stylish3 = answersRandomSort[2]
                },
                    new AnswerSaveModel
                {
                    Language = Languages.English,
                    CorrectStylish = correctAnswer,
                    Stylish1 = answersRandomSort[0],
                    Stylish2 = answersRandomSort[1],
                    Stylish3 = answersRandomSort[2]
                }
            };
        }

        #endregion Helper
    }
}
