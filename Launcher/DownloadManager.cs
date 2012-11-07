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
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri("http://xylocraft.com/ModPack/" + url), appData + "/.xylotech/" + url);
                webClient.DownloadFile(new Uri("http://assets.minecraft.net/1_4_2/minecraft.jar"),appData + "/.xylotech/minecraft.jar");
                if (!File.Exists(appData + "/.xylotech/launch.bat"))
                {
                    webClient.DownloadFile("http://xylocraft.com/launch.bat", appData + "/.xylotech/launch.bat");
                }
               
            }
            
            string[] lines = { "true", "" };
            System.IO.File.WriteAllLines(appData + "/.xylotech/config", lines);
        }
        public void InstallModPack(string zipName) {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            using (ZipFile zip = ZipFile.Read(appData + "/.xylotech/" + zipName))
            {
                zip.ExtractAll(appData + "/.xylotech/");
            }
            if(File.Exists(appData + "/.xylotech/.minecraft/minecraft.jar") == true){
                File.Delete(appData + "/.xylotech/.minecraft/minecraft.jar");
            }
            using (ZipFile zip = ZipFile.Read(appData + "/.xylotech/minecraft.jar")) {
                zip.ExtractAll(appData + "/.xylotech/minecraftT.jar");
            }
            foreach(string line in File.ReadAllLines(appData + "/.xylotech/modlist")){
                using(ZipFile zip = ZipFile.Read(appData + "/.xylotech/modlist/" + line)){
                    zip.ExtractAll(appData + "/.xylotech/minecraftT.jar");
                }
            }
            using(ZipFile zip = new ZipFile()){
                 foreach(string f in Directory.GetDirectories(appData + "/.xylotech/minecraftT.jar")){
                    zip.AddFile(f); 
                 }
                 zip.Save(appData + "/.xylotech/.minecraft/minecraft.jar");
            }
        }
    }
}
