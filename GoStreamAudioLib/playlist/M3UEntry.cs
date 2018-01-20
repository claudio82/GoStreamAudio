using System;

namespace GoStreamAudioLib
{
    public class M3UEntry
    {
        public M3UEntry(TimeSpan duration, string title, Uri path)
        {
            Duration = duration;
            Title = title;
            Path = path;
        }

        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public Uri Path { get; set; }
    }
}
