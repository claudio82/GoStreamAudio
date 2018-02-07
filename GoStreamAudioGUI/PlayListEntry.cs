using System;

namespace GoStreamAudioGUI
{
    public class PlayListEntry
    {
        private int mPosInPlayList;
        private string mFileName;
        private bool hasPlayedOnce;

        #region Public Properties
        
        public int PosInPlayList
        {
            get
            {
                return mPosInPlayList;
            }
            set
            {
                mPosInPlayList = value;
            }
        }

        public string FileName
        {
            get
            {
                return mFileName;
            }
            set
            {
                mFileName = value;
            }
        }

        public bool HasPlayedOnce
        {
            get
            {
                return hasPlayedOnce;
            }
            set
            {
                hasPlayedOnce = value;
            }
        }

        #endregion

    }
}
