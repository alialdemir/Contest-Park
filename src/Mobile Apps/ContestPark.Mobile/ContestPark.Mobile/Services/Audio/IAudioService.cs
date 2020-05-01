using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Services.Audio
{
    public interface IAudioService
    {
        void FastForward();

        void Play(AudioTypes audio, bool loop = false);

        void Stop();

        void ToggleAudio(string mp3Url);

        void WindBack();
    }
}
