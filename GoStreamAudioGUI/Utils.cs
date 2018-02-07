using System;
using System.IO;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public static class Utils
    {

        #region Public Methods

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}", (int)ts.TotalMinutes, ts.Seconds);
        }

        public static string FormatTimeSpan2(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)ts.TotalHours, (int)ts.Minutes, ts.Seconds);
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        #endregion
    }
}
