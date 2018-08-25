namespace ContestPark.Mobile.Services.Audio
{
    public interface IAudioService
    {
        void Play(Audio audio, bool loop = false);

        void Stop();
    }
}