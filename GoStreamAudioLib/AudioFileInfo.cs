using NAudio.Wave;
using System;
using System.IO;

namespace GoStreamAudioLib
{
    public class AudioFileInfo
    {
        private string fileName = "";
        private string fullPath = "";

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

        #region Constructor
        
        public AudioFileInfo(string fName, string fPath)
        {
            fileName = fName;
            fullPath = fPath;
        }

        #endregion

    }
}
