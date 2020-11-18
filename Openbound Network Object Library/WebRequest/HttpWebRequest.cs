using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.WebRequest
{
    public class HttpWebRequest
    {
        public static void AsyncDownloadFile(string url, string destinationFolder, Action<float> onReceiveData = default, Action onFinishDownload = default)
        {
            Thread downloadThread = new Thread(() =>
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (sender, e) => onReceiveData?.Invoke(e.BytesReceived / (float)e.TotalBytesToReceive);
                    webClient.DownloadFileCompleted += (sender, e) => onFinishDownload?.Invoke();
                    webClient.DownloadFileAsync(new Uri(url), destinationFolder);
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
