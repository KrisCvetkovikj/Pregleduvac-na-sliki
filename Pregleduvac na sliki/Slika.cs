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
            startingSettings();
        }

        /// <summary>
        /// Sets controls to their starting preferences.
        /// </summary>
        public void startingSettings()
        {
            fileNames = new List<string>();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Image = Properties.Resources.finki;
            this.KeyPreview = true;
            label1.Text = "/";
            // Used to set starting directory to Desktop when opening dialog
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "Слики (*.JPG;*.BMP;*.GIF,*.PNG)| *.JPG;*.BMP;*.GIF;*.PNG|Сите датотеки (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Одбери слика...";
            // Allows dropping files on Form
            this.pictureBox1.AllowDrop = true;
            this.toolTip1.SetToolTip(this.pictureBox1, "Right click for more options.");
        }

        // Needed variables

        // Keeps the paths to the images
        protected List<string> fileNames;

        protected int pCurrentImage = -1;
        protected Bitmap picImage;

        // An object of class FullScreen that helps when setting the form to fullscreen mode.
        private FullScreen FullScreen = new FullScreen();
        
        // These variables are used to return the bounds of picture box as they were at first
        private static readonly int PIC_WIDTH = 790;
        private static readonly int PIC_HEIGHT = 400;

        // Used to check whether the form is during a slideshow
        private bool isSlideShow = false;


        /// <summary>
        /// Shows dialog for opening an image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // Clears previous filenames.
            fileNames.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // This returns a list of all bmp, gif, png and jpg files.
                    // Uses EnumerateFiles for traversing only through relevant image files instead of all files in folder.
                    // SearchOption is set to TopDirectoryOnly so it will search only through the opened folder.
                    // While traversing through files and checks extensions, uses lowercase function because some images
                    // have extensions like PNG, JPG and so on.
                    fileNames.AddRange(Directory.EnumerateFiles(Path.GetDirectoryName(openFileDialog1.FileName), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".bmp") || s.ToLower().EndsWith(".gif")));

                    // Sets pCurrentImage to the relevant image index in the string list.
                    pCurrentImage = fileNames.IndexOf(openFileDialog1.FileName);
                }
                catch (Exception ioe)
                {
                    label1.Text = ioe.Message;
                }
                ShowCurrentImage();
            }
        }

        /// <summary>
        /// This button is used to show the previous image in folder.
        /// Jumps to the last image when the first image is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pCurrentImage = pCurrentImage == 0 ? fileNames.Count - 1 : --pCurrentImage;
                picImage.Dispose();
                ShowCurrentImage();
            }
            else
            {
                MessageBox.Show("Отвори слика!");
            }
        }

        /// <summary>
        /// This button is used to show the next image in folder.
        /// Jumps to the first image at the last image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pCurrentImage = pCurrentImage == fileNames.Count - 1 ? 0 : ++pCurrentImage;
                picImage.Dispose();
                ShowCurrentImage();
            }
            else
            {
                MessageBox.Show("Отвори слика!");
            }
        }

        /// <summary>
        /// Method for opening images. Provides a bitmap with the absolute path of the image and then sets it 
        /// to the picture box.
        /// 
        /// Provides information for the current image and the amount of images in the loaded folder.
        /// </summary>
        protected void ShowCurrentImage()
        {
            try
            {
                // I used a Bitmap because it throws NullException otherwise when reading height and width.
                picImage = new Bitmap(fileNames[pCurrentImage]);
                pictureBox1.Image = picImage;
                label1.Text = fileNames[pCurrentImage] + " (" + picImage.Width + "x" + picImage.Height + ")";
                label2.Text = " Вкупно слики:" + fileNames.Count.ToString();
            }
            catch (ArgumentOutOfRangeException aor)
            {
                // Sets the picture box to the default image and clears the string list that keeps the paths to the images.
                pictureBox1.Image = Properties.Resources.finki;
                fileNames.Clear();
                label1.Text = aor.Message;
            }
        }


        /// <summary>
        /// Shows dialog for opening images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnOpen_Click(null, null);
        }

        /// <summary>
        /// Shows the previous image using the right-click menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnPrev_Click(null, null);
        }

        /// <summary>
        /// Shows the next image using the right-click menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnNext_Click(null, null);
        }

        /// <summary>
        /// Used to shorten code when changing wallpapers.
        /// </summary>
        /// <param name="wallpaperName"></param>
        /// <param name="wallStyle"></param>
        private void changeWallpaper(String wallpaperName, WallpaperStyle wallStyle)
        {
            if (!String.IsNullOrEmpty(fileNames[pCurrentImage]))
            {
                Wallpaper.SetDesktopWallpaper(fileNames[pCurrentImage], wallStyle);
            }
        }

        /// <summary>
        /// Changes the wallpaper to stretch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stretchedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Stretch);
        }

        /// <summary>
        /// Changes the wallpaper to tile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Tile);
        }

        /// <summary>
        /// Changes the wallpaper to center.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Center);
        }

        /// <summary>
        /// Changes to wallpaper to Fit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Fit);
        }

        /// <summary>
        /// Changes the wallpaper to Fill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeWallpaper(fileNames[pCurrentImage], WallpaperStyle.Fill);
        }

        /// <summary>
        /// This enables the user to save the image loaded in the picture box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Provides keyboard shortcuts for traversing through the loaded images.
        /// When in window mode, support for using NumPad keys is added because an actual picture box
        /// can't be focused in WinForms and the buttons have the focus when using arrow keys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slika_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.NumPad4)
            {
                btnPrev_Click(null, null);
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.NumPad6)
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

        /// <summary>
        /// This method enables dragging and dropping images on the form, specifically on the picture box.
        /// The body of this method is similar to the actual method for opening an image with a button, 
        /// however here as file path is used a different source, rather than the openFileDialog component.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    pCurrentImage = fileNames.IndexOf(strPathToLoadedImage);

                    ShowCurrentImage();
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("Can't open file!" + "\nError: \n" + ioe.Message);
                }
            }
        }

        /// <summary>
        /// This event is raised on double click on the picture box.
        /// Enables fullscreen mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                this.Opacity = 0.0;
                TimerStart();
                viewSettings();
                toolTip1.Active = false;
            }
        }

        /// <summary>
        /// Provides a key shortcut to exit fullscreen mode.
        /// Also disables tooltip when in fullscreen.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                viewSettings();
                if (isSlideShow)
                {
                    isSlideShow = false;
                    timer2.Stop();
                }
                toolTip1.Active = true;
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        /// <summary>
        /// This is necessary since in fullscreen mode the labels and buttons need to be hidden.
        /// Also, the form, panel and picture box need to be set accordingly.
        /// </summary>
        public void viewSettings()
        {
            if (this.FullScreen.IsFullScreen)
            {
                // Unhide the buttons and restore the slideshow panel.
                label1.Show();
                label2.Show();
                btnNext.Show();
                btnOpen.Show();
                btnPrev.Show();
                this.panel1.Dock = DockStyle.None;
                pictureBox1.Height = PIC_HEIGHT;
                pictureBox1.Width = PIC_WIDTH;
                panel1.Height = PIC_HEIGHT;
                panel1.Width = PIC_WIDTH;
                FullScreen.LeaveFullScreen(this);
            }
            else
            {
                // Hide the buttons and max the slideshow panel.
                label1.Hide();
                label2.Hide();
                btnNext.Hide();
                btnOpen.Hide();
                btnPrev.Hide();
                FullScreen.EnterFullScreen(this);
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.panel1.Dock = DockStyle.Fill;
                panel1.Bounds = Screen.PrimaryScreen.Bounds;
                pictureBox1.Bounds = Screen.PrimaryScreen.Bounds;
            }
        }


        /// <summary>
        /// Rotates the image 90 degrees counterclockwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotateImage(RotateFlipType.Rotate270FlipNone);
        }

        /// <summary>
        /// Rotates the image 90 degrees clockwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotateImage(RotateFlipType.Rotate90FlipNone);
        }

        /// <summary>
        /// This method is used for rotating the currently loaded image with the passed parameter type.
        /// </summary>
        /// <param name="rotateType"></param>
        protected void rotateImage(RotateFlipType rotateType)
        {
            Bitmap rotatedImg = new Bitmap(this.pictureBox1.Image);
            if (rotatedImg != null)
            {
                rotatedImg.RotateFlip(rotateType);
                this.pictureBox1.Image = rotatedImg;
            }
        }

        /// <summary>
        /// This method provides starting the animation for viewing the current image in fullscreen.
        /// </summary>
        private void TimerStart()
        {
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 20;
            timer1.Start();
        }

        /// <summary>
        /// Intended for loading current image in fullscreen with an effect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Starts slideshow with images from loaded folder.
        /// Changes image every 2 seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                timer2.Tick += new EventHandler(timer2_Tick);
                timer2.Interval = 2000;
                isSlideShow = true;
                pictureBox1_DoubleClick(null, null);
                timer2.Start();
            }
        }

        /// <summary>
        /// Timer function for next image in slideshow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            btnNext_Click(null, null);
        }
    }
}