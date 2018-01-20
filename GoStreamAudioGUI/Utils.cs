using System;

namespace GoStreamAudioGUI
{
    public static class Utils
    {
        public static string FormatTimeSpan(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}", (int)ts.TotalMinutes, ts.Seconds);
        }

        public static string FormatTimeSpan2(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)ts.TotalHours, (int)ts.Minutes, ts.Seconds);
        }
    }
}
