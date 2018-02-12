using GoStreamAudioLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public class MarqueeLabel : Label
    {
        public System.Windows.Forms.Timer MarqueeTimer { get; set; }
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

        #region Constructor
        
        public MarqueeLabel()
        {
            this.DoubleClick += new System.EventHandler(this.MarqueeLabel_DoubleClick);
            textBrush = new SolidBrush(this.ForeColor);
            backBrush = new SolidBrush(this.BackColor);
            yOffset = 0;
            Speed = 1;
            MarqueeTimer = new System.Windows.Forms.Timer();
            MarqueeTimer.Interval = 25;
            MarqueeTimer.Enabled = true;
            MarqueeTimer.Tick += (aSender, eArgs) =>
            {
                offset = (offset - Speed);
                if (offset < -this.ClientSize.Width) offset = 0;
                this.Invalidate();
            };
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// resets horizontal scrolling position
        /// </summary>
        /// <param name="w"></param>
        public void ResetPosition(int w)
        {
            offset = w;
        }

        #endregion

        #region Protected Methods
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(backBrush, e.ClipRectangle);
            e.Graphics.DrawString(this.Text, this.Font, textBrush, offset, yOffset);
            e.Graphics.DrawString(this.Text, this.Font, textBrush,
                                  this.ClientSize.Width + offset, yOffset);
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

        #endregion

        #region Private Methods

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
            Thread t = new Thread(delegate()
            {
                if (this.filePlayingFullPath != null
                    && this.filePlayingFullPath != "")
                {
                    if (this.filePlayingFullPath.ToLowerInvariant().EndsWith(".mp3")
                        || this.filePlayingFullPath.ToLowerInvariant().EndsWith(".ogg")
                        || this.filePlayingFullPath.ToLowerInvariant().EndsWith(".flac"))
                    {
                        try
                        {
                            Invoke((Action)(() =>
                            {
                                TagEditor tEditor = new TagEditor();
                                tEditor.LoadTagInfo(this.filePlayingFullPath, this.audioPlayer,
                                    this.filePlayingFullPath.ToLowerInvariant().EndsWith(".mp3") == true ? true : false);
                                DialogResult dr = tEditor.ShowDialog(this);
                                tEditor.CleanUp();
                                tEditor.Dispose();
                            }));
                        }
                        catch (Exception ex)
                        {
                            Invoke((Action)(() =>
                            {
                                MessageBox.Show(string.Format("Error reading tag info: {0}", ex.Message));
                            }));
                        }
                    }
                }
            });
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }

        #endregion

    }
}
