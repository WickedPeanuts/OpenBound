using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Forms
{
    public enum MenuState
    {
        ReadyToDownload,
        Downloading,
        Installing,
        Done,
    }

    public partial class GameUpdater : Form
    {
        private MenuState menuState;

        public AsynchronousAction TickAction;

        public GameUpdater()
        {
            TickAction = new AsynchronousAction();

            InitializeComponent();
        }

        private void GameUpdater_Load(object sender, EventArgs e)
        {
            downloadLabel2.Text = downloadLabel1.Text = "";

            UpdateMenuState(MenuState.ReadyToDownload);
        }

        private void UpdateMenuState(MenuState menuState)
        {
            switch (this.menuState = menuState)
            {
                case MenuState.ReadyToDownload:
                    abortButton.Enabled = false;
                    updateButton.Enabled = true;
                    break;
                case MenuState.Downloading:
                    abortButton.Enabled = true;
                    updateButton.Enabled = false;
                    break;
                case MenuState.Installing:
                    abortButton.Enabled = false;
                    updateButton.Enabled = false;
                    break;
                case MenuState.Done:
                    abortButton.Enabled = false;
                    updateButton.Enabled = false;
                    break;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            HttpWebRequest.AsyncDownloadJsonObject<object>(
                "http://www.json-generator.com/api/json/get/cglxbuiiUO?indent=2",
                (o) =>
                {
                    MessageBox.Show(ObjectWrapper.Serialize(o));
                });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TickAction.AsynchronousInvokeAndDestroy();
        }
    }
}
