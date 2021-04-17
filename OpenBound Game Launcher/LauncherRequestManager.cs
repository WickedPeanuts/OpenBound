using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Forms;
using OpenBound_Game_Launcher.Forms.GenericLoadingScreen;
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
        public void PrepareLoginThread(LoginLoadingScreen loadingScreen, string nickname, string password)
        {
            RequestThread = new Thread(() => Login(loadingScreen, nickname, password));
            RequestThread.IsBackground = true;
            RequestThread.Start();
        }

        public void Login(LoginLoadingScreen loadingScreen, string nickname, string password)
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
                            player = ObjectWrapper.Deserialize<Player>(message[1]);

                            if (player == null)
                            {
                                waiting = true;
                                return;
                            }
                        }
                        else
                        {
                            List<int> idList = ObjectWrapper.Deserialize<List<int>>(message[2]);

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

                csp.OnFailToEstabilishConnection += loadingScreen.OnFailToStablishConnection;
                csp.StartOperation();
                csp.RequestQueue.Enqueue(NetworkObjectParameters.LoginServerLoginAttemptRequest, player);

                while (!waiting)
                    Thread.Sleep(100);

                csp.StopOperation();

                if (player == null || player.ID == 0)
                {
                    loadingScreen.OnFailToFindPlayer();
                    return;
                }

                player.LoadOwnedAvatarDictionary();

                //Success
                Parameter.Player = player;
                loadingScreen.Close(DialogResult.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }
        #endregion

        #region Sign Up Form
        public void PrepareRegistrationThread(SignUpLoadingScreen signUpLoadingForm, PlayerDTO account)
        {
            RequestThread = new Thread(() => Register(signUpLoadingForm, account));
            RequestThread.IsBackground = true;
            RequestThread.Start();
        }

        public void Register(SignUpLoadingScreen signUpLoadingScreen, PlayerDTO account)
        {
            try
            {
                bool waiting = false;

                Player newPlayer = null;

                ClientServiceProvider csp = new ClientServiceProvider(
                    NetworkObjectParameters.LoginServerInformation.ServerPublicAddress,
                    NetworkObjectParameters.LoginServerInformation.ServerPort,
                    NetworkObjectParameters.LoginServerBufferSize,
                    (serviceProvider, message) =>
                    {
                        newPlayer = ObjectWrapper.Deserialize<Player>(message[1]);
                        waiting = true;
                    });
                csp.OnFailToEstabilishConnection += signUpLoadingScreen.OnFailToStablishConnection;

                csp.StartOperation();
                csp.RequestQueue.Enqueue(NetworkObjectParameters.LoginServerAccountCreationRequest, account);

                while (!waiting)
                    Thread.Sleep(100);

                csp.StopOperation();

                if (newPlayer == null)
                {
                    //Unnable to create player (unknown reason)
                    signUpLoadingScreen.OnFailToCreateAccount();
                    return;
                }

                string errorMessage = "";

                //Invalid Nickname
                if (newPlayer.Nickname == null)
                    errorMessage += Language.RegisterFailureNickname;

                //Invalid Email
                if (newPlayer.Email == null)
                    errorMessage += Language.RegisterFailureEmail;
                
                //Fail
                if (errorMessage.Length > 0)
                {
                    signUpLoadingScreen.OnFailToCreateAccount();
                    return;
                }

                //Success
                signUpLoadingScreen.OnCreateAccount();
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
