using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ionic.Zip;
using System.IO;
using System.Diagnostics;
namespace Launcher
{
    class DownloadManager
    {

        public FileStream CreateConfigs() {
            using (FileStream config = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/config", FileMode.OpenOrCreate)) { }
            FileStream logindata = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/logindata", FileMode.OpenOrCreate);
            return logindata;
        }
        public void DeleteCurrentPack() {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            foreach (string f in Directory.GetFiles(appData + "/.xylotech","*")) {
                if (f != appData + "/.xylotech\\logindata") {
                    Debug.Write(f);
                    File.Delete(f);                    
                }
            }
            foreach (string f in Directory.GetDirectories(appData + "/.xylotech")) {
                Directory.Delete(f,true);
            }
            CreateConfigs();
        }
        public  void DownloadModPack(string url) {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri("http://xylocraft.com/ModPack/" + url), appData + "/.xylotech/" + url);
                webClient.DownloadFile(new Uri("http://assets.minecraft.net/1_4_2/minecraft.jar"),appData + "/.xylotech/minecraft.zip");
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
            if(File.Exists(appData + "/.xylotech/.minecraft/bin/minecraft.jar"))
                File.Delete(appData + "/.xylotech/.minecraft/bin/minecraft.jar");
            using (ZipFile zip = ZipFile.Read(appData + "/.xylotech/minecraft.zip")) {
                foreach (string line in File.ReadAllLines(appData + "/.xylotech/modlist")) {
                    using (ZipFile modZip = ZipFile.Read(appData + "/.xylotech/instMods/" + line)) {
                        modZip.ExtractAll(appData + "/.xylotech/" + line + "/");
                    }
                    //foreach (string z in Directory.GetFiles(appData + "/.xylotech/" + line, "*", SearchOption.AllDirectories))
                    //    zip.UpdateFile(z, "");
                    zip.UpdateDirectory(appData + "/.xylotech/" + line,"");
                }
                List<ZipEntry> delete = new List<ZipEntry>();
                foreach (ZipEntry e in zip.Entries) {
                    if (e.FileName.Contains("META-INF")) {
                        Debug.Write("Found IT!");
                        delete.Add(e);
                    }
                }
                foreach (ZipEntry e in delete) {
                    zip.RemoveEntry(e);
                }
                zip.Save(appData + "/.xylotech/minecraft.zip");
            }
           File.Move(appData + "/.xylotech/minecraft.zip", appData + "/.xylotech/.minecraft/bin/minecraft.jar");
        }
    }
} 