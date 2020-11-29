using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.WebRequest
{
    public class HttpWebRequest
    {
        public static void AsyncDownloadFile(string url, string destinationPath, Action<float, long, long> onReceiveData = default, Action onFinishDownload = default, Action<Exception> onFailToDownload = default)
        {
            //Create the directory the files are being downloaded at
            string directory = destinationPath.Split('\\').Last();
            directory = destinationPath.Replace(@$"\{directory}", "");

            Directory.CreateDirectory(directory);

            //Initialize the download thread
            Thread downloadThread = new Thread(() =>
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        int lastPercentage = 0;

                        webClient.DownloadProgressChanged += (sender, e) =>
                        {
                            if (lastPercentage != e.ProgressPercentage)
                            {
                                lastPercentage = e.ProgressPercentage;
                                onReceiveData?.Invoke(lastPercentage, e.BytesReceived, e.TotalBytesToReceive);
                            }

                            if (e.ProgressPercentage >= 100)
                            {
                                onFinishDownload?.Invoke();
                            }
                        };

                        webClient.DownloadFileCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                onFailToDownload?.Invoke(e.Error);
                                return;
                            }
                        };

                        webClient.DownloadFileAsync(new Uri(url), destinationPath);
                    }
                }
                catch (Exception e)
                {
                    onFailToDownload?.Invoke(e);
                }
            });

            downloadThread.IsBackground = true;
            downloadThread.Start();
        }

        public static void AsyncDownloadJsonObject<T>(string url, Action<T> onFinishDownload)
        {
            Thread downloadThread = new Thread(() =>
            {
                using (WebClient webClient = new WebClient())
                {
                    string jsonObject = webClient.DownloadString(new Uri(url));
                    onFinishDownload?.Invoke(ObjectWrapper.Deserialize<T>(jsonObject));
                }
            });

            downloadThread.IsBackground = true;
            downloadThread.Start();
        }
    }
}
