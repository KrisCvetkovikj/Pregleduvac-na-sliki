using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pregleduvac_na_sliki
{
    public partial class FullScreen : Form
    {
        List<string> imgList;
        int currImage = 0;

        public FullScreen(List<string> imgList, int currImage)
        {
            InitializeComponent();
            this.Visible = false;
            this.DoubleBuffered = true;
            this.imgList = imgList;
            this.currImage = currImage;
            fullPreview(this.imgList, this.currImage);
        }

        private void fullPreview(List<string> imgList, int currImage)
        {
            try
            {
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.pictureBox1.Bounds = Screen.PrimaryScreen.Bounds;
                this.Visible = true;
                this.Opacity = 0.0;
                TimerStart();
                this.pictureBox1.Image = new Bitmap(this.imgList[this.currImage]);
            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show("Отвори слика од диск!" + " Грешка " + ane.Message);
            }
        }

        private void TimerStart()
        {
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 20;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.02;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.pictureBox1.Dispose();
            this.Dispose();
            this.Close();
        }

        private void FullScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                pictureBox1_DoubleClick(null, null);
            }
            if (e.KeyCode == Keys.Left)
            {
                this.pictureBox1.Image = Image.FromFile(imgList[--currImage]);
            }
            if (e.KeyCode == Keys.Right)
            {
                this.pictureBox1.ImageLocation = imgList[++currImage];
            }
        }
    }
}