using GoStreamAudioLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GoStreamAudioGUI
{
    public class Mp3TagManager : Form
    {
        const string CM_CHANGE = "Change";
        const string CM_REMOVE = "Remove";
        const string CM_GETONLINE = "Get picture from online service (Last.fm)";
        const string LFM_BASE_URL = "http://ws.audioscrobbler.com/2.0/";
        const string LFM_METHOD = "album.getinfo";
        const string LFM_API_KEY = "57ee3318536b23ee81d6b27e36997cde";
        static ContextMenu cm;
        static Form prompt;
        static TextBox txtArtist;
        static TextBox txtAlbum;
        static PictureBox picBox;
        static TagLib.File file;

        public DialogResult LoadTagInfo(string caption, string fileName, LocalAudioPlayer player)
        {
            prompt = new Form();
            prompt.Width = 400;
            prompt.Height = 500;
            prompt.ShowIcon = false;
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = caption;
            
            try
            {
                file = TagLib.File.Create(fileName);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                Debug.WriteLine("UNSUPPORTED FILE: " + fileName);
            }
            
            /*
            u = new UltraID3();

            try
            {
                u.Read(fileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error reading tag info: {0}", ex.Message));
            }

            GenreInfoCollection genColl = u.GenreInfos;
            */
            Label lblTitle = new Label() { Left = 5, Top = 50, Text = "Title", Width = 50 }; // Text = text };            
            TextBox txtTitle = new TextBox() { Left = 70, Top = 50, Width = 300 };
            txtTitle.Text = (file.Tag.Title == null) ? "" : file.Tag.Title;
            Label lblArtist = new Label() { Left = 5, Top = 90, Text = "Artist", Width = 50 };
            txtArtist = new TextBox() { Left = 70, Top = 90, Width = 300 };
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
            Label lblGenre = new Label() { Left = 5, Top = 130, Text = "Genre", Width = 50 };
            ComboBox cbGenre = new ComboBox() { Left = 70, Top = 130, Width = 300 };
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

            Label lblAlbum = new Label() { Left = 5, Top = 170, Text = "Album", Width = 50 };
            txtAlbum = new TextBox() { Left = 70, Top = 170, Width = 300 };
            txtAlbum.Text = (file.Tag.Album == null) ? "" : file.Tag.Album;
            Label lblYear = new Label() { Left = 5, Top = 210, Text = "Year", Width = 50 };
            NumericUpDown nudYear = new NumericUpDown() { Left = 70, Top = 210, Width = 300 };
            nudYear.Minimum = 1900;
            nudYear.Maximum = 9999;
            
            if (file.Tag.Year > 0)
                nudYear.Text = file.Tag.Year.ToString();
            
            Label lblTrackNum = new Label() { Left = 5, Top = 250, Text = "Track #", Width = 50 };
            NumericUpDown nudTrackNum = new NumericUpDown() { Left = 70, Top = 250, Width = 300 };
            nudTrackNum.Minimum = 0;            
            if (file.Tag.Track > 0)
                nudTrackNum.Text = file.Tag.Track.ToString();
            
            Label lblPict = new Label() { Left = 5, Top = 290, Text = "Picture", Width = 50 };
            picBox = new PictureBox() { Left = 70, Width = 128, Height = 128, Top = 290 };
            picBox.Name = "pictureBox";
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            cm = new ContextMenu();
            cm.MenuItems.Add(CM_CHANGE, new EventHandler(Addpicture_Click));
            cm.MenuItems.Add(CM_GETONLINE, new EventHandler(GetpictureOnline_Click));
            //cm.MenuItems.Add("Item 2");

            if (file.Tag.Pictures != null)
            {
                // display the embedded picture if exists
                //ID3FrameCollection frCol = u.ID3v2Tag.Frames.GetFrames(ID3v2FrameTypes.ID3v23Picture);
                var pictures = file.Tag.Pictures;
                if (pictures.Length > 0)
                {
                    TagLib.Picture fstPic = new TagLib.Picture(pictures[0].Data);
                    if (fstPic != null)
                    {
                        MemoryStream ms = new MemoryStream(fstPic.Data.ToArray());
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        var img = image;
                        picBox.Image = img;
                        cm.MenuItems[0].Enabled = false;
                        cm.MenuItems[1].Enabled = false;
                        cm.MenuItems.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
                    }
                }
                else
                {
                    picBox.Image = GoStreamAudioGUI.Properties.Resources.noPic;
                }
            }
            picBox.ContextMenu = cm;
            //NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Update", Left = 270, Width = 100, Top = 430 };
            confirmation.Click += (sender, e) =>
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
                file.Tag.Title = title;                
                file.Tag.Performers = new string[] { artist };                
                file.Tag.Album = album;
                file.Tag.Genres = new string[] { genre };
                file.Tag.Year = year;
                file.Tag.Track = trackNum;

                if (!Utils.IsFileLocked(new FileInfo(fileName)))
                    file.Save();
                else
                    player.SaveMp3Tag(file);
                                
                prompt.Close();
                prompt.Dispose();
            };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(lblTitle);
            prompt.Controls.Add(txtTitle);
            prompt.Controls.Add(lblArtist);
            prompt.Controls.Add(txtArtist);
            prompt.Controls.Add(lblAlbum);
            prompt.Controls.Add(txtAlbum);
            prompt.Controls.Add(lblGenre);
            prompt.Controls.Add(cbGenre);
            prompt.Controls.Add(lblYear);
            prompt.Controls.Add(nudYear);
            prompt.Controls.Add(lblTrackNum);
            prompt.Controls.Add(nudTrackNum);
            prompt.Controls.Add(lblPict);
            prompt.Controls.Add(picBox);
            return prompt.ShowDialog();
        }

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
        
        /// <summary>
        /// remove all the picture frames from the ID3 v2 tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Removepicture_Click(object sender, EventArgs e)
        {
            if (file.Tag.Pictures != null)
            {                
                var pictures = file.Tag.Pictures;
                if (pictures.Length > 0)
                {
                    file.Tag.Pictures = null;
                    
                    // reset the picture to the default one
                    Control ctls = prompt.Controls["pictureBox"];
                    if (ctls != null)
                    {
                        try
                        {
                            PictureBox pictBox = (PictureBox)ctls;
                            pictBox.Image = GoStreamAudioGUI.Properties.Resources.noPic;
                            foreach (MenuItem item in cm.MenuItems)
                            {
                                if ((string)item.Text == CM_REMOVE)
                                {
                                    cm.MenuItems.Remove(item);
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

        private static void Addpicture_Click(object sender, EventArgs e)
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
                    Control ctls = prompt.Controls["pictureBox"];
                    if (ctls != null)
                    {
                        try
                        {
                            PictureBox pictBox = (PictureBox)ctls;
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
                        bool hasRemove = false;
                        foreach (MenuItem item in cm.MenuItems)
                        {
                            if ((string)item.Text == CM_REMOVE)
                            {
                                hasRemove = true;
                            }
                            if ((string)item.Text == CM_CHANGE
                                || (string)item.Text == CM_GETONLINE)
                            {
                                item.Enabled = false;
                            }
                        }
                        if (!hasRemove)
                            cm.MenuItems.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine(string.Format("Cannot load picture file : {0}", fName));
                }
            }
        }

        public void CleanUp()
        {
            if (cm != null)
            {
                foreach(MenuItem item in cm.MenuItems)
                {
                    if ((string)item.Text == CM_CHANGE)
                        item.Click -= Addpicture_Click;
                    if ((string)item.Text == CM_REMOVE)
                        item.Click -= Removepicture_Click;
                    if ((string)item.Text == CM_GETONLINE)
                        item.Click -= GetpictureOnline_Click;
                }
            }
            if (prompt != null)
            {
                prompt.Close();
                prompt.Dispose();
                prompt = null;
            }
            GC.Collect();
        }

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
                                    picBox.Image = image;

                                    //add the image to id3 tag
                                    file.Tag.Pictures = new TagLib.IPicture[]
                                    {
                                        new TagLib.Picture(
                                            new TagLib.ByteVector(
                                                (byte[])new System.Drawing.ImageConverter()
                                                .ConvertTo(image, typeof(byte[]))))
                                    };

                                    bool hasRemove = false;
                                    foreach (MenuItem item in cm.MenuItems)
                                    {
                                        if ((string)item.Text == CM_REMOVE)
                                        {
                                            hasRemove = true;
                                        }
                                        if ((string)item.Text == CM_CHANGE
                                            || (string)item.Text == CM_GETONLINE)
                                        {
                                            item.Enabled = false;
                                        }
                                    }
                                    if (!hasRemove)
                                        cm.MenuItems.Add(CM_REMOVE, new EventHandler(Removepicture_Click));
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
    }
}
