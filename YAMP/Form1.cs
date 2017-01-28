using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace YAMP
{
    public partial class Form1 : Form
    {
        public Mp3FileReader mp3 = null;
        public WaveFileReader wav;

        public TagLib.Tag tag;
        public TagLib.File tagfile;

        public WaveOut wo = null;

        public string curFile;

        public bool ReplayMusic = true;

        public Form1()
        {
            InitializeComponent();

            this.trackBar1.MouseDown += trackBar1_MouseDown;
            this.trackBar1.MouseUp += trackBar1_MouseUp;
        }

        void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Start(); mp3.CurrentTime = TimeSpan.FromSeconds(trackBar1.Value);
        }

        void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            timer1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadMusicFile(ofd.FileName);
                }
            }
        }

        private void LoadMusicFile(string filePath)
        {
            timer1.Stop(); timer1.Enabled = false; trackBar1.Value = 0;

            if (wo != null) { wo.Stop(); System.Threading.Thread.Sleep(100); wo.Dispose(); }
            if (mp3 != null) { mp3.Position = 0; mp3.CurrentTime = TimeSpan.FromSeconds(0); System.Threading.Thread.Sleep(100); mp3.Dispose(); }

            mp3 = new Mp3FileReader(filePath); mp3.Position = 0;
            wo = new WaveOut();

            wo.Init(mp3); wo.Play(); 

            tag = TagLib.File.Create(@filePath).Tag;
            tagfile = TagLib.File.Create(@filePath);

            this.Text = "YAMP - " + tag.Title + " by " + string.Join(", ", tag.Performers);
            label1.Text = tag.Title; label2.Text = string.Join(", ", tag.Performers);

            if (tag.Pictures.Length > 0)
            {
                pictureBox1.Image = Image.FromStream(new System.IO.MemoryStream(tag.Pictures[0].Data.Data));
            }

            timer1.Start(); curFile = filePath; button2.Text = "Pause";

            System.Diagnostics.Debug.WriteLine("YAMP Debug Info - " + DateTime.Now.ToString() + Environment.NewLine + "File: " + curFile + Environment.NewLine + "Audio Bitrate: " + tagfile.Properties.AudioBitrate + Environment.NewLine + "Audio Channels: " + tagfile.Properties.AudioChannels + Environment.NewLine + "Audio Sample Rate: " + tagfile.Properties.AudioSampleRate + Environment.NewLine + "Bits per Sample: " + tagfile.Properties.BitsPerSample + Environment.NewLine + "Codecs/Description: " + string.Join(", ", tagfile.Properties.Codecs) + " | " + tagfile.Properties.Description);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int totalTimeSec = (int)mp3.TotalTime.TotalSeconds;
            int curTimeSec = (int)mp3.CurrentTime.TotalSeconds;

            trackBar1.Maximum = totalTimeSec;
            trackBar1.Value = curTimeSec;

            label3.Text = mp3.CurrentTime.ToString(@"mm\:ss") + " / " + mp3.TotalTime.ToString(@"mm\:ss");

            if (ReplayMusic == true)
            {
                if (curTimeSec == totalTimeSec)
                {
                    mp3.Position = 0;
                }
            }
            else
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Play")
            {
                wo.Play(); button2.Text = "Pause";
            }
            else if (button2.Text == "Pause")
            {
                wo.Stop(); button2.Text = "Play";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wo.Stop(); mp3.Position = 0; button2.Text = "Play";
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (label4.Text == "Replay: ON")
            {
                label4.Text = "Replay: OFF";
                ReplayMusic = false;
            }
            else if (label4.Text == "Replay: OFF")
            {
                label4.Text = "Replay: ON";
                ReplayMusic = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadMusicFile(ofd.FileName);
                }
            }
        }
    }
}
