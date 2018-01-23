using System;

namespace GoStreamAudioLib
{
    public interface IAudioPlayer
    {
        void StreamMp3();
        void Play();
        void Pause();
        void StopPlayback();
    }
}
