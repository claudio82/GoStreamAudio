using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GoStreamAudioGUI
{
    public partial class TestAlbumArt : Form
    {
        public TestAlbumArt()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAlbum.Text.Trim()))
            {
                string[] search = new string[2];
                search[0] = txtArtist.Text;
                search[1] = txtAlbum.Text;
                if (!string.IsNullOrEmpty(txtArtist.Text) 
                    && !string.IsNullOrEmpty(txtAlbum.Text))
                    FindCover(search);
            }
        }

        private void FindCover(string[] info)
        {
            string artist = info[0].Trim();
            string album = info[1].Trim();
            Uri uri = new Uri(
                HttpUtility.HtmlDecode(
                string.Format("http://ws.audioscrobbler.com/2.0/?method=album.getinfo&amp;artist={0}&amp;album={1}&amp;api_key=57ee3318536b23ee81d6b27e36997cde&amp;format=xml", 
                artist, 
                album))
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
                                    Image image = System.Drawing.Image.FromStream(stm);
                                    pictureBox1.Image = image;
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
                            
                            /*
                            while(xReader.Read())
                            {
                                if (xReader.Name == "album")
                                {
                                    XElement el = XNode.ReadFrom(xReader) as XElement;
                                    XElement imgEl = el.Element(XName.Get("image"));
                                    if (imgEl != null) // && imgEl.HasAttributes && imgEl.FirstAttribute.Name.Equals("medium"))
                                    {
                                        
                                    }
                                }
                            }*/
                        //}
                        //string line = sReader.ReadLine();
                        //Image image = System.Drawing.Image.FromStream(objStream);
                        //pictureBox1.Image = image;
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR : " + ex.Message);
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
