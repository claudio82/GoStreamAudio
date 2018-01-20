using GoStreamAudioLib;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public partial class TestMp3StreamingForm : Form
    {
        Mp3StreamAudioPlayer audioPlayer;
        Thread workingThread = null;

        public TestMp3StreamingForm()
        {
            InitializeComponent();

            // create new player with empty url
            audioPlayer = new Mp3StreamAudioPlayer("");
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (audioPlayer.StreamingPlaybackState == StreamingPlaybackState.Stopped)
            {
                if (txtAudioUrl.Text != null && !string.IsNullOrWhiteSpace(txtAudioUrl.Text))
                {
                    audioPlayer.AudioUrl = txtAudioUrl.Text;
                    audioPlayer.StreamingPlaybackState = StreamingPlaybackState.Buffering;
                    try
                    {
                        workingThread = new Thread(new ParameterizedThreadStart(DoJob))
                        { 
                            IsBackground = true
                        };
                        workingThread.Start();
//                      ThreadPool.QueueUserWorkItem(audioPlayer.StreamMp3, null);
                        timer1.Enabled = true;
                    }
                    catch (WebException)
                    {

                    }
                    catch (UriFormatException)
                    {

                    }
                    catch (Exception)
                    {
                                               
                    }
                }
            }
            else if (audioPlayer.StreamingPlaybackState == StreamingPlaybackState.Paused)
            {
                if (audioPlayer.AudioUrl != string.Empty)
                    audioPlayer.StreamingPlaybackState = StreamingPlaybackState.Buffering;
            }
        }

        private void DoJob(object obj)
        {
            audioPlayer.StreamMp3();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (audioPlayer.FlowAudio())
            {
                timer1.Enabled = false;
                workingThread.Abort();
                workingThread = null;
            }
        }
    }
}
