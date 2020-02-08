namespace ContestPark.Mobile.Services.Audio
{
    public interface IAudioService
    {
        void FastForward();

        void Play(Audio audio, bool loop = false);

        void Stop();

        void ToggleAudio(string mp3Url);

        void WindBack();
    }
}
