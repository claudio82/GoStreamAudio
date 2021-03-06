﻿using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Flac;
using System;
using System.Diagnostics;
using Luminescence.Xiph;

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

    public class LocalAudioPlayer : IAudioPlayer, IDisposable
    {
        // private fields
        private IWavePlayer wavePlayer;
        private AudioFileReader file;
        private VorbisWaveReader vorbisReader;
        private FlacReader flacReader;
        private string fileName;
        public PlaybackStopTypes PlaybackStopType { get; set; }

        #region Constructor

        /// <summary>
        /// Initialize a local audio player with given type
        /// </summary>
        /// <param name="fileName">the file name to play</param>
        /// <param name="waveOutType">the wave out type to use</param>
        public LocalAudioPlayer(string fileName, WaveOutType waveOutType)
        {
            this.fileName = fileName;
            PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
            Debug.Assert(this.wavePlayer == null);
            this.wavePlayer = CreateWavePlayer(waveOutType);
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

        #endregion

        #region Public Methods

        /// <summary>
        /// unsupported operation
        /// </summary>
        public void StreamMp3()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// play an audio file
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
        /// pause an audio file
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

                if (file != null)
                {
                    file.Position = 0L;
                }
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

        /// <summary>
        /// returns true if player state is PlaybackState.Stopped
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// sets the volume of audio file (works only for Media Foundation file types)
        /// </summary>
        /// <param name="val"></param>
        public void SetVolume(float val)
        {
            if (file != null)
                file.Volume = val;
            //if (vorbisReader != null)
            //    wavePlayer.Volume = val;
        }

        /// <summary>
        /// sets the current time position of audio file
        /// </summary>
        /// <param name="ts"></param>
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

        /// <summary>
        /// saves mp3 tag information to file
        /// </summary>
        /// <param name="taglibFile"></param>
        public void SaveMp3Tag(TagLib.File taglibFile)
        {
            if (taglibFile != null)
            {
                //string fName = file.FileName;
                //AudioFileReader prev = file;
                if (file != null)
                {
                    float curVol = file.Volume;
                    wavePlayer.Stop();
                    //wavePlayer.Dispose();
                    //file.Close();
                    //file.Dispose();

                    string prevFile = this.file.FileName;

                    this.file.Close();
                    this.file.Dispose();
                    this.file = null;

                    try
                    {
                        taglibFile.Save();
                    }
                    catch (UnauthorizedAccessException)
                    { }
                    catch (Exception)
                    { }

                    this.file = new AudioFileReader(prevFile);
                    wavePlayer.Init(file);
                    SetVolume(curVol);
                    //wavePlayer.Play();
                }
            }
        }

        /// <summary>
        /// saves vorbis ogg or flac tag information to file
        /// </summary>
        /// <param name="taglibFile"></param>
        public void SaveVorbisTag(VorbisComment taglibFile)
        {
            if (taglibFile != null)
            {
                //string fName = file.FileName;
                //AudioFileReader prev = file;
                if (flacReader != null)
                {
                    //float curVol = file.Volume;
                    wavePlayer.Stop();
                    
                    string prevFile = this.fileName;

                    this.flacReader.Close();
                    this.flacReader.Dispose();
                    this.flacReader = null;

                    try
                    {
                        taglibFile.SaveMetadata();
                    }
                    catch (UnauthorizedAccessException)
                    { }
                    catch (Exception)
                    { }

                    this.flacReader = new FlacReader(prevFile);
                    wavePlayer.Init(this.flacReader);
                    //SetVolume(curVol);                    
                }
                else if (vorbisReader != null)
                {
                    wavePlayer.Stop();

                    string prevFile = this.fileName;

                    this.vorbisReader.Close();
                    this.vorbisReader.Dispose();
                    this.vorbisReader = null;

                    try
                    {
                        taglibFile.SaveMetadata();
                    }
                    catch (UnauthorizedAccessException)
                    { }
                    catch (Exception)
                    { }

                    this.vorbisReader = new VorbisWaveReader(prevFile);
                    wavePlayer.Init(this.vorbisReader);
                    //SetVolume(curVol);                    
                }
            }
        }

        /// <summary>
        /// releases all the player related resources
        /// </summary>
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
                this.wavePlayer.PlaybackStopped -= wavePlayer_PlaybackStopped;
                this.wavePlayer.Dispose();
                this.wavePlayer = null;
            }
        }

        #endregion

        #region Public and private events

        public event Action PlaybackResumed;
        //public event Action PlaybackPaused;
        public event Action PlaybackStopped;

        void wavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            //Dispose();
            if (PlaybackStopped != null)
            {
                PlaybackStopped();
            }
        }

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
        
    }
}
