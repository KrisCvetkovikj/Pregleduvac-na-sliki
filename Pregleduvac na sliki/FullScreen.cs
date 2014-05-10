using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pregleduvac_na_sliki
{
    public class FullScreen
    {

        private FormWindowState winState;
        private FormBorderStyle brdStyle;
        private bool topMost;
        private Rectangle bounds;

        /// <summary>
        /// Constructor for FullScreen.
        /// </summary>
        public FullScreen()
        {
            IsFullScreen = false;
        }

        public bool IsFullScreen
        {
            get;
            set;
        }

        /// <summary>
        /// Maximize the window to the full screen.
        /// </summary>
        /// <param name="targetForm"></param>
        public void EnterFullScreen(Form targetForm)
        {
            if (!IsFullScreen)
            {
                // Save the original form state.
                Save(targetForm);

                targetForm.WindowState = FormWindowState.Maximized;
                targetForm.FormBorderStyle = FormBorderStyle.None;
                targetForm.TopMost = true;
                targetForm.Bounds = Screen.GetBounds(targetForm);

                IsFullScreen = true;
            }
        }

        /// <summary>
        /// Save the current Window state.
        /// </summary>
        /// <param name="targetForm"></param>
        private void Save(Form targetForm)
        {
            winState = targetForm.WindowState;
            brdStyle = targetForm.FormBorderStyle;
            topMost = targetForm.TopMost;
            bounds = targetForm.Bounds;
        }

        /// <summary>
        /// Leave the full screen mode and restore the original window state.
        /// </summary>
        /// <param name="targetForm"></param>
        public void LeaveFullScreen(Form targetForm)
        {
            if (IsFullScreen)
            {
                // Restore the original Window state.
                targetForm.WindowState = winState;
                targetForm.FormBorderStyle = brdStyle;
                targetForm.TopMost = topMost;
                targetForm.Bounds = bounds;
                IsFullScreen = false;
            }
        }
    }
}