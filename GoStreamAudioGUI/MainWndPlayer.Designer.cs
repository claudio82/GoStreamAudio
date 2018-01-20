namespace GoStreamAudioGUI
{
    partial class MainWndPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWndPlayer));
            this.menuStrip1 = new GoStreamAudioGUI.AppMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plistWndTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.langTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.langEngTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.langItaTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.lblNowTime = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.comboBoxOutputDriver = new System.Windows.Forms.ComboBox();
            this.lblAudioDriver = new System.Windows.Forms.Label();
            this.bgPlayWorker = new System.ComponentModel.BackgroundWorker();
            this.tbVolume = new System.Windows.Forms.TrackBar();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblSep = new System.Windows.Forms.Label();
            this.btnRew = new System.Windows.Forms.Button();
            this.btnFwd = new System.Windows.Forms.Button();
            this.tbAudioPos = new System.Windows.Forms.TrackBar();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.marqueeLbl = new GoStreamAudioGUI.MarqueeLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAudioPos)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTsmItem,
            this.exitTsmItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openTsmItem
            // 
            this.openTsmItem.Name = "openTsmItem";
            resources.ApplyResources(this.openTsmItem, "openTsmItem");
            this.openTsmItem.Click += new System.EventHandler(this.openTsmItem_Click);
            // 
            // exitTsmItem
            // 
            this.exitTsmItem.Name = "exitTsmItem";
            resources.ApplyResources(this.exitTsmItem, "exitTsmItem");
            this.exitTsmItem.Click += new System.EventHandler(this.exitTsmItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plistWndTsmItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // plistWndTsmItem
            // 
            this.plistWndTsmItem.Checked = true;
            this.plistWndTsmItem.CheckOnClick = true;
            this.plistWndTsmItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.plistWndTsmItem.Name = "plistWndTsmItem";
            resources.ApplyResources(this.plistWndTsmItem, "plistWndTsmItem");
            this.plistWndTsmItem.Click += new System.EventHandler(this.plistWndTsmItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.langTsmItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // langTsmItem
            // 
            this.langTsmItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.langEngTsmItem,
            this.langItaTsmItem});
            this.langTsmItem.Name = "langTsmItem";
            resources.ApplyResources(this.langTsmItem, "langTsmItem");
            // 
            // langEngTsmItem
            // 
            this.langEngTsmItem.Checked = true;
            this.langEngTsmItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.langEngTsmItem.Name = "langEngTsmItem";
            resources.ApplyResources(this.langEngTsmItem, "langEngTsmItem");
            this.langEngTsmItem.Click += new System.EventHandler(this.langEngTsmItem_Click);
            // 
            // langItaTsmItem
            // 
            this.langItaTsmItem.Name = "langItaTsmItem";
            resources.ApplyResources(this.langItaTsmItem, "langItaTsmItem");
            this.langItaTsmItem.Click += new System.EventHandler(this.langItaTsmItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutTsmItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutTsmItem
            // 
            this.aboutTsmItem.Name = "aboutTsmItem";
            resources.ApplyResources(this.aboutTsmItem, "aboutTsmItem");
            this.aboutTsmItem.Click += new System.EventHandler(this.aboutTsmItem_Click);
            // 
            // openSFileDlg
            // 
            resources.ApplyResources(this.openSFileDlg, "openSFileDlg");
            // 
            // lblNowTime
            // 
            resources.ApplyResources(this.lblNowTime, "lblNowTime");
            this.lblNowTime.Name = "lblNowTime";
            // 
            // lblTotalTime
            // 
            resources.ApplyResources(this.lblTotalTime, "lblTotalTime");
            this.lblTotalTime.Name = "lblTotalTime";
            // 
            // comboBoxOutputDriver
            // 
            this.comboBoxOutputDriver.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxOutputDriver, "comboBoxOutputDriver");
            this.comboBoxOutputDriver.Name = "comboBoxOutputDriver";
            // 
            // lblAudioDriver
            // 
            resources.ApplyResources(this.lblAudioDriver, "lblAudioDriver");
            this.lblAudioDriver.Name = "lblAudioDriver";
            // 
            // bgPlayWorker
            // 
            this.bgPlayWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgPlayWorker_DoWork);
            this.bgPlayWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgPlayWorker_RunWorkerCompleted);
            // 
            // tbVolume
            // 
            resources.ApplyResources(this.tbVolume, "tbVolume");
            this.tbVolume.Maximum = 100;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.SmallChange = 5;
            this.tbVolume.TickFrequency = 10;
            this.tbVolume.Value = 50;
            this.tbVolume.ValueChanged += new System.EventHandler(this.tbVolume_ValueChanged);
            // 
            // lblVolume
            // 
            resources.ApplyResources(this.lblVolume, "lblVolume");
            this.lblVolume.Name = "lblVolume";
            // 
            // lblSep
            // 
            resources.ApplyResources(this.lblSep, "lblSep");
            this.lblSep.Name = "lblSep";
            // 
            // btnRew
            // 
            resources.ApplyResources(this.btnRew, "btnRew");
            this.btnRew.Image = global::GoStreamAudioGUI.Properties.Resources.prev;
            this.btnRew.Name = "btnRew";
            this.btnRew.UseVisualStyleBackColor = true;
            this.btnRew.Click += new System.EventHandler(this.btnRew_Click);
            // 
            // btnFwd
            // 
            resources.ApplyResources(this.btnFwd, "btnFwd");
            this.btnFwd.Image = global::GoStreamAudioGUI.Properties.Resources.next;
            this.btnFwd.Name = "btnFwd";
            this.btnFwd.UseVisualStyleBackColor = true;
            this.btnFwd.Click += new System.EventHandler(this.btnFwd_Click);
            // 
            // tbAudioPos
            // 
            resources.ApplyResources(this.tbAudioPos, "tbAudioPos");
            this.tbAudioPos.Maximum = 100;
            this.tbAudioPos.Name = "tbAudioPos";
            this.tbAudioPos.SmallChange = 5;
            this.tbAudioPos.TickFrequency = 10;
            this.tbAudioPos.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbAudioPos.ValueChanged += new System.EventHandler(this.tbAudioPos_ValueChanged);
            this.tbAudioPos.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbAudioPos_MouseDown);
            this.tbAudioPos.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbAudioPos_MouseUp);
            // 
            // btnPause
            // 
            resources.ApplyResources(this.btnPause, "btnPause");
            this.btnPause.Image = global::GoStreamAudioGUI.Properties.Resources.pause;
            this.btnPause.Name = "btnPause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::GoStreamAudioGUI.Properties.Resources.stop;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Name = "btnStop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::GoStreamAudioGUI.Properties.Resources.play;
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // marqueeLbl
            // 
            resources.ApplyResources(this.marqueeLbl, "marqueeLbl");
            this.marqueeLbl.Name = "marqueeLbl";
            this.marqueeLbl.Speed = 1;
            this.marqueeLbl.yOffset = 0;
            // 
            // MainWndPlayer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.marqueeLbl);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.tbAudioPos);
            this.Controls.Add(this.btnFwd);
            this.Controls.Add(this.btnRew);
            this.Controls.Add(this.lblSep);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.tbVolume);
            this.Controls.Add(this.lblAudioDriver);
            this.Controls.Add(this.comboBoxOutputDriver);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.lblNowTime);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWndPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWndPlayer_FormClosing);
            this.Resize += new System.EventHandler(this.MainWndPlayer_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAudioPos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Public Properties

        public System.Windows.Forms.OpenFileDialog OpenSFileDlg
        {
            get
            {
                return openSFileDlg;
            }
        }

        #endregion

        private AppMenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTsmItem;
        private System.Windows.Forms.ToolStripMenuItem exitTsmItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutTsmItem;
        private System.Windows.Forms.OpenFileDialog openSFileDlg;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblNowTime;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.ComboBox comboBoxOutputDriver;
        private System.Windows.Forms.Label lblAudioDriver;
        private System.ComponentModel.BackgroundWorker bgPlayWorker;
        private System.Windows.Forms.TrackBar tbVolume;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblSep;
        private System.Windows.Forms.Button btnRew;
        private System.Windows.Forms.Button btnFwd;
        private System.Windows.Forms.TrackBar tbAudioPos;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plistWndTsmItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem langTsmItem;
        private System.Windows.Forms.ToolStripMenuItem langEngTsmItem;
        private System.Windows.Forms.ToolStripMenuItem langItaTsmItem;
        private MarqueeLabel marqueeLbl;        
    }
}