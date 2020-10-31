using OpenBound_Game_Launcher.Common;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.FileOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Language = OpenBound_Game_Launcher.Common.Language;

namespace OpenBound_Game_Launcher.Forms
{
    public partial class GameSettings : Form
    {
        Dictionary<string, List<string>> supportedResolutionDictionary = new Dictionary<string, List<string>>()
        {
            { "4 : 3", new List<string>(){ "1024 x 768", "1280 x 960", "1400 x 1050", "1440 x 1080", "1600 x 1200", "1856 x 1392", "1920 x 1440", "2048 x 1536" } },
            { "16 : 9", new List<string>(){ "1280 x 720", "1366 x 768", "1600 x 900", "1920 x 1080", "2560 x 1440", "3840 x 2160", "7680 x 4320" } },
            { "16 : 10", new List<string>(){ "1280 x 800", "1440 x 900", "1680 x 1050", "1920 x 1200", "2560 x 1600" } },
        };

        List<string> msaaType = new List<string>() { "MSAA x 2", "MSAA x 4", "MSAA x 8" };

        public GameSettings()
        {
            InitializeComponent();
            FormClosed += (e, args) => SaveConfigurations();
        }

        private string FindAspectRatio(string resolution)
        {
            foreach(KeyValuePair<string, List<string>> kvp in supportedResolutionDictionary)
            {
                if (kvp.Value.Exists((x) => x == resolution))
                    return kvp.Key;
            };

            return Language.NoAspectRatioFound;
        }

        private void GameSettings_Load(object sender, EventArgs e)
        {
            //Loading field settings

            //Launcher Settings
            nicknameTextBox.Text = Parameter.GameClientSettingsInformation.SavedNickname;
            loginServerIPTextBox.Text = NetworkObjectParameters.LoginServerInformation.ServerPublicAddress;
            loginServerPortTextBox.Text = $"{NetworkObjectParameters.LoginServerInformation.ServerPort}";
            lobbyServerIPTextBox.Text = NetworkObjectParameters.LobbyServerInformation.ServerPublicAddress;
            lobbyServerPortTextBox.Text = $"{NetworkObjectParameters.LobbyServerInformation.ServerPort}";
            fetchServerIPTextBox.Text = "127.0.0.1";
            fetchServerPortTextBox.Text = "0";

            //Graphic Settings
            //-- Windowed Settings
            windowedAspectRatioTextBox.Items.AddRange(supportedResolutionDictionary.Keys.ToArray());
            windowedResolution.Text = $"{Parameter.GameClientSettingsInformation.WindowedWidth} x {Parameter.GameClientSettingsInformation.WindowedHeight}";
            windowedAspectRatioTextBox.Text = FindAspectRatio(windowedResolution.Text);
            isWindowedCheckbox.Checked = !Parameter.GameClientSettingsInformation.IsFullScreen;
            isWindowedCheckbox.CheckedChanged += (e, args) => RefreshWindowCheck();

            RefreshWindowCheck();

            //-- Menu Settings
            menuAspectRatioComboBox.Text = "4 : 3";
            menuResolutionComboBox.Text = "1024 x 768";

            //-- In-Game Settings
            inGameAspectRatioComboBox.Items.AddRange(supportedResolutionDictionary.Keys.ToArray());
            inGameResolutionComboBox.Text  = $"{Parameter.GameClientSettingsInformation.InGameResolutionWidth} x {Parameter.GameClientSettingsInformation.InGameResolutionHeight}";
            inGameAspectRatioComboBox.Text = FindAspectRatio(inGameResolutionComboBox.Text);

            //-- Events
            RefreshGameSettingsFields(null);

            windowedAspectRatioTextBox.SelectedIndexChanged += (e, args) => RefreshGameSettingsFields(e);
            inGameAspectRatioComboBox.SelectedIndexChanged += (e, args) => RefreshGameSettingsFields(e);

            windowedResolution.Text = $"{Parameter.GameClientSettingsInformation.WindowedWidth} x {Parameter.GameClientSettingsInformation.WindowedHeight}";
            inGameResolutionComboBox.Text = $"{Parameter.GameClientSettingsInformation.InGameResolutionWidth} x {Parameter.GameClientSettingsInformation.InGameResolutionHeight}";

            isWindowedCheckbox.CheckedChanged += (e, args) => NotifyAboutBadResolution();
            windowedAspectRatioTextBox.SelectedIndexChanged += (e, args) => NotifyAboutBadResolution();
            windowedResolution.SelectedIndexChanged += (e, args) => NotifyAboutBadResolution();
            inGameAspectRatioComboBox.SelectedIndexChanged += (e, args) => NotifyAboutBadResolution();
            inGameResolutionComboBox.SelectedIndexChanged += (e, args) => NotifyAboutBadResolution();

            //Graphical Enhancements
            //-- Anti-Aliasing
            isAntiAliasingOnCheckbox.Checked = Parameter.GameClientSettingsInformation.IsMultiSamplingEnabled;
            isAntiAliasingOnCheckbox.CheckedChanged += (e, args) => RefreshAntiAliasing();
            RefreshAntiAliasing();

            antiAliasingSamplesComboBox.Text = $"MSAA x {Parameter.GameClientSettingsInformation.MultiSamplingCount}";
            antiAliasingSamplesComboBox.Items.AddRange(msaaType.ToArray());

            //VSync
            isVSyncOnCheckbox.Checked = Parameter.GameClientSettingsInformation.IsVSyncOn;

            //Game Settings
            //--Background
            shouldRenderBackgroundCheckBox.Checked = Parameter.GameClientSettingsInformation.IsBackgroundOn;
            shouldRenderBackgroundCheckBox.CheckedChanged += (e, args) => RefreshRenderBackground();
            RefreshRenderBackground();


            redBackgroundColorTextBox.Text   = $"{Parameter.GameClientSettingsInformation.BackgroundColorRedChannel}";
            greenBackgroundColorTextBox.Text = $"{Parameter.GameClientSettingsInformation.BackgroundColorGreenChannel}";
            blueBackgroundColorTextBox.Text  = $"{Parameter.GameClientSettingsInformation.BackgroundColorBlueChannel}";

            bgmTrackBar.Value         = (int)(bgmTrackBar.Maximum * (Parameter.GameClientSettingsInformation.BGM / 100f));
            soundEffectTrackBar.Value = (int)(soundEffectTrackBar.Maximum * (Parameter.GameClientSettingsInformation.SFX / 100f));
        }

        #region OnChangeFields
        private void RefreshWindowCheck()
        {
            windowedResolution.Enabled = windowedAspectRatioTextBox.Enabled = isWindowedCheckbox.Checked;
        }

        private void RefreshGameSettingsFields(object e)
        {
            if (e == null || e == windowedAspectRatioTextBox)
            {
                windowedResolution.Items.Clear();

                if (supportedResolutionDictionary.ContainsKey(windowedAspectRatioTextBox.Text))
                {
                    windowedResolution.Items.AddRange(supportedResolutionDictionary[windowedAspectRatioTextBox.Text].ToArray());
                    windowedResolution.ResetText();
                    windowedResolution.SelectedItem = windowedResolution.Items[0];
                }
            }
            
            if (e == null || e == inGameAspectRatioComboBox)
            {
                inGameResolutionComboBox.Items.Clear();
                if (supportedResolutionDictionary.ContainsKey(inGameAspectRatioComboBox.Text))
                {
                    inGameResolutionComboBox.Items.AddRange(supportedResolutionDictionary[inGameAspectRatioComboBox.Text].ToArray());
                    inGameResolutionComboBox.ResetText();
                    inGameResolutionComboBox.SelectedItem = inGameResolutionComboBox.Items[0];
                }
            }
        }

        private void RefreshAntiAliasing()
        {
            antiAliasingSamplesComboBox.Enabled = isAntiAliasingOnCheckbox.Checked;
        }

        private void RefreshRenderBackground()
        {
            redBackgroundColorTextBox.Enabled = greenBackgroundColorTextBox.Enabled =
                blueBackgroundColorTextBox.Enabled = shouldRenderBackgroundCheckBox.Checked;
        }
        #endregion

        private void NotifyAboutBadResolution()
        {
            string[] windowedRes = windowedResolution.Text.Split(" x ");
            string[] ingameRes = inGameResolutionComboBox.Text.Split(" x ");

            if ((int.Parse(windowedRes[0]) < int.Parse(ingameRes[0]) ||
                int.Parse(windowedRes[1]) < int.Parse(ingameRes[1])) &&
                isWindowedCheckbox.Checked)
                Helper.Feedback.CreateWarningMessageBox(Language.BadResolutionSelection);
        }

        private void SaveConfigurations()
        {
            //Launcher Settings
            Parameter.GameClientSettingsInformation.SavedNickname = nicknameTextBox.Text;
            NetworkObjectParameters.LoginServerInformation.ServerPublicAddress = loginServerIPTextBox.Text;
            NetworkObjectParameters.LoginServerInformation.ServerPort = int.Parse(loginServerPortTextBox.Text);
            NetworkObjectParameters.LobbyServerInformation.ServerPublicAddress = lobbyServerIPTextBox.Text;
            NetworkObjectParameters.LobbyServerInformation.ServerPort = int.Parse(lobbyServerPortTextBox.Text);
            
            //fetchServerIPTextBox.Text = "127.0.0.1";
            //fetchServerPortTextBox.Text = "0";

            //Graphic Settings
            //-- Windowed Settings
            string[] windowedRes = windowedResolution.Text.Split(" x ");
            Parameter.GameClientSettingsInformation.WindowedWidth = int.Parse(windowedRes[0]);
            Parameter.GameClientSettingsInformation.WindowedHeight = int.Parse(windowedRes[1]);
            Parameter.GameClientSettingsInformation.IsFullScreen = !isWindowedCheckbox.Checked;

            //-- In-Game Settings
            string[] ingameRes = inGameResolutionComboBox.Text.Split(" x ");
            Parameter.GameClientSettingsInformation.InGameResolutionWidth = int.Parse(ingameRes[0]);
            Parameter.GameClientSettingsInformation.InGameResolutionHeight = int.Parse(ingameRes[1]);

            //Graphical Enhancements
            //-- Anti-Aliasing
            Parameter.GameClientSettingsInformation.IsMultiSamplingEnabled = isAntiAliasingOnCheckbox.Checked;
            Parameter.GameClientSettingsInformation.MultiSamplingCount = int.Parse(antiAliasingSamplesComboBox.Text.Split(" x ")[1]);

            //VSync
            Parameter.GameClientSettingsInformation.IsVSyncOn = isVSyncOnCheckbox.Checked;

            //Game Settings
            //--Background
            Parameter.GameClientSettingsInformation.IsBackgroundOn = shouldRenderBackgroundCheckBox.Checked;
            Parameter.GameClientSettingsInformation.BackgroundColorRedChannel = int.Parse(redBackgroundColorTextBox.Text);
            Parameter.GameClientSettingsInformation.BackgroundColorGreenChannel = int.Parse(greenBackgroundColorTextBox.Text);
            Parameter.GameClientSettingsInformation.BackgroundColorBlueChannel = int.Parse(blueBackgroundColorTextBox.Text);

            Parameter.GameClientSettingsInformation.BGM = (int)Math.Clamp((bgmTrackBar.Value / (float)bgmTrackBar.Maximum) * 100f, 0, 100);
            Parameter.GameClientSettingsInformation.SFX = (int)Math.Clamp((soundEffectTrackBar.Value / (float)soundEffectTrackBar.Maximum) * 100f, 0, 100);

            ConfigFileManager.OverwriteGameServerSettings(Parameter.GameClientSettingsInformation);
            
            ConfigFileManager.OverwriteLauncherServerSettings(new List<ServerInformation>() {
                NetworkObjectParameters.LoginServerInformation,
                NetworkObjectParameters.LobbyServerInformation
            });
        }
    }
}
