using GoStreamAudioLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public class MarqueeLabel : Label
    {
        public Timer MarqueeTimer { get; set; }
        public int Speed { get; set; }
        public int yOffset { get; set; }

        public void Start() { MarqueeTimer.Start(); }
        public void Stop() { MarqueeTimer.Stop(); }

        private int offset;
        SolidBrush backBrush;
        SolidBrush textBrush;

        private string filePlayingFullPath;
        private LocalAudioPlayer audioPlayer;

        #region Public Properties

        /// <summary>
        /// the full path of the file which is currently playing 
        /// </summary>
        public string FilePlayingFullPath 
        {
            get
            {
                return filePlayingFullPath;
            }
            set
            {
                filePlayingFullPath = value;
            }
        }

        public LocalAudioPlayer AudioPlayer
        {
            get
            {
                return audioPlayer;
            }
            set
            {
                audioPlayer = value;
            }
        }

        #endregion

        public MarqueeLabel()
        {
            this.DoubleClick += new System.EventHandler(this.MarqueeLabel_DoubleClick);
            textBrush = new SolidBrush(this.ForeColor);
            backBrush = new SolidBrush(this.BackColor);
            yOffset = 0;
            Speed = 1;
            MarqueeTimer = new Timer();
            MarqueeTimer.Interval = 25;
            MarqueeTimer.Enabled = true;
            MarqueeTimer.Tick += (aSender, eArgs) =>
            {
                offset = (offset - Speed);
                if (offset < -this.ClientSize.Width) offset = 0;
                this.Invalidate();
            };
        }

        public void ResetPosition(int w)
        {
            offset = w;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(backBrush, e.ClipRectangle);
            e.Graphics.DrawString(this.Text, this.Font, textBrush, offset, yOffset);
            e.Graphics.DrawString(this.Text, this.Font, textBrush,
                                  this.ClientSize.Width + offset, yOffset);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MarqueeLabel
            //             
            this.ResumeLayout(false);
        }

        private void MarqueeLabel_DoubleClick(object sender, EventArgs e)
        {
            if (this.filePlayingFullPath != null 
                && this.filePlayingFullPath != "" 
                && this.filePlayingFullPath.ToLowerInvariant().EndsWith(".mp3"))
            {
                try
                {
                    Mp3TagManager tagMan = new Mp3TagManager();
                    DialogResult dr = tagMan.LoadTagInfo("Edit Mp3 Tag", this.filePlayingFullPath, this.audioPlayer);
                    if (dr == DialogResult.Cancel)
                    {
                        tagMan.CleanUp();
                        tagMan.Close();
                        tagMan.Dispose();
                    }
                    else if (dr == DialogResult.OK)
                    {
                        tagMan.CleanUp();
                        tagMan.Close();
                        tagMan.Dispose();
                    }

                    //Mp3TagManager.ShowDialog("Edit Mp3 Tag", this.filePlayingFullPath, this.audioPlayer);

                    //UltraID3 u = new UltraID3();
                    //u.Read(this.filePlayingFullPath);

                    //Action action = () => MessageBox.Show(u.Artist + " - " + u.Title);
                    //Invoke(action);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error reading tag info: {0}", ex.Message));
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (MarqueeTimer != null)
            {
                MarqueeTimer.Dispose();
                MarqueeTimer = null;
            }
            this.DoubleClick -= this.MarqueeLabel_DoubleClick;
            base.Dispose(disposing);
        }
    }
}
