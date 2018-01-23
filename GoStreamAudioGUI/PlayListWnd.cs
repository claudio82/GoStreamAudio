using GoStreamAudioLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public partial class PlayListWnd : LocalizedForm
    {
        private MainWndPlayer parent;
        private int lastFileIdx = 0;
        //private int rClkFileSelIdx = 0;
        
        private M3UFile _m3uFile;
        private bool hasUserSelTrack = false;

        private System.Resources.ResourceManager rm;
        //private double totPlaylistTime = 0;
        //public ICollection<IAudioFileInspector> Inspectors { get; private set; }
        
        ListView l = new ListView();

        #region Public Properties

        /// <summary>
        /// the current file index in the playlist
        /// </summary>
        public int LastFileIdx
        {
            get
            {
                return lastFileIdx;
            }
            set
            {
                this.lastFileIdx = value;
            }
        }

        public bool HasUserSelTrack
        {
            get
            {
                return hasUserSelTrack;
            }
            set
            {
                this.hasUserSelTrack = value;
            }
        }

        public System.Windows.Forms.ListView.ListViewItemCollection PlayListItems
        {
            get
            {
                return this.l.Items;
            }
        }

        #endregion

        public PlayListWnd(MainWndPlayer parent)
        {
            InitializeComponent();
            rm = new System.Resources.ResourceManager(typeof(GoStreamAudioGUI.PlayListWnd));
            lblPlistInfo.Visible = false;

            this.Left = parent.Location.X + parent.Size.Width - 15;
            this.Top = parent.Location.Y;
            this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.ShowInTaskbar = false;
            this.parent = parent;
            this.parent.CurrentTrackCompleted += parent_CurrentTrackCompleted;
            this.parent.NextTrackStarted += parent_NextTrackStarted;           

            this.btnClear.Enabled = this.btnSave.Enabled = false;

            //ColumnHeader cHeader = new ColumnHeader();
            cHeader0.Name = "cHeader0";
            //cHeader.Text = rm.GetString("lvColHeader0");
            cHeader0.Width = (int)(this.Width * 3) / 4;
            l.Columns.Add(cHeader0);
            l.Columns.Add("");
            l.MouseDoubleClick += new MouseEventHandler(l_MouseDoubleClick);
            l.Scrollable = true;
            l.View = View.Details;
            l.FullRowSelect = true;
            l.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //l.Dock = DockStyle.Fill;
            l.Width = this.Width - 15;
            l.Height = this.Height - 90;
            l.Columns[1].TextAlign = HorizontalAlignment.Right;
            l.Columns[1].Width = 0;
            l.SuspendLayout();
            this.Controls.Add(l);

            //Inspectors = new List<IAudioFileInspector>();
            //Inspectors.Add(new Mp3FileInspector());            

            hasUserSelTrack = false;
        }
                        
        public AudioFileInfo GetFileToPlay(int idx)
        {
            AudioFileInfo afInfo;
            if (idx < 0)
                return null;
            else
            {
                ListViewItem item = null;
                Action action = () =>
                    {
                        item = l.Items[idx];
                    };
                Invoke(action);
                if (item != null)
                {
                    afInfo = GetAudioFileInfo(item);
                    if (afInfo != null)
                    {
                        return afInfo;
                    }
                    else
                        return null;
                }
                return null;
            }
        }

        public int GetPlaylistSize()
        {
            return l.Items.Count;
        }

        /// <summary>
        /// choose the next file to play by index
        /// </summary>
        /// <returns>true if the file must play, false otherwise</returns>
        public bool GetNextFileFromPlayList(/*out string nextFileToPlay*/)
        {
            string nFile = "";
            if (lastFileIdx < l.Items.Count)
            {
                Action action = () =>
                {
                    nFile = l.Items[lastFileIdx].Text;
                    AudioFileInfo afInfo = GetAudioFileInfo(l.Items[lastFileIdx]);
                    parent.AudioFile = afInfo.FullPath;
                };
                Invoke(action);
                return true;
            }
            else
            {
                Action action = () => lastFileIdx = l.Items.Count - 1;
                Invoke(action);
                return false;
            }
        }

        /// <summary>
        /// loads all available audio files inside the control
        /// </summary>
        private void LoadAudioFilesIntoPlWnd()
        {            
            List<AudioFileInfo> filesList = new List<AudioFileInfo>();
            using (FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog())
            {
                string folderPath = "";                                
                //folderBrowserDialog1 = new FolderBrowserDialog();
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Thread t = new Thread(delegate()
                    {
                        folderPath = folderBrowserDialog1.SelectedPath;
                        var allFiles = DirSearch(folderPath);
                        foreach (string af in allFiles)
                        {
                            AudioFileInfo afInfo = new AudioFileInfo(Path.GetFileName(af), af);
                            //afInfo.SetDuration();
                            filesList.Add(afInfo);  //, TimeSpan.Zero));
                        }

                        Action action;
                        action = () =>
                        {
                            if (l.Items.Count > 0)
                                l.Items.Clear();

                            //l.Invoke(action);
                            int cnt = 0;
                            foreach (AudioFileInfo item in filesList)
                            {
                                ++cnt;

                                //add items to ListView
                                ListViewItem itm = new ListViewItem(new[] { string.Format("{0}. {1}", cnt.ToString("D3"), item.FileName), ""/*Utils.FormatTimeSpan2(item.FileLength)*/, item.FullPath });
                                l.Items.Add(itm);
                                //action = () => l.Items.Add(itm);
                                //l.Invoke(action);
                            }
                            if (l.Items.Count > 0)
                            {
                                l.AutoResizeColumn(l.Columns.Count - 1, ColumnHeaderAutoResizeStyle.ColumnContent);
                                UpdateButtons();
                                parent.IsPlaylistRunning = true;
                                lastFileIdx = -1;

                                lblPlistInfo.Text = string.Format("{0} files.",
                                    l.Items.Count);
                                lblPlistInfo.Visible = true;

                                this.btnClear.Enabled = true;
                                this.btnSave.Enabled = true;

                                //PlaylistLoaded(this, null);

                                //lblPlistInfo.Text += string.Format(", Tot. Time: {0}:{1}", 
                                //    (int)totPlaylistTime/60, (int)totPlaylistTime % 60);
                                //StartPlaying(l.Items[0].Text, 0);
                            }
                            else
                            {
                                lblPlistInfo.Text = string.Format("{0} files.", 0);
                            }
                            PlaylistLoaded(this, null);
                        };
                        Invoke(action);
                    });
                    t.Start();
                }
            }
            
        }

        private List<String> DirSearch(string sDir)
        {
            string[] allowExt = { ".mp3", ".m4a", ".wav", ".ogg", ".wma", ".flac" }; //, ".aiff", ".wma" };
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    foreach (string fExt in allowExt)
                        if (f.ToLowerInvariant().EndsWith(fExt))
                        {
                            files.Add(f);
                            break;
                        }
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            return files;
        }

        private void UpdateButtons()
        {
            Action action = () =>
            {
                if (l.Items.Count > 0)
                {
                    btnClear.Enabled = true;
                    btnSave.Enabled = true;
                }
                else
                {
                    btnClear.Enabled = false;
                    btnSave.Enabled = false;
                }
            };
            Invoke(action);
        }

        /// <summary>
        /// returns the total length of audio file
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetAudioFileTime()
        {
            return parent.AudioPlayer.GetTotalTime();
        }

        /// <summary>
        /// print audio file information (form mp3 only)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string DescribeFile(string fileName)
        {
            /*
            if (fileName != string.Empty && fileName.ToLowerInvariant().EndsWith(".mp3"))
            {
                Mp3FileInspector mp3FileInsp = (Mp3FileInspector)Inspectors.ElementAt(0);
                if (mp3FileInsp != null)
                {
                    return mp3FileInsp.Describe(fileName);
                }
            }*/
            return "";
        }

        /// <summary>
        /// load a playlist from a M3U file
        /// </summary>
        private void LoadPlaylistFromFile()
        {
            string lblAudioFileType = rm.GetString("plAudioFilterType");
            using (var ofd = new OpenFileDialog 
                { Filter = lblAudioFileType + " (*.m3u)|*.m3u", Title = rm.GetString("openPlDlgTitle") })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _m3uFile = null;

                    try
                    {
                        _m3uFile = new M3UFile();
                        _m3uFile.Load(ofd.FileName);
                        //parent.IsPlaylistRunning = true;
                    }
                    catch (Exception)
                    {
                        //parent.IsPlaylistRunning = false;
                        MessageBox.Show("Error loading playlist from file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    PopulateEntries();
                    if (l.Items.Count > 0)
                    {
                        parent.IsPlaylistRunning = true;
                        lastFileIdx = -1;

                        lblPlistInfo.Text = string.Format("{0} files.",
                            l.Items.Count);
                        lblPlistInfo.Visible = true;
                        this.btnClear.Enabled = true;
                        this.btnSave.Enabled = true;

                        //PlaylistLoaded(this, null);
                        //lblPlistInfo.Text += string.Format(", Total Time: {0}:{1}", 
                        //    (int)totPlaylistTime / 60, (int)totPlaylistTime % 60);
                        //StartPlaying(l.Items[0].Text, 0);
                    }
                    else
                    {
                        lblPlistInfo.Text = string.Format("{0} files.", 0);
                    }
                    PlaylistLoaded(this, null);
                }
            }
        }

        private void PopulateEntries()
        {
            l.Items.Clear();

            if (_m3uFile == null)
                return;

            foreach (var entry in _m3uFile)
            {
                UpdateEntryItem(entry);
            }

            l.AutoResizeColumn(l.Columns.Count - 1, ColumnHeaderAutoResizeStyle.ColumnContent);
            UpdateButtons();

        }

        private void UpdateEntryItem(M3UEntry entry, int index = -1)
        {
            if (index == -1)
                index = l.Items.Count;

            var lvi = new ListViewItem(new[]
            {
                entry.Title,
                "", //string.Format("{0:D2}:{1:D2}:{2:D2}", entry.Duration.Hours, entry.Duration.Minutes, entry.Duration.Seconds),
                entry.Path.IsFile ? entry.Path.LocalPath : entry.Path.ToString()
            });

            if (index > l.Items.Count - 1)
                l.Items.Add(lvi);
            else
                l.Items[index] = lvi;
        }

        /// <summary>
        /// save current playlist as a M3U file
        /// </summary>
        private void SaveCurrentPlToFile()
        {
            string lblAudioFileType = rm.GetString("plAudioFilterType");
            using (var sfd = new SaveFileDialog 
                { Filter = lblAudioFileType + " (*.m3u)|*.m3u", Title = rm.GetString("savePlDlgTitle") })
            {
                sfd.InitialDirectory = "";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _m3uFile = null;

                    try
                    {
                        _m3uFile = new M3UFile();
                        foreach (ListViewItem lvItem in l.Items)
                        {
                            /*AudioFileInfo afInfo = new AudioFileInfo(
                                Path.GetFileName(lvItem.SubItems[2].Text), lvItem.SubItems[2].Text);
                            afInfo.SetDuration();*/                            

                            M3UEntry _m3uEntry = new M3UEntry(TimeSpan.Zero, 
                                lvItem.SubItems[0].Text, 
                                new Uri(lvItem.SubItems[2].Text));
                            _m3uFile.Add(_m3uEntry);
                        }
                        // save files into playlist file
                        _m3uFile.Save(sfd.FileName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error saving playlist to file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public bool StartPlaying(string fName, int plPos)
        {
            bool canPlay = false;
            parent.EnableButtons(false);
            if (parent.IsWaitingHandle)
            {
                parent.IsWaitingHandle = false;
                parent.WaitHandle.Set();
            }
            if (File.Exists(fName))
            {
                parent.AudioFile = fName;
                parent.InitPlayer();
                parent.InitBgWorker();
                parent.StartPlaybackThread();
                canPlay = true;
            }
            else
            {
                MessageBox.Show(string.Format("File {0} not found!", fName));
            }
            return canPlay;
        }

        private AudioFileInfo GetAudioFileInfo(ListViewItem item)
        {
            AudioFileInfo afInfo = null;
            if (item.Text != "")
            {
                afInfo = new AudioFileInfo(item.SubItems[0].Text, item.SubItems[2].Text); //, TimeSpan.Zero);
            }
            return afInfo;
        }

        void parent_CurrentTrackCompleted(object sender, EventArgs e)
        {
            if (l.Items.Count > 0 && lastFileIdx >= 0)
            {
                Action action = () =>
                    {
                        l.Items[lastFileIdx].ForeColor = l.Items[lastFileIdx].SubItems[1].ForeColor = Color.Black;
                    };
                Invoke(action);
            }
        }

        void parent_NextTrackStarted(object sender, EventArgs e)
        {
            if (l.Items.Count > 0 && lastFileIdx >= 0)
            {
                Action action = () =>
                    {
                        ListViewItem item = l.Items[lastFileIdx];
                        item.ForeColor = item.SubItems[1].ForeColor = Color.Blue;
                        if (item.SubItems[1].Text == "")
                        {
                            item.SubItems[1].Text = Utils.FormatTimeSpan2(parent.AudioPlayer.GetTotalTime());
                            l.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    };
                Invoke(action);
            }
        }

        //private void l_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        //{
        //    // This is the default text alignment
        //    TextFormatFlags flags = TextFormatFlags.Right;

        //    e.DrawText(flags);
        //}

        //// Handle DrawColumnHeader event
        //private void l_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        //{
        //    // Draw the column header normally
        //    e.DrawDefault = true;
        //    e.DrawBackground();
        //    e.DrawText();
        //}

        #region Event Handlers

        public event EventHandler CloseButtonClicked;
        protected virtual void OnCloseButtonClicked(EventArgs e)
        {
            var handler = CloseButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler PlaylistItemDoubleClicked;
        protected virtual void OnPlaylistItemDoubleClicked(EventArgs e)
        {
            var handler = PlaylistItemDoubleClicked;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler PlaylistLoaded;
        protected virtual void OnPlaylistLoaded(EventArgs e)
        {
            var handler = PlaylistLoaded;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler PlaylistCleared;
        protected virtual void OnPlaylistCleared(EventArgs e)
        {
            var handler = PlaylistCleared;
            if (handler != null)
                handler(this, e);
        }

        private void l_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = l.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                if (lastFileIdx >= 0)
                    l.Items[lastFileIdx].ForeColor = l.Items[lastFileIdx].SubItems[1].ForeColor = Color.Black;
                hasUserSelTrack = true;
               
                lastFileIdx = item.Index;
                parent.UserStopped = false;
                AudioFileInfo afInfo = GetAudioFileInfo(item);
                if (StartPlaying(afInfo.FullPath, lastFileIdx))
                {
                    item.ForeColor = item.SubItems[1].ForeColor = Color.Blue;
                    parent.IsPlaylistRunning = true;

                    if (parent.AudioPlayer != null)
                    {
                        if (l.Items[lastFileIdx].SubItems[1].Text == "")
                        {
                            l.Items[lastFileIdx].SubItems[1].Text = Utils.FormatTimeSpan2(parent.AudioPlayer.GetTotalTime());
                            l.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    }

                    //MessageBox.Show("The selected Item Name is: " + item.Text);
                    OnPlaylistItemDoubleClicked(e);
                }
            }
            else
            {
                this.l.SelectedItems.Clear();
                MessageBox.Show("No Item is selected");
            }
        }
        /*
        void l_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (l.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ListViewHitTestInfo info = l.HitTest(e.X, e.Y);
                    rClkFileSelIdx = info.Item.Index;
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }*/

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadPlaylistFromFile();
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            LoadAudioFilesIntoPlWnd();
            //Thread t = new Thread(delegate()
            //    {
            //        LoadAudioFilesIntoPlayList();
            //    });
            //t.SetApartmentState(ApartmentState.STA);
            //t.Priority = ThreadPriority.Lowest;
            //t.Start();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (l.Items.Count > 0)
                SaveCurrentPlToFile();
        }
        
        private void PlayListWnd_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                OnCloseButtonClicked(e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (l.Items.Count > 0)
            {
                l.Items.Clear();
                
                btnClear.Enabled = false;
                btnSave.Enabled = false;
                lastFileIdx = -1;

                parent.IsPlaylistRunning = false;

                lblPlistInfo.Text = "";
                this.btnClear.Enabled = this.btnSave.Enabled = false;
            }
            OnPlaylistCleared(e);
        }

        //private void audioPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (l.Items[rClkFileSelIdx].Text.ToLowerInvariant().EndsWith(".mp3"))
        //        MessageBox.Show(DescribeFile(l.Items[rClkFileSelIdx].Text));
        //}

        #endregion

    }
}
