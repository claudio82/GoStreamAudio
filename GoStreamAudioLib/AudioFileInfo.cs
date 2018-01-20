using NAudio.Wave;
using System;
using System.IO;

namespace GoStreamAudioLib
{
    public class AudioFileInfo
    {
        private string fileName = "";
        private string fullPath = "";
        //private TimeSpan fileLength;

        #region Public Properties

        public string FileName {
            get
            {
                return fileName;
            }
        }

        public string FullPath
        {
            get
            {
                return fullPath;
            }
        }

        //public TimeSpan FileLength
        //{
        //    get
        //    {
        //        return fileLength;
        //    }
            
        //}

        #endregion

        public AudioFileInfo(string fName, string fPath)
        {
            fileName = fName;
            fullPath = fPath;
        }

        //#region Public Methods

        //public void SetDuration()
        //{
        //    if (fullPath != string.Empty && File.Exists(fullPath))
        //    {
        //        try
        //        {
        //            fileLength = new AudioFileReader(fullPath).TotalTime;
        //        }
        //        catch (Exception)
        //        {
        //            fileLength = TimeSpan.Zero;
        //        }
        //    }
        //    else
        //        fileLength = TimeSpan.Zero;
        //}

        //#endregion
    }
}
