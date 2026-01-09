namespace AudioManagement.Core
{
    public interface IAudioController
    {
        void Mute();
        void Unmute();
        void ProvideAudioClipsPath(string path);
    }
}
