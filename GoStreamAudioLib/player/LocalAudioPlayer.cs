using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Flac;
using System;
using System.Diagnostics;

namespace GoStreamAudioLib
{
    public enum WaveOutType
    {
        WaveOutSimpleType = 0,
        WaveOutWithCallback = 1,
        WaveOutEventType = 2
    }

    public enum PlaybackStopTypes
    {
        PlaybackStoppedByUser, PlaybackStoppedReachingEndOfFile
    }
    
    /*
    public enum AudioReaderType
    {
        AudioFileReader = 0,
        VorbisFileReader = 1,
        FlacFileReader = 2,
        Unknown = 3
    }*/

    public class LocalAudioPlayer : IAudioPlayer, IDisposable
    {
        // private fields
        private IWavePlayer wavePlayer;
        private AudioFileReader file;
        private VorbisWaveReader vorbisReader;
        private FlacReader flacReader;
        public PlaybackStopTypes PlaybackStopType { get; set; }

        //#region Public Properties

        ///// <summary>
        ///// the wave player instance
        ///// </summary>
        //public IWavePlayer WavePlayer
        //{
        //    get
        //    {
        //        return wavePlayer;
        //    }
        //}

        //#endregion

        /// <summary>
        /// Initialize a local audio player with given type
        /// </summary>
        /// <param name="fileName">the file name to play</param>
        /// <param name="waveOutType">the wave out type to use</param>
        public LocalAudioPlayer(string fileName, WaveOutType waveOutType)
        {
            PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
            Debug.Assert(this.wavePlayer == null);
            this.wavePlayer = CreateWavePlayer(waveOutType); /*new DirectSoundOut(200);*/
            wavePlayer.PlaybackStopped += wavePlayer_PlaybackStopped;
            try
            {
                
                if (fileName.ToLowerInvariant().EndsWith(".ogg"))
                {
                    vorbisReader = new VorbisWaveReader(fileName);                    
                    wavePlayer.Init(vorbisReader);
                }
                else if (fileName.ToLowerInvariant().EndsWith(".flac"))
                {
                    flacReader = new FlacReader(fileName);
                    wavePlayer.Init(flacReader);
                }
                else
                {
                    this.file = new AudioFileReader(fileName);
                    this.wavePlayer.Init(file);                    
                }
                
                //this.wavePlayer.PlaybackStopped += wavePlayer_PlaybackStopped;
            }
            catch (Exception)
            {                
                //throw;
            }
            
            //Play();
        }

        void wavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            //Dispose();
            if (PlaybackStopped != null)
            {
                PlaybackStopped();
            }
        }
        
        //private void wavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        //{
        //    //file.Position = 0;
        //    //sCleanUp();
        //}
        
        /// <summary>
        /// unsupported operation
        /// </summary>
        public void StreamMp3()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// play audio file
        /// </summary>
        public void Play()
        {
            if (wavePlayer != null)
            {
                try
                {
                    wavePlayer.Play();
                }
                catch (Exception)
                {
                    //throw;
                }
                if (PlaybackResumed != null)
                {
                    PlaybackResumed();
                }
            }
            
            //throw new NotImplementedException();
        }

        /// <summary>
        /// pause audio file
        /// </summary>
        public void Pause()
        {
            if (wavePlayer != null)
                wavePlayer.Pause();
        }

        /// <summary>
        /// stop audio playback
        /// </summary>
        public void StopPlayback()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                if (file != null) file.Position = 0L;
                if (vorbisReader != null) vorbisReader.Position = 0L;
                if (flacReader != null) flacReader.Position = 0L;
            }
        }

        /// <summary>
        /// returns true if player state is PlaybackState.Playing 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            if (wavePlayer != null)
            {
                if (wavePlayer.PlaybackState == PlaybackState.Playing)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// returns true if player state is PlaybackState.Paused
        /// </summary>
        /// <returns></returns>
        public bool IsPaused()
        {
            if (wavePlayer != null)
            {
                if (wavePlayer.PlaybackState == PlaybackState.Paused)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool IsStopped()
        {
            if (wavePlayer != null)
            {
                if (wavePlayer.PlaybackState == PlaybackState.Stopped)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// gets current playing time of file
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            if (file != null)
                return file.CurrentTime;
            if (vorbisReader != null)
                return vorbisReader.CurrentTime;
            if (flacReader != null)
                return flacReader.CurrentTime;
            return TimeSpan.Zero;
        }

        /// <summary>
        /// gets total playing time of file
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTotalTime()
        {
            if (file != null)
                return file.TotalTime;
            if (vorbisReader != null)
                return vorbisReader.TotalTime;
            if (flacReader != null)
                return flacReader.TotalTime;
            return TimeSpan.Zero;
        }

        public void SetVolume(float val)
        {
            if (file != null)
                file.Volume = val;
            //if (vorbisReader != null)
            //    wavePlayer.Volume = val;
        }

        public void SetCurrentTime(TimeSpan ts)
        {
            if (file != null && ts.CompareTo(file.TotalTime) < 0)
            {
                file.CurrentTime = ts;
            }
            if (vorbisReader != null && ts.CompareTo(vorbisReader.TotalTime) < 0)
            {
                vorbisReader.CurrentTime = ts;
            }
            if (flacReader != null && ts.CompareTo(flacReader.TotalTime) < 0)
            {
                flacReader.CurrentTime = ts;
            }
        }

        //public AudioReaderType GetReaderType()
        //{
        //    if (file != null)
        //        return AudioReaderType.AudioFileReader;
        //    else if (vorbisReader != null)
        //        return AudioReaderType.VorbisFileReader;
        //    else if (flacReader != null)
        //        return AudioReaderType.FlacFileReader;
        //    else
        //        return AudioReaderType.Unknown;
        //}

        #region Public Events

        public event Action PlaybackResumed;
        //public event Action PlaybackPaused;
        public event Action PlaybackStopped;

        #endregion

        #region Private Methods

        private void CleanUp()
        {          
        }

        private IWavePlayer CreateWavePlayer(WaveOutType woType)
        {
            switch(woType)
            {
                case WaveOutType.WaveOutEventType:
                    return new WaveOutEvent();
                case WaveOutType.WaveOutWithCallback:
                    return new WaveOut(WaveCallbackInfo.FunctionCallback());                
                case WaveOutType.WaveOutSimpleType:
                default:
                    return new WaveOut();
            }
        }
        #endregion

        public void Dispose()
        {
            if (this.file != null)
            {
                this.file.Close();
                this.file.Dispose();
                this.file = null;
            }
            if (vorbisReader != null)
            {
                this.vorbisReader.Close();
                this.vorbisReader.Dispose();
                this.vorbisReader = null;
            }
            if (flacReader != null)
            {
                this.flacReader.Close();
                this.flacReader.Dispose();
                this.flacReader = null;
            }
            if (this.wavePlayer != null)
            {
                //this.wavePlayer.PlaybackStopped -= wavePlayer_PlaybackStopped;
                this.wavePlayer.Dispose();
                this.wavePlayer = null;
            }     
        }
    }
}
