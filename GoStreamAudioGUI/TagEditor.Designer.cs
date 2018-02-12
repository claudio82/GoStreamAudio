namespace GoStreamAudioGUI
{
    partial class TagEditor
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblArtist = new System.Windows.Forms.Label();
            this.lblGenre = new System.Windows.Forms.Label();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.lblTrackNum = new System.Windows.Forms.Label();
            this.lblPicture = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.txtAlbum = new System.Windows.Forms.TextBox();
            this.nudYear = new System.Windows.Forms.NumericUpDown();
            this.nudTrackNum = new System.Windows.Forms.NumericUpDown();
            this.pictBox = new System.Windows.Forms.PictureBox();
            this.cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chgPict = new System.Windows.Forms.ToolStripMenuItem();
            this.getRemotePict = new System.Windows.Forms.ToolStripMenuItem();
            this.delPict = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cbGenre = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrackNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBox)).BeginInit();
            this.cMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(13, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            // 
            // lblArtist
            // 
            this.lblArtist.AutoSize = true;
            this.lblArtist.Location = new System.Drawing.Point(13, 50);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(30, 13);
            this.lblArtist.TabIndex = 1;
            this.lblArtist.Text = "Artist";
            // 
            // lblGenre
            // 
            this.lblGenre.AutoSize = true;
            this.lblGenre.Location = new System.Drawing.Point(13, 80);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Size = new System.Drawing.Size(36, 13);
            this.lblGenre.TabIndex = 2;
            this.lblGenre.Text = "Genre";
            // 
            // lblAlbum
            // 
            this.lblAlbum.AutoSize = true;
            this.lblAlbum.Location = new System.Drawing.Point(13, 110);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(36, 13);
            this.lblAlbum.TabIndex = 3;
            this.lblAlbum.Text = "Album";
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(13, 140);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(29, 13);
            this.lblYear.TabIndex = 4;
            this.lblYear.Text = "Year";
            // 
            // lblTrackNum
            // 
            this.lblTrackNum.AutoSize = true;
            this.lblTrackNum.Location = new System.Drawing.Point(13, 170);
            this.lblTrackNum.Name = "lblTrackNum";
            this.lblTrackNum.Size = new System.Drawing.Size(45, 13);
            this.lblTrackNum.TabIndex = 5;
            this.lblTrackNum.Text = "Track #";
            // 
            // lblPicture
            // 
            this.lblPicture.AutoSize = true;
            this.lblPicture.Location = new System.Drawing.Point(13, 200);
            this.lblPicture.Name = "lblPicture";
            this.lblPicture.Size = new System.Drawing.Size(40, 13);
            this.lblPicture.TabIndex = 6;
            this.lblPicture.Text = "Picture";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(69, 17);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(250, 20);
            this.txtTitle.TabIndex = 7;
            // 
            // txtArtist
            // 
            this.txtArtist.Location = new System.Drawing.Point(69, 47);
            this.txtArtist.Name = "txtArtist";
            this.txtArtist.Size = new System.Drawing.Size(250, 20);
            this.txtArtist.TabIndex = 8;
            // 
            // txtAlbum
            // 
            this.txtAlbum.Location = new System.Drawing.Point(69, 107);
            this.txtAlbum.Name = "txtAlbum";
            this.txtAlbum.Size = new System.Drawing.Size(250, 20);
            this.txtAlbum.TabIndex = 10;
            // 
            // nudYear
            // 
            this.nudYear.Location = new System.Drawing.Point(69, 138);
            this.nudYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.nudYear.Name = "nudYear";
            this.nudYear.Size = new System.Drawing.Size(250, 20);
            this.nudYear.TabIndex = 11;
            this.nudYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            // 
            // nudTrackNum
            // 
            this.nudTrackNum.Location = new System.Drawing.Point(69, 168);
            this.nudTrackNum.Name = "nudTrackNum";
            this.nudTrackNum.Size = new System.Drawing.Size(250, 20);
            this.nudTrackNum.TabIndex = 12;
            // 
            // pictBox
            // 
            this.pictBox.ContextMenuStrip = this.cMenu;
            this.pictBox.Image = global::GoStreamAudioGUI.Properties.Resources.noPic;
            this.pictBox.InitialImage = null;
            this.pictBox.Location = new System.Drawing.Point(69, 216);
            this.pictBox.Name = "pictBox";
            this.pictBox.Size = new System.Drawing.Size(146, 132);
            this.pictBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBox.TabIndex = 13;
            this.pictBox.TabStop = false;
            // 
            // cMenu
            // 
            this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chgPict,
            this.getRemotePict,
            this.delPict});
            this.cMenu.Name = "cMenu";
            this.cMenu.Size = new System.Drawing.Size(287, 70);
            // 
            // chgPict
            // 
            this.chgPict.Name = "chgPict";
            this.chgPict.Size = new System.Drawing.Size(286, 22);
            this.chgPict.Text = "Change";
            this.chgPict.Click += new System.EventHandler(this.Addpicture_Click);
            // 
            // getRemotePict
            // 
            this.getRemotePict.Name = "getRemotePict";
            this.getRemotePict.Size = new System.Drawing.Size(286, 22);
            this.getRemotePict.Text = "Get picture from online service (Last.fm)";
            this.getRemotePict.Click += new System.EventHandler(this.GetpictureOnline_Click);
            // 
            // delPict
            // 
            this.delPict.Name = "delPict";
            this.delPict.Size = new System.Drawing.Size(286, 22);
            this.delPict.Text = "Remove";
            this.delPict.Click += new System.EventHandler(this.Removepicture_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(244, 359);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 15;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cbGenre
            // 
            this.cbGenre.FormattingEnabled = true;
            this.cbGenre.Location = new System.Drawing.Point(69, 77);
            this.cbGenre.Name = "cbGenre";
            this.cbGenre.Size = new System.Drawing.Size(250, 21);
            this.cbGenre.TabIndex = 9;
            // 
            // TagEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 387);
            this.Controls.Add(this.cbGenre);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.pictBox);
            this.Controls.Add(this.nudTrackNum);
            this.Controls.Add(this.nudYear);
            this.Controls.Add(this.txtAlbum);
            this.Controls.Add(this.txtArtist);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblPicture);
            this.Controls.Add(this.lblTrackNum);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.lblAlbum);
            this.Controls.Add(this.lblGenre);
            this.Controls.Add(this.lblArtist);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 426);
            this.Name = "TagEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tag Editor";
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrackNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBox)).EndInit();
            this.cMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Label lblTrackNum;
        private System.Windows.Forms.Label lblPicture;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.TextBox txtAlbum;
        private System.Windows.Forms.NumericUpDown nudYear;
        private System.Windows.Forms.NumericUpDown nudTrackNum;
        private System.Windows.Forms.PictureBox pictBox;
        private System.Windows.Forms.ContextMenuStrip cMenu;
        private System.Windows.Forms.ToolStripMenuItem chgPict;
        private System.Windows.Forms.ToolStripMenuItem getRemotePict;
        private System.Windows.Forms.ToolStripMenuItem delPict;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ComboBox cbGenre;
    }
}