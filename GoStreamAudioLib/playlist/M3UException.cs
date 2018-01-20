using System;

namespace GoStreamAudioLib
{
    public class M3UException : Exception
    {
        public M3UException()
        {
        }

        public M3UException(string message)
            : base(message)
        {
        }
    }
}
