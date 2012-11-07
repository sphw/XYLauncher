using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ionic.Zip;
using System.IO;
namespace Launcher
{
    class DownloadManager
    {
        public  void DownloadModPack(string url) {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri("http://xylocraft.com/ModPack/" + url), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/" + url);
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/launch.bat"))
                {
                    webClient.DownloadFile("http://xylocraft.com/launch.bat", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/launch.bat");
                }
               
            }
            
            string[] lines = { "true", "" };
            System.IO.File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/config", lines);
        }
        public void InstallModPack(string zipName) {
            using (ZipFile zip = ZipFile.Read(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/" + zipName))
            {
                zip.ExtractAll(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/");
            }
        }
    }
}
