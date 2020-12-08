using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Forms.GenericLoadingScreen
{
    public partial class LoadingMenu : Form
    {
        protected AsynchronousAction animationTickAction, 
            timer1InvokeAndDestroyTickAction, timer1TickAction;

        protected int animationFrameIndex = 1;

        protected readonly Bitmap[] ANIMATION_FRAMES = new Bitmap[]
        {
            Resources.RaonLauncherMine_0, Resources.RaonLauncherMine_1,
            Resources.RaonLauncherMine_2, Resources.RaonLauncherMine_3,
            Resources.RaonLauncherMine_4, Resources.RaonLauncherMine_5,
            Resources.RaonLauncherMine_6, Resources.RaonLauncherMine_7,
            Resources.RaonLauncherMine_8, Resources.RaonLauncherMine_9,
            Resources.RaonLauncherMine_10, Resources.RaonLauncherMine_11,
        };

        public LoadingMenu()
        {
            InitializeComponent();

            timer1InvokeAndDestroyTickAction = new AsynchronousAction();
            timer1TickAction = new AsynchronousAction();
        }

        public virtual void timer1_Tick(object sender, EventArgs e) {
            timer1InvokeAndDestroyTickAction.AsynchronousInvokeAndDestroy();
            timer1TickAction.AsynchronousInvoke();
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            animationFrameIndex = (animationFrameIndex + 1) % 12;
            pictureBox1.Image = ANIMATION_FRAMES[animationFrameIndex];
        }

        public void Close(DialogResult dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
