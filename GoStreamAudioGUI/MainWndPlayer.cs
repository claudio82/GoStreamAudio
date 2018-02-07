using GoStreamAudioLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public partial class MainWndPlayer : LocalizedForm
    {

        #region Private Fields
        
        private const float VOL_INIT = 0.5f;

        //private fields        
        private EventWaitHandle waitHandle = new ManualResetEvent(false);
        private AudioFileInfo currentAudioFile;
        private string mAudioFile;
        private LocalAudioPlayer audioPlayer;
        private string lastPlayDirectory = "";
        private volatile bool userTbAudioUpdating = false;
        private volatile bool mUpdateAudioPos = true;
        private volatile bool userStopped = false;
        private volatile bool isWaitingHandle = false;
        private volatile bool isSingleFilePlaying = false;
        private System.Timers.Timer aTimer;
        private PlayListWnd plWnd;
        private volatile bool mIsPlaylistRunning = false;
        private volatile bool mIsShuffleChecked = false;
        private List<PlayListEntry> randSongsIdx;
        private int shuffleCnt = 0;

        private System.Resources.ResourceManager rm;

        #endregion
        
        #region Public Properties

        public string AudioFile
        {
            get
            {
                return mAudioFile;
            }
            set
            {
                this.mAudioFile = value;
            }
        }

        public LocalAudioPlayer AudioPlayer
        {
            get
            {
                return audioPlayer;
            }
        }

        public bool IsPlaylistRunning
        {
            get
            {
                return this.mIsPlaylistRunning;
            }
            set
            {
                this.mIsPlaylistRunning = value;
            }
        }

        public bool UserStopped
        {
            get
            {
                return userStopped;
            }
            set
            {
                this.userStopped = value;
            }
        }

        public bool IsWaitingHandle
        {
            get
            {
                return isWaitingHandle;
            }
            set
            {
                isWaitingHandle = value;
            }
        }

        public EventWaitHandle WaitHandle
        {
            get
            {
                return waitHandle;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWndPlayer()
            : base()
        {
            InitializeComponent();

            plWnd = new PlayListWnd(this);
            plWnd.CloseButtonClicked += plWnd_CloseButtonClicked;
            plWnd.PlaylistItemDoubleClicked += plWnd_PlaylistItemDoubleClicked;
            plWnd.PlaylistLoaded += plWnd_PlaylistLoaded;
            plWnd.PlaylistCleared += plWnd_PlaylistCleared;
            plWnd.Show(this);

            lblTotalTime.Text = "00:00";
            tbAudioPos.Enabled = false;

            EnableButtons(false);
            btnRew.Enabled = btnFwd.Enabled = false;
            chkShuffle.Enabled = false;
            marqueeLbl.Visible = false;

            aTimer = new System.Timers.Timer();
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Interval = 300;

            rm = new System.Resources.ResourceManager(typeof(GoStreamAudioGUI.MainWndPlayer));
            this.Text = rm.GetString("MainWndPlayer.Text");

            toolTipShuffle.SetToolTip(this.chkShuffle, rm.GetString("toolTipShuffle.Title"));

            //cultInfo = LocalizedForm.GlobalUICulture;
            if (LocalizedForm.GlobalUICulture.TwoLetterISOLanguageName != "en")
            {
                if (LocalizedForm.GlobalUICulture.TwoLetterISOLanguageName == "it")
                {
                    langEngTsmItem.Checked = false;
                    langItaTsmItem.Checked = true;
                }
                else
                    LocalizedForm.GlobalUICulture = new System.Globalization.CultureInfo("en-US");
            }

            BringToFront();
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// init the audio player
        /// </summary>
        public void InitPlayer(bool dispose = true, PlaybackStopTypes pbStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile)
        {
            if (aTimer.Enabled)
            {
                aTimer.Stop();
                aTimer.Enabled = false;
            }

            Action action;
            //action = () =>
            //    {
            try
            {
                if (audioPlayer != null)
                {
                    //waitHandle.Set();
                    if (audioPlayer.IsPlaying() || audioPlayer.IsPaused())
                    {
                        audioPlayer.StopPlayback();
                    }
                    while (isWaitingHandle)
                    {
                    }
                    audioPlayer.PlaybackResumed -= audioPlayer_PlaybackResumed;
                    //audioPlayer.PlaybackStopped -= audioPlayer_PlaybackStopped;
                    if (dispose)
                    {
                        audioPlayer.Dispose();
                        audioPlayer = null;
                    }

                }
            }
            catch (Exception)
            {
                //throw;
            }
            //    };
            //Invoke(action);
            try
            {
                //if (audioPlayer == null)

                audioPlayer =
                        new LocalAudioPlayer(mAudioFile, GetSelectedOutputDriver());
                audioPlayer.PlaybackStopType = pbStopType;
                //audioPlayer.PlaybackPaused += audioPlayer_PlaybackPaused;
                audioPlayer.PlaybackResumed += audioPlayer_PlaybackResumed;
                //audioPlayer.PlaybackStopped += audioPlayer_PlaybackStopped;
                if (!IsPlaylistRunning)
                    currentAudioFile = new AudioFileInfo(Path.GetFileName(mAudioFile), mAudioFile);
            }
            catch (Exception)
            {
                action = () => MessageBox.Show(this, string.Format("Cannot load file '{0}'", mAudioFile));
                Invoke(action);
            }
            if (audioPlayer != null)
            {
                action = () =>
                {
                    if (tbVolume.Value != VOL_INIT * 100)
                        audioPlayer.SetVolume((float)tbVolume.Value / 100f);
                    else
                        audioPlayer.SetVolume(VOL_INIT);
                };
                tbVolume.Invoke(action);
                action = () => lblTotalTime.Text = Utils.FormatTimeSpan(audioPlayer.GetTotalTime());
                lblTotalTime.Invoke(action);
                action = () =>
                {
                    tbAudioPos.Value = 0;
                    tbAudioPos.Maximum = (int)(audioPlayer.GetTotalTime().Minutes) * 60
                        + audioPlayer.GetTotalTime().Seconds;
                    if (tbAudioPos.Maximum > 0)
                        tbAudioPos.Enabled = true;
                    else
                        tbAudioPos.Enabled = false;
                };
                tbAudioPos.Invoke(action);
                action = () =>
                {
                    UpdateMarquee();
                };
                Invoke(action);
            }
        }

        public void InitBgWorker()
        {
            if (bgPlayWorker == null)
            {
                bgPlayWorker = new BackgroundWorker();
                bgPlayWorker.WorkerReportsProgress = false;
                bgPlayWorker.WorkerSupportsCancellation = true;
                this.bgPlayWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgPlayWorker_DoWork);
                this.bgPlayWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgPlayWorker_RunWorkerCompleted);
            }
        }

        public void EnableButtons(bool playing)
        {
            Action action = () =>
            {
                btnPlay.Enabled = !playing;
                btnPause.Enabled = playing;
                btnStop.Enabled = playing;
            };
            Invoke(action);
        }

        #endregion

        #region Private methods
        
        /// <summary>
        /// opens a single audio file
        /// </summary>
        /// <returns></returns>
        private bool SelectInputFile()
        {
            bool isSelOk = false;
            //Thread t = new Thread(delegate() 
            {
                try
                {                    
                    string lblFileType = rm.GetString("openAudioFilterType", LocalizedForm.GlobalUICulture);
                    openSFileDlg.Filter = lblFileType + "|*.mp3;*.m4a;*.wav;*.ogg;*.wma;*.flac"; //;*.aiff";*.wma";
                    openSFileDlg.Title = openSFileDlg.Title; //"Open Audio File";
                    if (lastPlayDirectory == "")
                        openSFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) != null
                            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Environment.CurrentDirectory; //@"C:\";
                    else
                        openSFileDlg.InitialDirectory = lastPlayDirectory;
                    openSFileDlg.RestoreDirectory = false;

                    Invoke((Action)(() =>
                    {
                        if (openSFileDlg.ShowDialog() == DialogResult.OK)
                        {                            
                            mAudioFile = openSFileDlg.FileName;
                            //InitPlayer();
                            isSelOk = true;
                            //StartPlaybackThread();
                            lastPlayDirectory = Path.GetDirectoryName(openSFileDlg.FileName);
                            if (IsPlaylistRunning)
                            {
                                CurrentTrackCompleted(this, null);
                                IsPlaylistRunning = false;
                                plWnd.LastFileIdx = -1;
                                chkRepeat.Enabled = chkShuffle.Enabled = false;
                                btnRew.Enabled = btnFwd.Enabled = false;
                                //marqueeLbl.Visible = false;
                            }
                        }
                    }));
                }
                catch (Exception)
                {

                }
                return isSelOk;
                //});
                //t.IsBackground = true;
                //t.Start();
            }
        }

        //private void audioPlayer_PlaybackStopped()
        //{
            
        //}

        private void audioPlayer_PlaybackResumed()
        {
            if (audioPlayer != null)
                audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
            //hasResumed = true;
        }

        //private void audioPlayer_PlaybackPaused()
        //{
            
        //}

        /// <summary>
        /// starts audio file playback
        /// </summary>
        private bool BeginPlayback()
        {
            bool canPlay = false;
            //audioPlayer =
            //    new LocalAudioPlayer(mAudioFile, WaveOutType.WaveOutSimpleType ); //GetSelectedOutputDriver());            
            Action action;
            try
            {
                if (audioPlayer != null)
                {
                    audioPlayer.Play();                                        
                    canPlay = true;
                    action = () =>
                    {
                        EnableButtons(true);
                        //if (audioPlayer != null)
                        //{
                        //    if (audioPlayer.GetReaderType() == AudioReaderType.AudioFileReader)
                        //        tbVolume.Enabled = true;
                        //    else
                        //    {
                        //        // disable volume ctrl because it is currently unsupported for other file types
                        //        tbVolume.Enabled = false;
                        //    }
                        //}
                    };
                    Invoke(action);
                }
                else
                {
                    aTimer.Enabled = false;
                    action = () => EnableButtons(true);
                    Invoke(action);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            { }
            catch (InvalidOperationException)
            {
                Action act = () =>
                {
                    canPlay = false;
                    EnableButtons(false);
                    MessageBox.Show(this, "Cannot play this file!");                    
                };
                Invoke(act);
            }
            catch (Exception)
            {
                Action act = () =>
                {
                    canPlay = false;
                    EnableButtons(false);
                    MessageBox.Show(this, "Cannot play this file!");
                };
                Invoke(act);
            }
            return canPlay;
        }

        /// <summary>
        /// starts the play audio thread
        /// </summary>
        public void StartPlaybackThread()
        {
            if (mAudioFile != null)
            {
                
                if (bgPlayWorker.IsBusy != true)
                {
                    //btnPause.Enabled = true;
                    bgPlayWorker.RunWorkerAsync();
                }
                else
                {
                    if (audioPlayer.IsPaused() || audioPlayer.IsStopped())
                    {

                        if (!aTimer.Enabled)
                        {
                            aTimer.Enabled = true;
                            aTimer.Start();
                        }

                        //resume audio play
                        audioPlayer.Play();
                        Invoke((Action)(() =>
                        {
                            btnPause.Enabled = true;
                            btnPlay.Enabled = false;
                            btnStop.Enabled = true;
                        }));
                        
                    }
                }
            }
        }

        /// <summary>
        /// stops audio file playback
        /// </summary>
        public void StopPlayback()
        {
            if (audioPlayer != null)
            {
                audioPlayer.StopPlayback();                

                //audioPlayer.Dispose();
                //audioPlayer = null;
                
                Action action = () => lblNowTime.Text = "00:00";
                lblNowTime.Invoke(action);

                action = () =>
                {
                    tbAudioPos.Value = 0;
                };
                tbAudioPos.Invoke(action);
            }
        }

        /// <summary>
        /// updates audio position when trackbar is dragged and released
        /// </summary>
        private void UpdateAudioPos(int newVal)
        {          
            if (newVal != (int)(audioPlayer.GetCurrentTime().Minutes) * 60  + audioPlayer.GetCurrentTime().Seconds)
            {
                if (audioPlayer != null)
                {
                    if (audioPlayer.IsPlaying() || audioPlayer.IsPaused())
                    {
                        //audioPlayer.StopPlayback();
                        
                        TimeSpan newAudioPos = TimeSpan.FromMinutes(newVal / 60);
                        newAudioPos = newAudioPos.Add(TimeSpan.FromSeconds(newVal % 60));
                        audioPlayer.SetCurrentTime(newAudioPos);
                        tbAudioPos.Value = newVal;

                        //audioPlayer.Play();
                    }
                }
            }
        }

        /// <summary>
        /// gets selected output driver from user selection
        /// </summary>
        /// <returns></returns>
        private WaveOutType GetSelectedOutputDriver()
        {
            switch (comboBoxOutputDriver.SelectedIndex)
            {
                case 0:
                    return WaveOutType.WaveOutSimpleType;
                case 1:
                    return WaveOutType.WaveOutWithCallback;
                case 2:
                default:
                    return WaveOutType.WaveOutEventType;                    
            }
        }

        private void UpdateMarquee()
        {
            if (currentAudioFile != null)
            {
                Action action = () =>
                {
                    marqueeLbl.Invalidate();
                    marqueeLbl.Text = currentAudioFile.FileName;
                    marqueeLbl.Width = this.MinimumSize.Width + currentAudioFile.FileName.Length + 5;
                    marqueeLbl.ResetPosition(this.Width - currentAudioFile.FileName.Length - 20);
                    marqueeLbl.Visible = true;
                    marqueeLbl.FilePlayingFullPath = currentAudioFile.FullPath;
                    marqueeLbl.AudioPlayer = this.audioPlayer;
                };
                Invoke(action);
            }
        }

        private bool ResetPlayList(object sender, EventArgs e, int plIdx)
        {
            bool canPlay = false;
            if (plIdx >= 0)
            {
                plWnd.LastFileIdx = plIdx;
                if (plWnd.GetNextFileFromPlayList())
                {
                    if (File.Exists(mAudioFile))
                    {
                        InitPlayer();
                        StartPlaybackThread();
                        NextTrackStarted(sender, e);
                        canPlay = true;
                    }
                    else
                    {
                        MessageBox.Show(string.Format("File {0} not found", mAudioFile));
                    }
                }
            }
            return canPlay;
        }

        private void switchToLang()
        {
            if (langItaTsmItem.Checked == true)         //in italian
            {
                //create culture for italian
                LocalizedForm.GlobalUICulture = new System.Globalization.CultureInfo("it-IT");
            }
            else  //in english
            {
                LocalizedForm.GlobalUICulture = new System.Globalization.CultureInfo("en-US");
            }
            this.Culture = LocalizedForm.GlobalUICulture;
            plWnd.Culture = LocalizedForm.GlobalUICulture;
        }

        private void CleanUp()
        {
            if (bgPlayWorker != null)
            {
                this.bgPlayWorker.DoWork -= new System.ComponentModel.DoWorkEventHandler(this.bgPlayWorker_DoWork);
                this.bgPlayWorker.RunWorkerCompleted -= new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgPlayWorker_RunWorkerCompleted);
                bgPlayWorker.Dispose();
                bgPlayWorker = null;
            }
            if (waitHandle != null)
            {
                waitHandle.Close();
                //waitHandle.Dispose();
                waitHandle = null;
            }

            if (audioPlayer != null)
            {
                audioPlayer.StopPlayback();
                audioPlayer.Dispose();
                audioPlayer = null;
            }
            if (aTimer != null)
            {
                aTimer.Elapsed -= OnTimedEvent;
                aTimer.Dispose();
                aTimer = null;
            }
            if (marqueeLbl != null)
                marqueeLbl.Dispose();            
            plWnd.CloseButtonClicked -= plWnd_CloseButtonClicked;
            plWnd.PlaylistItemDoubleClicked -= plWnd_PlaylistItemDoubleClicked;
            plWnd.PlaylistLoaded -= plWnd_PlaylistLoaded;
            plWnd.PlaylistCleared -= plWnd_PlaylistCleared;
            plWnd.Close();
            plWnd.Dispose();
        }

        #endregion

        #region Event handlers

        private void bgPlayWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
                //break;
            }
            else
            {
                // start playback
                if (BeginPlayback())
                {
                    if (this.IsPlaylistRunning && plWnd.HasUserSelTrack)
                        plWnd.HasUserSelTrack = false;

                    aTimer.Enabled = true;
                    aTimer.Start();

                    isWaitingHandle = true;
                    waitHandle.WaitOne();

                    waitHandle.Reset();
                }
            }
        }

        private void bgPlayWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Canceled!");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                //Debug.WriteLine("BACKGROUND WORKER THREAD FINISHED.");

                isWaitingHandle = false;

                if (IsPlaylistRunning)
                {
                    if (!isSingleFilePlaying)
                    {
                        if (!userStopped)
                        {
                            int aPos = -1;

                            if (plWnd.HasUserSelTrack)
                            {
                                aPos = plWnd.LastFileIdx;
                                plWnd.LastFileIdx = aPos;
                                //plWnd.HasUserSelTrack = false;
                            }
                            else
                            {
                                CurrentTrackCompleted(this, e);
                                if (!mIsShuffleChecked)
                                {
                                    aPos = plWnd.LastFileIdx + 1;
                                }
                                else
                                {
                                    aPos = randSongsIdx[((shuffleCnt + 1)) % plWnd.GetPlaylistSize()].PosInPlayList;
                                    ++shuffleCnt;
                                    if (shuffleCnt == plWnd.GetPlaylistSize())
                                        shuffleCnt = 0;
                                }
                                plWnd.LastFileIdx = aPos;
                            }

                            if (aPos < plWnd.GetPlaylistSize())
                            {
                                currentAudioFile = plWnd.GetFileToPlay(aPos);
                                mAudioFile = currentAudioFile.FullPath;

                                if (File.Exists(mAudioFile))
                                {
                                    try
                                    {
                                        InitPlayer(true);
                                        plWnd.HasUserSelTrack = false;
                                        InitBgWorker();
                                        StartPlaybackThread();
                                        NextTrackStarted(this, e);
                                        UpdateMarquee();
                                    }
                                    catch (Exception)
                                    {
                                        //throw;
                                    }
                                }
                                //else
                                //    MessageBox.Show(string.Format("File {0} not found!", mAudioFile));
                                //++plWnd.LastFileIdx;                            
                            }
                            else
                            {
                                try
                                {
                                    StopPlayback();
                                }
                                catch (Exception)
                                {
                                    //throw;
                                }
                                plWnd.LastFileIdx = -1;
                                IsPlaylistRunning = false;
                                EnableButtons(false);
                            }
                        }
                    }
                    else
                        isSingleFilePlaying = false;
                }
                else
                {
                    //isWaitingHandle = true;
                    StopPlayback();
                    //StartPlaybackThread();
                    Action action = () =>
                    {
                        if (isSingleFilePlaying)
                            EnableButtons(false);
                        else
                            EnableButtons(true);
                    };
                    Invoke(action);
                }
                ((BackgroundWorker)sender).Dispose();
                GC.Collect();
                //MessageBox.Show("End of playback.");
            }
        }

        private void openTsmItem_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(delegate()
                {
                    bool hasSelFile = SelectInputFile();
                    if (hasSelFile)
                    {                        
                        if (isWaitingHandle)
                        {
                            isWaitingHandle = false;
                            waitHandle.Set();
                        }
                        
                            InitPlayer();
                            InitBgWorker();

                            StartPlaybackThread();

                            isSingleFilePlaying = true;
                            //Action action = () =>
                            //    {
                            //        marqueeLbl.Visible = false;
                            //    };
                            //Invoke(action);
                    }
                });
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }

        private void exitTsmItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void plistWndTsmItem_Click(object sender, EventArgs e)
        {
            if (plistWndTsmItem.Checked && !plWnd.Visible)
                plWnd.Show();
            else if (!plistWndTsmItem.Checked && plWnd.Visible)
                plWnd.Hide();
        }

        private void aboutTsmItem_Click(object sender, EventArgs e)
        {
            Invoke((Action)(() =>
            {
                MessageBox.Show(this, "2018 - by C.G.", rm.GetString("aboutTsmItem.Text", LocalizedForm.GlobalUICulture), MessageBoxButtons.OK);
            }));            
        }

        void plWnd_PlaylistItemDoubleClicked(object sender, EventArgs e)
        {
            if (File.Exists(mAudioFile))
            {
                IsPlaylistRunning = true;
                plWnd.HasUserSelTrack = true;
                chkShuffle.Enabled = true;
                currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                UpdateMarquee();
                if (audioPlayer != null)
                {
                    //audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
                    if (audioPlayer.IsStopped())
                    {
                        if (!isWaitingHandle)
                        {
                            //InitPlayer();
                            //StartPlaybackThread();                            
                            userStopped = false;
                        }                        
                    }
                    if (isSingleFilePlaying)
                        isSingleFilePlaying = false;

                    Action action = () =>
                    {                        
                        btnRew.Enabled = btnFwd.Enabled = true;
                    };
                    Invoke(action);
                }                
            }
            //if (audioPlayer != null)
            //    audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
        }

        void plWnd_CloseButtonClicked(object sender, EventArgs e)
        {
            plistWndTsmItem.Checked = false;
        }

        private void MainWndPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUp();
        }
        
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (mAudioFile == null)
            {
                Thread t = new Thread(delegate()
                {
                    bool hasSelFile = SelectInputFile();
                    if (hasSelFile)
                    {
                        InitPlayer();
                        InitBgWorker();
                        StartPlaybackThread();
                        //if (audioPlayer != null)
                        //    audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                        userStopped = false;
                        isSingleFilePlaying = true;
                    }
                });               
                t.Start();                
            }
            else
            {
                try
                {
                    if (audioPlayer == null)
                    {
                        audioPlayer =
                                new LocalAudioPlayer(mAudioFile, GetSelectedOutputDriver());
                        audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                        //audioPlayer.PlaybackPaused += audioPlayer_PlaybackPaused;
                        audioPlayer.PlaybackResumed += audioPlayer_PlaybackResumed;
                        //audioPlayer.PlaybackStopped += audioPlayer_PlaybackStopped;

                        if (!IsPlaylistRunning)
                            currentAudioFile = new AudioFileInfo(Path.GetFileName(mAudioFile), mAudioFile);

                        Action action = () =>
                            {
                                if (tbVolume.Value != VOL_INIT * 100)
                                    audioPlayer.SetVolume((float)tbVolume.Value / 100f);
                                else
                                    audioPlayer.SetVolume(VOL_INIT);
                            };
                        Invoke(action);

                        StartPlaybackThread();
                    }
                    else
                    {
                        if (aTimer != null && !aTimer.Enabled)
                        {
                            aTimer.Enabled = true;
                            aTimer.Start();
                        }                        
                        StartPlaybackThread();
                        audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;                        
                    }
                }
                catch (Exception)
                {
                }
                userStopped = false;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (mAudioFile != null && audioPlayer != null && audioPlayer.IsPlaying())
            {
                audioPlayer.Pause();               
                //waitHandle.Set();                
                userStopped = false;
                //if (IsPlaylistRunning)
                //    plWnd.HasUserSelTrack = true;
                btnPlay.Enabled = true;
                btnPause.Enabled = false;
            }
        }

        private void btnRew_Click(object sender, EventArgs e)
        {
            if (isSingleFilePlaying)
                return;
            if (audioPlayer != null)
            {
                if (plWnd.LastFileIdx > 0)
                {
                    CurrentTrackCompleted(sender, e);

                    if (bgPlayWorker.IsBusy && isWaitingHandle)
                    {
                        if (!mIsShuffleChecked)
                            plWnd.LastFileIdx -= 2;
                        isWaitingHandle = false;
                        //audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
                        waitHandle.Set();
                        if (File.Exists(mAudioFile))
                            UpdateMarquee();
                    }
                    else
                    {
                        int nextPos = -1;
                        if (!mIsShuffleChecked)
                        {
                            nextPos = plWnd.LastFileIdx - 1;
                        }
                        else
                        {
                            nextPos = randSongsIdx[plWnd.LastFileIdx].PosInPlayList;
                        }
                        if (ResetPlayList(sender, e, nextPos))
                        {
                            IsPlaylistRunning = true;
                            audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                            userStopped = false;
                            currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                            UpdateMarquee();
                        }
                    }
                }
                else
                {
                    if (plWnd.LastFileIdx < 0) return;
                    if (plWnd.LastFileIdx == 0 && mIsShuffleChecked)
                    {
                        CurrentTrackCompleted(sender, e);

                        //plWnd.LastFileIdx = plWnd.GetPlaylistSize() - 1;
                        if (bgPlayWorker.IsBusy && isWaitingHandle)
                        {
                            isWaitingHandle = false;
                            //audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
                            waitHandle.Set();
                            if (File.Exists(mAudioFile))
                                UpdateMarquee();
                        }
                        else
                        {
                            if (ResetPlayList(sender, e, plWnd.GetPlaylistSize() - 1))
                            {
                                IsPlaylistRunning = true;
                                audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                                userStopped = false;
                                currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                                UpdateMarquee();
                            }
                        }
                    }
                }
            }
            else
            {
                int aPos = -1;
                if (!mIsShuffleChecked)
                    aPos = 0;
                else
                    aPos = randSongsIdx[0].PosInPlayList;
                if (ResetPlayList(sender, e, aPos))
                {
                    IsPlaylistRunning = true;
                    audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                    userStopped = false;
                    currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                    UpdateMarquee();
                }
            }
        }

        private void btnFwd_Click(object sender, EventArgs e)
        {
            if (isSingleFilePlaying)
                return;
            if (audioPlayer != null)
            {
                if (plWnd.LastFileIdx < plWnd.GetPlaylistSize() - 1)
                {
                    CurrentTrackCompleted(sender, e);

                    if (bgPlayWorker.IsBusy && isWaitingHandle)
                    {
                        isWaitingHandle = false;
                        waitHandle.Set();
                        //if (File.Exists(mAudioFile))
                        //    UpdateMarquee();
                    }
                    else
                    {
                        int nextPos = -1;
                        if (!mIsShuffleChecked)
                        {
                            nextPos = plWnd.LastFileIdx + 1;
                        }
                        else
                        {
                            if (plWnd.LastFileIdx < 0)
                                plWnd.LastFileIdx = 0;
                            nextPos = randSongsIdx[plWnd.LastFileIdx].PosInPlayList;
                        }
                        if (ResetPlayList(sender, e, nextPos))
                        {
                            IsPlaylistRunning = true;
                            audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                            userStopped = false;
                            currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                            UpdateMarquee();
                        }
                    }
                }
                else
                {
                    if (plWnd.LastFileIdx == plWnd.GetPlaylistSize() - 1 && mIsShuffleChecked)
                    {
                        CurrentTrackCompleted(sender, e);

                        //plWnd.LastFileIdx = plWnd.GetPlaylistSize() - 1;
                        if (bgPlayWorker.IsBusy && isWaitingHandle)
                        {
                            isWaitingHandle = false;
                            //audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
                            waitHandle.Set();
                            //if (File.Exists(mAudioFile))
                            //    UpdateMarquee();
                        }
                        else
                        {
                            if (ResetPlayList(sender, e, 0))
                            {
                                IsPlaylistRunning = true;
                                audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                                userStopped = false;
                                currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                                UpdateMarquee();
                            }
                        }
                    }
                }
            }
            else
            {
                int aPos = -1;
                if (!mIsShuffleChecked)
                    aPos = 0;
                else
                    aPos = randSongsIdx[0].PosInPlayList;
                if (ResetPlayList(sender, e, aPos))
                {
                    IsPlaylistRunning = true;
                    audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                    userStopped = false;
                    currentAudioFile = plWnd.GetFileToPlay(plWnd.LastFileIdx);
                    UpdateMarquee();
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (mAudioFile != null)
            {
                StopPlayback();
                //waitHandle.Set();
                if (audioPlayer != null)
                    audioPlayer.PlaybackStopType = PlaybackStopTypes.PlaybackStoppedByUser;
                userStopped = true;
                //if (IsPlaylistRunning)
                //    plWnd.HasUserSelTrack = true;
                EnableButtons(false);
            }
        }

        private void tbVolume_ValueChanged(object sender, EventArgs e)
        {
            if (audioPlayer != null)
            {
                audioPlayer.SetVolume((float)tbVolume.Value / 100f);
            }
        }

        private void tbAudioPos_ValueChanged(object sender, EventArgs e)
        {
            userTbAudioUpdating = true;
        }

        private void tbAudioPos_MouseUp(object sender, MouseEventArgs e)
        {
            if (userTbAudioUpdating)
            {
                int newVal = tbAudioPos.Value;
                userTbAudioUpdating = false;

                UpdateAudioPos(newVal);
            }
            mUpdateAudioPos = true;
        }

        private void tbAudioPos_MouseDown(object sender, MouseEventArgs e)
        {
            mUpdateAudioPos = false;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Action updateTbPos;
            TimeSpan ts = audioPlayer.GetCurrentTime();
            string ctimeStr = Utils.FormatTimeSpan(ts);
            Action action = () => lblNowTime.Text = ctimeStr;
            lblNowTime.Invoke(action);
            if (mUpdateAudioPos)
            {
                updateTbPos = () =>
                {
                    tbAudioPos.Value = (int)ts.Minutes * 60 + ts.Seconds;
                };

                tbAudioPos.Invoke(updateTbPos);
            }
            action = () =>
            {
                if (audioPlayer.IsStopped() && audioPlayer.PlaybackStopType == PlaybackStopTypes.PlaybackStoppedReachingEndOfFile
                    /*Math.Abs(audioPlayer.GetTotalTime().TotalSeconds - ts.TotalSeconds) < 0.20d */
                    )
                {
                    if (isWaitingHandle)
                    {
                        if (IsPlaylistRunning && plWnd.HasUserSelTrack)
                        {
                            plWnd.HasUserSelTrack = false;
                        }
                        isWaitingHandle = false;
                        waitHandle.Set();
                    }
                    if (aTimer != null)
                    {
                        aTimer.Stop();
                        aTimer.Enabled = false;
                    }
                    tbAudioPos.Value = 0;
                }
            };
            Invoke(action);
        }

        private void MainWndPlayer_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                if (plWnd.Visible)
                    plWnd.Hide();
            }
            if (WindowState == FormWindowState.Maximized)
            {
                if (!plWnd.Visible)
                    plWnd.Show(this);
            }
        }

        private void langEngTsmItem_Click(object sender, EventArgs e)
        {
            if (langEngTsmItem.Checked == true)
            {
                langEngTsmItem.Checked = false;
                langItaTsmItem.Checked = true;
            }
            else
            {
                langEngTsmItem.Checked = true;
                langItaTsmItem.Checked = false;
            }
            switchToLang();
        }

        private void langItaTsmItem_Click(object sender, EventArgs e)
        {
            if (langItaTsmItem.Checked == true)      //in italian, switch to default language
            {
                langItaTsmItem.Checked = false;
                langEngTsmItem.Checked = true;
            }
            else
            {
                langItaTsmItem.Checked = true;
                langEngTsmItem.Checked = false;
            }
            switchToLang();
        }

        private void chkShuffle_CheckedChanged(object sender, EventArgs e)
        {
            mIsShuffleChecked = chkShuffle.Checked;
        }

        void plWnd_PlaylistCleared(object sender, EventArgs e)
        {
            Action action = () =>
            {
                btnRew.Enabled = false;
                btnFwd.Enabled = false;
                //chkRepeat.Enabled = false;
                chkShuffle.Enabled = false;
                //marqueeLbl.Visible = false;
                if (chkShuffle.Checked)
                    chkShuffle.Checked = !chkShuffle.Checked;
            };
            Invoke(action);
        }

        void plWnd_PlaylistLoaded(object sender, EventArgs e)
        {
            plWnd.LastFileIdx = -1;  //0
            Action action = () =>
            {
                bool btnState = false;
                if (plWnd.GetPlaylistSize() > 0)
                    btnState = true;
                btnRew.Enabled = btnState;
                btnFwd.Enabled = btnState;
                //chkRepeat.Enabled = btnState;
                chkShuffle.Enabled = btnState;
                //marqueeLbl.Visible = btnState;
            };
            Invoke(action);
            if (plWnd.GetPlaylistSize() > 0)
            {
                randSongsIdx = PlayListShuffler.ComputedRandomOrder(plWnd.PlayListItems);
                shuffleCnt = 0;
                //int cnt = 0;
                //foreach (PlayListEntry sngIdx in randSongsIdx)
                //{
                //    ++cnt;
                //    Debug.WriteLine(string.Format("-- {0}. {1} - {2} (has played = {3}", 
                //        cnt, sngIdx.PosInPlayList, sngIdx.FileName, sngIdx.HasPlayedOnce));
                //}
            }
        }

        #endregion
                
        #region External Event Handlers

        public event EventHandler CurrentTrackCompleted;        
        protected virtual void OnCurrentTrackCompleted(EventArgs e)
        {
            var handler = CurrentTrackCompleted;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler NextTrackStarted;
        protected virtual void OnNextTrackStarted(EventArgs e)
        {
            var handler = NextTrackStarted;
            if (handler != null)
                handler(this, e);
        }

        #endregion
        
    }
}
