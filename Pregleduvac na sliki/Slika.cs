using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Pregleduvac_na_sliki
{
    public partial class Slika : Form
    {
        public Slika()
        {
            InitializeComponent();
            fileNames = new List<string>();
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Image = Properties.Resources.finki;
            this.KeyPreview = true;
            label1.Text = pictureBox1.ImageLocation;
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "Слики (*.JPG;*.BMP;*.GIF,*.PNG)| *.JPG;*.BMP;*.GIF;*.PNG|Сите датотеки (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Одбери слика...";
            contextMenuStrip1.Items[1].Enabled = false;
            contextMenuStrip1.Items[2].Enabled = false;
            this.pictureBox1.AllowDrop = true;
            this.toolTip1.SetToolTip(this.pictureBox1, "Right click for more options.");
        }

        protected List<string> fileNames;
        protected int pCurrentImage = -1;
        protected bool isFullScreen = false;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            fileNames.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileNames.AddRange(Directory.EnumerateFiles(Path.GetDirectoryName(openFileDialog1.FileName), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".bmp") || s.ToLower().EndsWith(".gif")));
                    pCurrentImage = fileNames.IndexOf(openFileDialog1.FileName);
                    ShowCurrentImage();
                }
                catch (Exception ioe)
                {
                }
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            pCurrentImage = pCurrentImage == 0 ? fileNames.Count - 1 : --pCurrentImage;
            ShowCurrentImage();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            pCurrentImage = pCurrentImage == fileNames.Count - 1 ? 0 : ++pCurrentImage;
            ShowCurrentImage();
        }

        protected void ShowCurrentImage()
        {
            if (fileNames.Count > 0)
            {
                try
                {
                    pictureBox1.ImageLocation = fileNames[pCurrentImage];
                    label1.Text = fileNames[pCurrentImage] + " (" + pictureBox1.Image.Width + "x" + pictureBox1.Image.Height + ")";
                    label2.Text = " Вкупно слики:" + fileNames.Count.ToString();
                    btnPrev.Enabled = true;
                    btnNext.Enabled = true;
                    contextMenuStrip1.Items[1].Enabled = true;
                    contextMenuStrip1.Items[2].Enabled = true;
                }
                catch (ArgumentOutOfRangeException aor)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = Properties.Resources.finki;
                    fileNames.Clear();
                    label1.Text = "";
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnOpen_Click(null, null);
        }

        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnPrev_Click(null, null);
        }

        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnNext_Click(null, null);
        }

        private void stretchedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Stretch);
        }

        private void changeWallpaper(String wallpaperName, WallpaperStyle wallStyle)
        {
            if (!String.IsNullOrEmpty(fileNames[pCurrentImage]))
            {
                Wallpaper.SetDesktopWallpaper(fileNames[pCurrentImage], wallStyle);
            }
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Tile);
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Center);
        }

        private void fitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Fit);
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Fill);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            if (pictureBox1.Image != null)
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
                {
                    try
                    {
                        pictureBox1.Image.Save(saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not save file \"" + openFileDialog1.FileName + "\" to disk. Original error: " + ex.Message);
                    }
                }
            }
        }

        private void Slika_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad4)
            {
                btnPrev_Click(null, null);
            }
            if (e.KeyCode == Keys.NumPad6)
            {
                btnNext_Click(null, null);
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (a != null)
            {
                String strPathToLoadedImage = a.GetValue(0).ToString();
                fileNames.Clear();
                try
                {
                    fileNames.AddRange(Directory.EnumerateFiles(Path.GetDirectoryName(strPathToLoadedImage), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".bmp") || s.ToLower().EndsWith(".gif")));

                    for (int i = 0; i < fileNames.Count; i++)
                    {
                        if (fileNames[i].Equals(strPathToLoadedImage))
                        {
                            pCurrentImage = i;
                            break;
                        }
                    }
                    ShowCurrentImage();
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("Can't open file!" + "\nError: \n" + ioe.Message);
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (!isFullScreen)
            {
                if (pictureBox1.ImageLocation != null)
                {
                    FullScreen fs = new FullScreen(fileNames, pCurrentImage);
                    fs.Show();
                    isFullScreen = true;
                }
            }
            else
            {
                isFullScreen = false;
            }
        }

        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap rotatedImg = new Bitmap(this.pictureBox1.Image);
            if (rotatedImg != null)
            {
                rotatedImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
                this.pictureBox1.Image = rotatedImg;
            }
        }

        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap rotatedImg = new Bitmap(this.pictureBox1.Image);
            if (rotatedImg != null)
            {
                rotatedImg.RotateFlip(RotateFlipType.Rotate90FlipNone);
                this.pictureBox1.Image = rotatedImg;
            }
        }
    }
}