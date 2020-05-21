using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using Plugin.SimpleAudioPlayer;
using Prism.Services;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.Audio
{
    public class AudioService : IAudioService
    {
        #region Private variables

        private readonly ISimpleAudioPlayer _simpleAudioPlayer;
        private readonly IPageDialogService _pageDialogService;

        #endregion Private variables

        #region Constructor

        public AudioService(IPageDialogService pageDialogService)
        {
            _simpleAudioPlayer = CrossSimpleAudioPlayer.Current;
            _pageDialogService = pageDialogService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Ses çalar
        /// </summary>
        /// <param name="audio">Bu enumun içindeki enumlar ses dosyalarının adları ile aynı olmalı</param>
        /// <param name="loop">Ses tekrar tekrar çalmasını sağlar</param>
        public void Play(AudioTypes audio, bool loop = false)
        {
            if (_simpleAudioPlayer == null)
                return;

            var assembly = typeof(ContestParkApp).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream($"ContestPark.Mobile.Common.Audios.{audio}.mp3");
            if (audioStream != null)
            {
                _simpleAudioPlayer.Load(audioStream);

                _simpleAudioPlayer.Play();

                if (loop)
                {
                    _simpleAudioPlayer.PlaybackEnded += Player_PlaybackEnded;
                }
            }
        }

        private void Player_PlaybackEnded(object sender, EventArgs e)
        {
            if (_simpleAudioPlayer == null)
                return;

            CrossSimpleAudioPlayer.Current?.Play();
        }

        public void Stop()
        {
            if (_simpleAudioPlayer == null)
                return;

            _simpleAudioPlayer.Stop();
            _simpleAudioPlayer.PlaybackEnded -= Player_PlaybackEnded;
        }

        /// <summary>
        /// Mp3 müzik dosyası çalar
        /// </summary>
        /// <param name="mp3Url">Mp3 url</param>
        private void Play(string mp3Url)
        {
            if (_simpleAudioPlayer == null || string.IsNullOrEmpty(mp3Url) || CheckNetworkAsync())
                return;

            if (mp3Url.StartsWith("https://") || mp3Url.StartsWith("http://"))
            {
                WebClient wc = new WebClient();
                Stream fileStream = wc.OpenRead(mp3Url);

                _simpleAudioPlayer.Load(fileStream);

                _simpleAudioPlayer.Play();
            }
        }

        private bool CheckNetworkAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _pageDialogService.DisplayAlertAsync(string.Empty,
                                                     ContestParkResources.NoInternet,
                                                     ContestParkResources.Okay);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Müzik aç/kapat
        /// </summary>
        /// <param name="mp3Url">Mp3 müzik linki</param>
        public void ToggleAudio(string mp3Url)
        {
            if (_simpleAudioPlayer.IsPlaying)
                Stop();
            else
                Play(mp3Url);
        }

        /// <summary>
        /// Şarkıyı 5 saniye geri sarar
        /// </summary>
        public void WindBack()
        {
            Seek(-5);
        }

        /// <summary>
        /// Şarkıyı 5 saniye ileri sarar
        /// </summary>
        public void FastForward()
        {
            Seek(5);
        }

        /// <summary>
        /// Çağan şarkıyı position değeri kadar ileri sarar
        /// </summary>
        /// <param name="position">İleri sarılacak süre</param>
        private void Seek(double position = 5)
        {
            if (_simpleAudioPlayer == null)
                return;

            double nextPosition = _simpleAudioPlayer.CurrentPosition + position;
            if (nextPosition <= _simpleAudioPlayer.Duration)
                _simpleAudioPlayer.Seek(nextPosition);
        }

        #endregion Methods
    }
}
