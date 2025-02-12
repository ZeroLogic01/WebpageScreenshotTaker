using log4net;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ScreenshotTaker.Service.Jobs.Helper
{
    internal static class ScreenshotCapturer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ScreenshotCapturer));

        /// <summary>
        /// Takes the screenshot of given webpage &amp; save it in a directory.
        /// </summary>
        /// <param name="url">url of which screenshot to be taken.</param>
        /// <param name="directory">Where image file to be saved.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        internal static async Task DownloadWebpageScreenshot(string url, string directory, string imageName)
        {
            try
            {
                await Task.Run(() =>
                {
                    string link = Url2png(url);

                    _log.Debug($"Downloading {imageName} from {link}");

                    var request = WebRequest.Create(link) as HttpWebRequest;
                    Bitmap bitmap;
                    using (Stream stream = request.GetResponse().GetResponseStream())
                    {
                        bitmap = new Bitmap(stream);
                    }

                    // In-case user has accidently deleted the save directory
                    Directory.CreateDirectory(directory);

                    string imagefileFullPath = Path.Combine(directory, $"{imageName} {DateTime.Now.ToLocalTime().ToString("yyyy-M-dd hh-mm-ss")}.png");

                    bitmap.Save(imagefileFullPath, ImageFormat.Png);
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private static string Url2png(string UrlToSite)
        {
            string url2pngAPIKey = Environment.GetEnvironmentVariable("url2pngAPIKey");
            string url2pngPrivateKey = Environment.GetEnvironmentVariable("url2pngPrivateKey");

            string url = HttpUtility.UrlEncode(UrlToSite);

            string getstring = "fullpage=true&url=" + url;

            string SecurityHash_url2png = Md5HashPHPCompliant(url2pngPrivateKey + "+" + getstring).ToLower();

            var url2pngLink = "http://api.url2png.com/v6/" + url2pngAPIKey + "/" + SecurityHash_url2png + "/" + "png/?" + getstring;

            return url2pngLink;
        }


        private static string Md5HashPHPCompliant(string pass)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] dataMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= dataMd5.Length - 1; i++)
            {
                sb.AppendFormat("{0:x2}", dataMd5[i]);
            }

            return sb.ToString();

        }
    }
}
