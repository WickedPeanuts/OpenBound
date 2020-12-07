using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Forms
{
    public partial class LoadingMenu : Form
    {
        public AsynchronousAction animationTickAction, timer1TickAction;

        public Bitmap[] animationFrames = new Bitmap[]
        {
            Resources.RaonLauncherMine_0, Resources.RaonLauncherMine_1,
            Resources.RaonLauncherMine_2, Resources.RaonLauncherMine_3,
            Resources.RaonLauncherMine_4, Resources.RaonLauncherMine_5,
            Resources.RaonLauncherMine_6, Resources.RaonLauncherMine_7,
            Resources.RaonLauncherMine_8, Resources.RaonLauncherMine_9,
            Resources.RaonLauncherMine_10, Resources.RaonLauncherMine_11,
        };

        public int animationFrameIndex = 1;

        public LoadingMenu()
        {
            InitializeComponent();
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            animationFrameIndex = (animationFrameIndex + 1) % 12;
            pictureBox1.Image = animationFrames[animationFrameIndex];
        }

        public void StartAsyncAction()
        {

        }
    }
}
