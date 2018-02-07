using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace GoStreamAudioLib
{
    public class Mp3StreamAudioPlayer : IAudioPlayer
    {
        // private fields

        // the mp3 url
        private string audioUrl;
        // mp3 downloaded state
        private bool fullyDownloaded;
        // the http web request
        private HttpWebRequest webRequest;
        // the buffered wave provider
        private BufferedWaveProvider bufferedWaveProvider;
        private VolumeWaveProvider16 volumeProvider;
        // the wave out object
        private IWavePlayer waveOut;
        // the current streaming playback state
        private StreamingPlaybackState playbackState;
        
        #region Public Properties

        public string AudioUrl
        {
            get
            {
                return audioUrl;
            }
            set
            {
                audioUrl = value;
            }
        }

        public StreamingPlaybackState StreamingPlaybackState
        {
            get
            {
                return playbackState;
            }
            set
            {
                playbackState = value;
            }
        }

        /// <summary>
        /// checks if audio buffer is almost full
        /// </summary>
        public bool IsBufferNearlyFull
        {
            get
            {
                return bufferedWaveProvider != null &&
                       bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes
                       < bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        #endregion

        #region Constructor
        
        public Mp3StreamAudioPlayer(string url)
        {
            this.audioUrl = url;
            playbackState = StreamingPlaybackState.Stopped;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// stream a mp3 audio
        /// </summary>
        public void StreamMp3()
        {
            fullyDownloaded = false;
            if (audioUrl != null && !string.IsNullOrEmpty(audioUrl))
            {
                try
                {
                    webRequest = (HttpWebRequest)WebRequest.Create(audioUrl);
                }
                catch (UriFormatException)
                {
                    Debug.WriteLine("URL format is not valid");
                    playbackState = StreamingPlaybackState.Stopped;
                    return;
                }
                catch (Exception)
                {
                    Debug.WriteLine("Error in web request create");
                    playbackState = StreamingPlaybackState.Stopped;
                    return;
                }
                HttpWebResponse resp;
                try
                {
                    resp = (HttpWebResponse)webRequest.GetResponse();
                }
                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.RequestCanceled)
                    {
                        Debug.WriteLine("Web Exception error : " + e.Message);
                        //ShowError(e.Message);
                    }
                    return;
                }
                var buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

                IMp3FrameDecompressor decompressor = null;

                try
                {
                    using (var responseStream = resp.GetResponseStream())
                    {
                        var readFullyStream = new ReadFullyStream(responseStream);
                        do
                        {
                            if (IsBufferNearlyFull)
                            {
                                Debug.WriteLine("Buffer getting full, taking a break");
                                Thread.Sleep(500);
                            }
                            else
                            {
                                // read next mp3 frame
                                Mp3Frame frame;
                                try
                                {
                                    frame = Mp3Frame.LoadFromStream(readFullyStream);
                                }
                                catch (EndOfStreamException)
                                {
                                    fullyDownloaded = true;
                                    // reached the end of the MP3 file / stream
                                    playbackState = StreamingPlaybackState.Stopped;
                                    break;
                                }
                                catch (WebException)
                                {
                                    break;
                                }

                                if (decompressor == null)
                                {
                                    // don't think these details matter too much - just help ACM select the right codec
                                    // however, the buffered provider doesn't know what sample rate it is working at
                                    // until we have a frame
                                    decompressor = CreateFrameDecompressor(frame);
                                    bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                                    bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
                                    //this.bufferedWaveProvider.BufferedDuration = 250;
                                }
                                if (frame != null)
                                {
                                    int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                                    //Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
                                    bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                                }
                                else
                                {
                                    fullyDownloaded = true;
                                    break;
                                }
                            }
                        }
                        while (playbackState != StreamingPlaybackState.Stopped);

                        Debug.WriteLine("Exiting");
                        // was doing this in a finally block, but for some reason
                        // we are hanging on response stream .Dispose so never get there
                        decompressor.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Generic Error: " + ex.Message);
                    playbackState = StreamingPlaybackState.Stopped;
                }
                finally
                {
                    if (decompressor != null)
                    {
                        decompressor.Dispose();
                        decompressor = null;
                    }
                }
            }
        }

        /// <summary>
        /// play mp3 audio
        /// </summary>
        public void Play()
        {
            waveOut.Play();
            Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", waveOut.PlaybackState));
            playbackState = StreamingPlaybackState.Playing;
        }

        /// <summary>
        /// pause mp3 audio reproduction
        /// </summary>
        public void Pause()
        {
            playbackState = StreamingPlaybackState.Buffering;
            waveOut.Stop();
            Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", waveOut.PlaybackState));
        }

        /// <summary>
        /// stop audio playback
        /// </summary>
        public void StopPlayback()
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (!fullyDownloaded)
                {
                    webRequest.Abort();
                }

                playbackState = StreamingPlaybackState.Stopped;
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                //timer1.Enabled = false;
                // n.b. streaming thread may not yet have exited
                Thread.Sleep(500);
                //ShowBufferState(0);
            }
        }

        /// <summary>
        /// returns a mp3 frame decompressor using audio format from mp3 frame
        /// </summary>
        /// <param name="frame">the mp3 frame</param>
        /// <returns></returns>
        public static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                frame.FrameLength, frame.BitRate);
            return new AcmMp3FrameDecompressor(waveFormat);
        }

        /// <summary>
        /// initializes the audio streaming
        /// </summary>
        /// <returns></returns>
        public bool FlowAudio()
        {
            if (waveOut == null && bufferedWaveProvider != null)
            {
                Debug.WriteLine("Creating WaveOut Device");
                waveOut = CreateWaveOut();
                /*
                waveOut.PlaybackStopped += OnPlaybackStopped;
                 */
                volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                volumeProvider.Volume = 0.5f; //volumeSlider1.Volume;
                waveOut.Init(volumeProvider);
                /*
                progressBarBuffer.Maximum = (int)bufferedWaveProvider.BufferDuration.TotalMilliseconds;
                 */
                return false;
            }
            else if (bufferedWaveProvider != null)
            {

                var bufferedSeconds = bufferedWaveProvider.BufferedDuration.TotalSeconds;
                //ShowBufferState(bufferedSeconds);
                // make it stutter less if we buffer up a decent amount before playing
                if (bufferedSeconds < 0.5 && playbackState == StreamingPlaybackState.Playing && !fullyDownloaded)
                {
                    Pause();
                    return false;
                }
                else if ((bufferedSeconds >= 0.5 && playbackState == StreamingPlaybackState.Buffering)
                    || (bufferedSeconds > 3 && playbackState == StreamingPlaybackState.Buffering)
                    || (bufferedSeconds > 0 && playbackState == StreamingPlaybackState.Buffering))
                {
                    Play();
                    return false;
                }
                else if (fullyDownloaded && bufferedSeconds == 0)
                {
                    Debug.WriteLine("Reached end of stream");
                    StopPlayback();
                    return true;
                }
                return false;
            }
            return false;
        }

        #endregion

        #region Private Methods
        
        private IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
        }

        #endregion

    }
}
