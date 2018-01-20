using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
