using GoStreamAudioLib;
using Luminescence.Xiph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
//using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GoStreamAudioGUI
{
    public partial class TagEditor : Form
    {
        const string CM_CHANGE = "Change";
        const string CM_REMOVE = "Remove";
        const string CM_GETONLINE = "Get picture from online service (Last.fm)";
        const string LFM_BASE_URL = "http://ws.audioscrobbler.com/2.0/";
        const string LFM_METHOD = "album.getinfo";
        const string LFM_API_KEY = "57ee3318536b23ee81d6b27e36997cde";
        //static ContextMenu cm;
        
        static TagLib.File file;
        static VorbisComment vbTagger;
        static string tempImageFileName = string.Empty;

        LocalAudioPlayer player;
        string fileName;

        public TagEditor()
        {
            InitializeComponent();
        }

        #region Public Methods

        /// <summary>
        /// loads tag information from audio file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="player"></param>
        /// <param name="IsMp3"></param>
        public void LoadTagInfo(string fileName, LocalAudioPlayer player, bool IsMp3)
        {
            this.player = player;
            this.fileName = fileName;

            pictBox.WaitOnLoad = false;
            pictBox.LoadCompleted += LoadCompleted;

            if (IsMp3)
            {
                LoadMp3Information();
            }
            else
            {
                cMenu.Enabled = false;

                LoadVorbisInformation();
            }

        }

        /// <summary>
        /// performs clean up of resources 
        /// </summary>
        public void CleanUp()
        {
            if (cMenu != null)
            {
                foreach (ToolStripItem item in cMenu.Items)
                {
                    if ((string)item.Text == CM_CHANGE)
                        item.Click -= Addpicture_Click;
                    if ((string)item.Text == CM_REMOVE)
                        item.Click -= Removepicture_Click;
                    if ((string)item.Text == CM_GETONLINE)
                        item.Click -= GetpictureOnline_Click;
                }
            }
            if (pictBox != null)
                pictBox.LoadCompleted -= LoadCompleted;
            //if (prompt != null)
            //{
            this.Close();
            this.Dispose();
            // prompt = null;
            //}
            GC.Collect();
        }
                
        #endregion

        private void LoadMp3Information()
        {
            try
            {
                file = TagLib.File.Create(fileName);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                Debug.WriteLine("UNSUPPORTED FILE: " + fileName);
            }

            txtTitle.Text = (file.Tag.Title == null) ? "" : file.Tag.Title;
            txtArtist.Text = "";
            if (file.Tag.TagTypes == (TagLib.TagTypes.Id3v1 | TagLib.TagTypes.Id3v2))
            {
                TagLib.Tag id3V1 = file.GetTag(TagLib.TagTypes.Id3v1);
                if (id3V1 != null)
                {
                    if (id3V1.Performers.Length > 0 && id3V1.Performers[0].Contains("/"))
                    {
                        txtArtist.Text = id3V1.Performers[0];
                    }
                    else
                        txtArtist.Text = (file.Tag.Performers != null && file.Tag.Performers.Length > 0) ? file.Tag.Performers[0] : "";
                }
            }
            else
                txtArtist.Text = (file.Tag.Performers != null && file.Tag.Performers.Length > 0) ? file.Tag.Performers[0] : "";
            string strGenre = (file.Tag.Genres != null && file.Tag.Genres.Length > 0) ? file.Tag.Genres[0] : "";
            Dictionary<string, string> genres = new Dictionary<string, string>();
            genres.Add("0", "");
            int cnt = 1;
            foreach (string gInfo in TagLib.Genres.Audio)
            {
                genres.Add(cnt.ToString(), gInfo);
                ++cnt;
            }
            cbGenre.DisplayMember = "Value";
            cbGenre.ValueMember = "Key";
            cbGenre.BindingContext = new BindingContext();
            cbGenre.DataSource = new BindingSource(genres, null);

            if (genres.ContainsValue(strGenre))
            {
                cbGenre.SelectedItem = genres.FirstOrDefault(x => x.Value == strGenre);
            }
            txtAlbum.Text = (file.Tag.Album == null) ? "" : file.Tag.Album;
            nudYear.Minimum = 1900;
            nudYear.Maximum = 9999;
            if (file.Tag.Year > 0)
                nudYear.Text = file.Tag.Year.ToString();
            nudTrackNum.Minimum = 0;
            if (file.Tag.Track > 0)
                nudTrackNum.Text = file.Tag.Track.ToString();
            //pictBox.Name = "pictureBox";
            //pictBox.SizeMode = PictureBoxSizeMode.StretchImage;            

            if (file.Tag.Pictures != null)
            {
                // display the embedded picture if exists                
                var pictures = file.Tag.Pictures;
                if (pictures.Length > 0)
                {
                    TagLib.Picture fstPic = new TagLib.Picture(pictures[0].Data);
                    if (fstPic != null)
                    {
                        //MemoryStream ms = new MemoryStream(fstPic.Data.ToArray());
                        //System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        //var img = image;
                        //picBox.Image = img;

                        try
                        {
                            tempImageFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".jpg");
                            using (FileStream fileStream = new FileStream(tempImageFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fileStream))
                                {
                                    writer.Write(fstPic.Data.ToArray());
                                }
                            }

                            pictBox.LoadAsync(tempImageFileName);
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine(string.Format("Cannot save temp image file: {0}", tempImageFileName));
                        }

                        cMenu.Items[0].Enabled = false;
                        cMenu.Items[1].Enabled = false;
                        cMenu.Items[2].Enabled = true;
                        //cMenu.MenuItems.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
                    }
                }
                else
                    cMenu.Items[2].Enabled = false;
                //{
                //    pictBox.Image = GoStreamAudioGUI.Properties.Resources.noPic;
                //}
            }
            else
                cMenu.Items[2].Enabled = false;

        }

        private void LoadVorbisInformation()
        {
            try
            {
                if (fileName.EndsWith(".flac"))
                    vbTagger = new FlacTagger(fileName);
                if (fileName.EndsWith(".ogg"))
                    vbTagger = new OggTagger(fileName);
            }
            catch (Exception)
            {
                Debug.WriteLine("UNSUPPORTED FILE: " + fileName);
            }

            txtTitle.Text = (vbTagger.Title == null) ? "" : vbTagger.Title;
            txtArtist.Text = (vbTagger.Artist == null) ? "" : vbTagger.Artist;
            string strGenre = (vbTagger.Genre == null) ? "" : vbTagger.Genre;
            Dictionary<string, string> genres = new Dictionary<string, string>();
            genres.Add("0", "");
            int cnt = 1;
            foreach (string gInfo in Genres.GetGenres())
            {
                genres.Add(cnt.ToString(), gInfo);
                ++cnt;
            }
            cbGenre.DisplayMember = "Value";
            cbGenre.ValueMember = "Key";
            cbGenre.BindingContext = new BindingContext();
            cbGenre.DataSource = new BindingSource(genres, null);

            if (genres.ContainsValue(strGenre))
            {
                cbGenre.SelectedItem = genres.FirstOrDefault(x => x.Value == strGenre);
            }
            txtAlbum.Text = (vbTagger.Album == null) ? "" : vbTagger.Album;
            if (vbTagger.Date != null)
                nudYear.Text = vbTagger.Date;
            if (vbTagger.TrackNumber != null)
                nudTrackNum.Text = vbTagger.TrackNumber;

            if (vbTagger.Arts != null)
            {
                // display the embedded picture if exists                
                var pictures = vbTagger.Arts;
                if (pictures.Count > 0)
                {
                    TagLib.Picture fstPic = new TagLib.Picture(pictures[0].PictureData);
                    try
                    {
                        tempImageFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".jpg");
                        using (FileStream fileStream = new FileStream(tempImageFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            using (BinaryWriter writer = new BinaryWriter(fileStream))
                            {
                                writer.Write(pictures[0].PictureData.ToArray());
                            }
                        }

                        pictBox.LoadAsync(tempImageFileName);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine(string.Format("Cannot save temp image file: {0}", tempImageFileName));
                    }
                }
                //else
                //{
                    //pictBox.Image = GoStreamAudioGUI.Properties.Resources.noPic;
                //}
            }
        }

        /// <summary>
        /// uses a web service to retrieve album art
        /// </summary>
        /// <param name="info"></param>
        private void FindCover(string[] info)
        {
            string artist = info[0];
            string album = info[1];
            Uri uri = new Uri(
                HttpUtility.HtmlDecode(
                string.Format("{0}?method={1}&amp;artist={2}&amp;album={3}&amp;api_key={4}",
                LFM_BASE_URL,
                LFM_METHOD,
                artist,
                album,
                LFM_API_KEY))
            );
            if (uri != null)
            {
                WebRequest request = WebRequest.Create(uri);
                if (request != null)
                {
                    XmlReader xReader = null;
                    Stream objStream = null;
                    try
                    {
                        request.Method = "GET";

                        objStream = request.GetResponse().GetResponseStream();
                        xReader = XmlReader.Create(objStream);
                        XDocument xdoc = XDocument.Load(xReader);

                        //
                        //if (xReader != null)
                        //{
                        //xReader.MoveToContent();
                        /*XElement imgItem =
                            (from el in xdoc.Root.Elements()
                            where (string)el.Attribute("size") == "medium"
                            select el).FirstOrDefault();*/
                        XElement imgItem = xdoc.XPathSelectElement("//image[@size='large']");
                        if (imgItem != null && imgItem.FirstNode != null)
                        {
                            string imgPath = imgItem.FirstNode.ToString();
                            WebResponse res = null;
                            Stream stm = null;
                            try
                            {
                                WebRequest req2 = WebRequest.Create(imgPath);
                                res = req2.GetResponse();
                                if (res != null)
                                {
                                    stm = res.GetResponseStream();
                                    System.Drawing.Image image = System.Drawing.Image.FromStream(stm);
                                    pictBox.Image = image;

                                    //add the image to id3 tag
                                    file.Tag.Pictures = new TagLib.IPicture[]
                                    {
                                        new TagLib.Picture(
                                            new TagLib.ByteVector(
                                                (byte[])new System.Drawing.ImageConverter()
                                                .ConvertTo(image, typeof(byte[]))))
                                    };

                                    //bool hasRemove = false;
                                    foreach (ToolStripItem item in cMenu.Items)
                                    {
                                        if ((string)item.Text == CM_REMOVE)
                                        {
                                            item.Enabled = true;
                                        }
                                        if ((string)item.Text == CM_CHANGE
                                            || (string)item.Text == CM_GETONLINE)
                                        {
                                            item.Enabled = false;
                                        }
                                    }
                                    //if (!hasRemove)
                                    //    cMenu.Items.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
                                }
                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                if (stm != null)
                                    stm.Close();
                                if (res != null)
                                    res.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Album art not found.");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Album art not found.");
                    }
                    finally
                    {
                        if (xReader != null)
                            xReader.Close();
                        if (objStream != null)
                            objStream.Close();
                    }
                }
            }
        }

        private void LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // will get this if there's an error loading the file
            } if (e.Cancelled)
            {
                // would get this if you have code that calls pictureBox1.CancelAsync()
            }
            else
            {
                // picture was loaded successfully
                try
                {
                    File.Delete(tempImageFileName);
                }
                catch (Exception)
                {

                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text;
            string artist = txtArtist.Text;
            string album = txtAlbum.Text;
            string genre = cbGenre.GetItemText(cbGenre.SelectedItem);
            uint year = 0;
            if (nudYear.Text != "" && nudYear.Value != nudYear.Minimum)
            {
                uint.TryParse(nudYear.Text, out year);
            }
            uint trackNum = 0;
            if (nudTrackNum.Text != "" && nudTrackNum.Value != nudTrackNum.Minimum)
            {
                uint.TryParse(nudTrackNum.Text, out trackNum);
            }
            if (file != null)
            {
                file.Tag.Title = title;
                file.Tag.Performers = new string[] { artist };
                file.Tag.Album = album;
                file.Tag.Genres = new string[] { genre };
                file.Tag.Year = year;
                file.Tag.Track = trackNum;

                try
                {
                    if (!Utils.IsFileLocked(new FileInfo(fileName)))
                        file.Save();
                    else
                        player.SaveMp3Tag(file);
                }
                catch (UnauthorizedAccessException)
                { }
                catch (Exception)
                { }
            }
            if (vbTagger != null)
            {
                vbTagger.Title = title;
                vbTagger.Artist = artist;
                vbTagger.Album = album;
                vbTagger.Genre = genre;
                vbTagger.Date = year.ToString();
                vbTagger.TrackNumber = trackNum.ToString();

                try
                {
                    if (!Utils.IsFileLocked(new FileInfo(fileName)))
                        vbTagger.SaveMetadata();
                    else
                        player.SaveVorbisTag(vbTagger);
                }
                catch (UnauthorizedAccessException)
                { }
                catch (Exception)
                { }
            }
            this.Hide();
        }

        /// <summary>
        /// adds a picture frame to the ID3 v2 tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Addpicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();
            ofDialog.Filter = "Image Files|*.jpg;*.png";
            ofDialog.Title = "Open Image File";
            ofDialog.RestoreDirectory = false;

            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = ofDialog.FileName;
                try
                {
                    //Control ctls = this.Controls["pictBox"];
                    //if (ctls != null)
                    {
                        try
                        {
                            //PictureBox pictBox = (PictureBox)ctls;
                            pictBox.Image = System.Drawing.Image.FromFile(fName);
                            file.Tag.Pictures = new TagLib.IPicture[]
                            {
                                new TagLib.Picture(
                                    new TagLib.ByteVector(
                                        (byte[])new System.Drawing.ImageConverter()
                                        .ConvertTo(System.Drawing.Image.FromFile(fName), typeof(byte[]))))
                            };
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine("Unable to change the image.");
                        }
                        //bool hasRemove = false;
                        foreach (ToolStripItem item in cMenu.Items)
                        {
                            if ((string)item.Text == CM_REMOVE)
                            {
                                item.Enabled = true;
                            }
                            if ((string)item.Text == CM_CHANGE
                                || (string)item.Text == CM_GETONLINE)
                            {
                                item.Enabled = false;
                            }
                        }
                        //if (!hasRemove)
                        //    cm.MenuItems.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine(string.Format("Cannot load picture file : {0}", fName));
                }
            }
        }

        /// <summary>
        /// downloads a cover image from a remote service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetpictureOnline_Click(object sender, EventArgs e)
        {
            if (txtArtist != null && txtAlbum != null)
            {
                string album = txtAlbum.Text.Trim();
                string artist = txtArtist.Text.Trim();
                if (!string.IsNullOrEmpty(album)
                    && !string.IsNullOrEmpty(artist))
                {
                    FindCover(new string[] { artist, album });
                }
            }
        }

        // <summary>
        /// remove all the picture frames from the ID3 v2 tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Removepicture_Click(object sender, EventArgs e)
        {
            if (file.Tag.Pictures != null)
            {
                var pictures = file.Tag.Pictures;
                if (pictures.Length > 0)
                {
                    file.Tag.Pictures = null;

                    // reset the picture to the default one
                    //Control ctls = this.Controls["pictBox"];
                    //if (ctls != null)
                    {
                        try
                        {
                            //PictureBox pictBox = (PictureBox)ctls;
                            pictBox.Image = GoStreamAudioGUI.Properties.Resources.noPic;
                            foreach (ToolStripItem item in cMenu.Items)
                            {
                                if ((string)item.Text == CM_REMOVE)
                                {
                                    item.Enabled = false;
                                }
                                if ((string)item.Text == CM_CHANGE
                                    || (string)item.Text == CM_GETONLINE)
                                {
                                    item.Enabled = true;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
    }
}
