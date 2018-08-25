using Plugin.SimpleAudioPlayer;
using System;
using System.IO;
using System.Reflection;

namespace ContestPark.Mobile.Services.Audio
{
    public class AudioService : IAudioService
    {
        #region Methods

        /// <summary>
        /// Ses çalar
        /// </summary>
        /// <param name="audio">Bu enumun içindeki enumlar ses dosyalarının adları ile aynı olmalı</param>
        /// <param name="loop">Ses tekrar tekrar çalmasını sağlar</param>
        public void Play(Audio audio, bool loop = false)
        {
            var assembly = typeof(ContestParkApp).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream("ContestPark.Mobile." + $"Audios.{audio.ToString()}.mp3");
            if (audioStream != null)
            {
                ISimpleAudioPlayer player = CrossSimpleAudioPlayer.Current;
                if (player == null)
                    return;

                player.Load(audioStream);

                player.Play();

                if (loop)
                {
                    player.PlaybackEnded += Player_PlaybackEnded;
                }
            }
        }

        private void Player_PlaybackEnded(object sender, EventArgs e)
        {
            CrossSimpleAudioPlayer.Current?.Play();
        }

        public void Stop()
        {
            ISimpleAudioPlayer currentPlayer = CrossSimpleAudioPlayer.Current;
            if (currentPlayer != null)
            {
                currentPlayer.Stop();
                currentPlayer.PlaybackEnded -= Player_PlaybackEnded;
            }
        }

        #endregion Methods
    }
}