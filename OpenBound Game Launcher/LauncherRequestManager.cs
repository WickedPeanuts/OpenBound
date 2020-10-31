using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Forms;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.TCP.ServiceProvider;
using OpenBound_Network_Object_Library.ValidationModel;
using Language = OpenBound_Game_Launcher.Common.Language;

namespace OpenBound_Game_Launcher.Launcher.Connection
{
    public class LauncherRequestManager
    {
        private Thread RequestThread;

        public LauncherRequestManager() { }

        #region Game Launcher
        public void PrepareLoginThread(GameLauncher gameLauncher, string nickname, string password)
        {
            RequestThread = new Thread(() => Login(gameLauncher, nickname, password));
            RequestThread.IsBackground = true;
            RequestThread.Start();
        }

        public void Login(GameLauncher gameLauncher, string nickname, string password)
        {
            try
            {
                //Preparing player variable
                Player player = new Player()
                {
                    Nickname = nickname,
                    Password = password
                };

                bool waiting = false;

                ClientServiceProvider csp = new ClientServiceProvider(
                    NetworkObjectParameters.LoginServerInformation.ServerPublicAddress,
                    NetworkObjectParameters.LoginServerInformation.ServerPort,
                    NetworkObjectParameters.LoginServerBufferSize,
                    (serviceProvider, message) =>
                    {
                        if (message.Length == 2)
                        {
                            player = ObjectWrapper.DeserializeRequest<Player>(message[1]);

                            if (player == null)
                            {
                                waiting = true;
                                return;
                            }
                        }
                        else
                        {
                            List<int> idList = ObjectWrapper.DeserializeRequest<List<int>>(message[2]);

                            if (idList == null)
                            {
                                waiting = true;
                                return;
                            }

                            foreach (int i in idList)
                            {
                                player.PlayerAvatarMetadataList.Add(new PlayerAvatarMetadata()
                                {
                                    Player = player,
                                    AvatarMetadata = new AvatarMetadata()
                                    {
                                        ID = i,
                                        AvatarCategory = (AvatarCategory)int.Parse(message[1])
                                    },
                                });
                            }
                        }
                    });

                csp.OnFailToEstabilishConnection += () =>
                {
                    Feedback.CreateWarningMessageBox(Language.FailToEstabilishConnection);
                    OnFailToEstabilishConnection(gameLauncher);
                };
                csp.StartOperation();
                csp.RequestQueue.Enqueue(NetworkObjectParameters.LoginServerLoginAttemptRequest, player);

                while (!waiting) Thread.Sleep(100);

                csp.StopOperation();

                if (player == null || player.ID == 0)
                {
                    //Error
                    Feedback.CreateWarningMessageBox(Language.PlayerNotFound);
                    OnFailToEstabilishConnection(gameLauncher);

                    return;
                }

                player.LoadOwnedAvatarDictionary();

                //Success
                gameLauncher.TickAction += () => { gameLauncher.Close(); };
                Parameter.Player = player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void OnFailToEstabilishConnection(GameLauncher gameLauncher)
        {
            gameLauncher.TickAction = new Action(() =>
            {
                gameLauncher.SetEnableTextBox(true);
                gameLauncher.SetEnableInterfaceButtons(true);
            });
        }
        #endregion

        #region Sign Up Form
        public void PrepareRegistrationThread(SignUpForm signUpForm, PlayerDTO account)
        {
            RequestThread = new Thread(() => Register(signUpForm, account));
            RequestThread.IsBackground = true;
            RequestThread.Start();
        }

        public void Register(SignUpForm signUpForm, PlayerDTO account)
        {
            try
            {
                bool waiting = false;

                Player newPlayer = null;

                ClientServiceProvider csp = new ClientServiceProvider(
                    NetworkObjectParameters.LoginServerInformation.ServerLocalAddress,
                    NetworkObjectParameters.LoginServerInformation.ServerPort,
                    NetworkObjectParameters.LoginServerBufferSize,
                    (serviceProvider, message) =>
                    {
                        newPlayer = ObjectWrapper.DeserializeRequest<Player>(message[1]);
                        waiting = true;
                    });
                csp.OnFailToEstabilishConnection += () =>
                {
                    Feedback.CreateWarningMessageBox(Language.FailToEstabilishConnection);
                    OnFailToEstabilishConnection(signUpForm);
                };
                csp.StartOperation();
                csp.RequestQueue.Enqueue(NetworkObjectParameters.LoginServerAccountCreationRequest, account);

                while (!waiting)
                    Thread.Sleep(100);

                csp.StopOperation();

                bool wasAccountCreated = true;

                if (newPlayer == null)
                {
                    //Unnable to create player (unknown reason)
                    signUpForm.TickAction += () => signUpForm.SetEnableInterfaceElements(true);
                    Feedback.CreateWarningMessageBox(Language.RegisterFailureMessage);
                    return;
                }

                string errorMessage = "";
                if (newPlayer.Nickname == null)
                {
                    //Invalid Nickname
                    errorMessage += Language.RegisterFailureNickname;
                    wasAccountCreated = false;
                }

                if (newPlayer.Email == null)
                {
                    //Invalid Email
                    errorMessage += Language.RegisterFailureEmail;
                    wasAccountCreated = false;
                }

                if (wasAccountCreated)
                {
                    //Success
                    signUpForm.TickAction += () => signUpForm.Close();
                    Feedback.CreateInformationMessageBox($"{Language.RegisterSuccess}");
                }
                else
                {
                    signUpForm.TickAction += () => signUpForm.SetEnableInterfaceElements(true);
                    Feedback.CreateWarningMessageBox($"{Language.RegisterFailureMessage}{errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void OnFailToEstabilishConnection(SignUpForm signUpForm)
        {
            signUpForm.TickAction = new Action(() =>
            {
                signUpForm.SetEnableInterfaceElements(true);
            });
        }
        #endregion
    }
}
