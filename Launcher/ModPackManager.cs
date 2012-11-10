using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ionic.Zip;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Xml;
using System.Windows;
namespace Launcher
{
    class ModPackManager 
    {

        public FileStream CreateConfigs(string dir) {
            using (FileStream config = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + dir + "/config", FileMode.OpenOrCreate)) { }
            try
            {
                FileStream logindata = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + dir + "/logindata", FileMode.OpenOrCreate);
                return logindata;
            }
            catch {
                ErrorWindow error = new ErrorWindow();
                error.Error.Content = "You Clicked so fast the app crashed, or someone set the logindata file to read only";
                error.Error.Width = 480;
                error.Width = 480;
                error.Show();
                return null;
            }
        }
        public void ParseModPackFile(string xml, out List<string[]> tempArrays, out List<TextBlock> tempTexts) {
            tempTexts = new List<TextBlock>();
            tempArrays = new List<string[]>();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    StringBuilder tempString = new StringBuilder();
                    if (reader.Name == "modpack")
                    {
                       
                        string[] tempArray = new string[7];
                        reader.MoveToAttribute("name");
                        tempArray[4] = reader.Value;
                        tempString.Append(reader.Value);
                        reader.MoveToAttribute("author");
                        tempString.Append(" By " + reader.Value);
                        reader.MoveToAttribute("description");
                        tempString.AppendLine("");
                        tempString.Append(reader.Value);
                        TextBlock tempText = new TextBlock();
                        tempText.Text = tempString.ToString();
                        tempText.Width = 250;
                        tempText.TextWrapping = TextWrapping.Wrap;
                        reader.MoveToAttribute("id");
                        tempArray[0] = reader.Value;
                        reader.MoveToAttribute("newsURL");
                        tempArray[1] = tempString.ToString();
                        tempArray[2] = reader.Value;
                        reader.MoveToAttribute("image");
                        tempArray[3] = reader.Value;
                        reader.MoveToAttribute("url");
                        tempArray[5] = reader.Value;
                        reader.MoveToAttribute("dir");
                        tempArray[6] = reader.Value;
                        Debug.Write(reader.Value);
                        tempTexts.Add(tempText);
                        tempArrays.Add(tempArray);
                    }
                }
            } 
        }
        public void DeleteCurrentPack(string dir) {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            foreach (string f in Directory.GetFiles(appData + "/" + dir + "","*")) {
                if (f != appData + "/" + dir + "\\logindata") {
                    Debug.Write(f);
                    File.Delete(f);                    
                }
            }
            foreach (string f in Directory.GetDirectories(appData + "/" + dir + "")) {
                Directory.Delete(f,true);
            }
            CreateConfigs(dir);
        }
        public  void DownloadModPack(string url, string dir) {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri("http://xylocraft.com/ModPack/" + url), appData + "/" + dir + "/" + url);
                webClient.DownloadFile(new Uri("http://assets.minecraft.net/1_4_2/minecraft.jar"),appData + "/" + dir + "/minecraft.zip");
                if (!File.Exists(appData + "/" + dir + "/launch.bat"))
                {
                    webClient.DownloadFile("http://xylocraft.com/launch.bat", appData + "/" + dir + "/launch.bat");
                }
               
            }
            
            string[] lines = { "true", "" };
            System.IO.File.WriteAllLines(appData + "/" + dir + "/config", lines);
        }
        public void InstallModPack(string zipName, string dir) {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            using (ZipFile zip = ZipFile.Read(appData + "/" + dir + "/" + zipName))
            {
                zip.ExtractAll(appData + "/" + dir + "/");
            }
            if(File.Exists(appData + "/" + dir + "/.minecraft/bin/minecraft.jar"))
                File.Delete(appData + "/" + dir + "/.minecraft/bin/minecraft.jar");
            using (ZipFile zip = ZipFile.Read(appData + "/" + dir + "/minecraft.zip")) {
                foreach (string line in File.ReadAllLines(appData + "/" + dir + "/modlist")) {
                    using (ZipFile modZip = ZipFile.Read(appData + "/" + dir + "/instMods/" + line)) {
                        modZip.ExtractAll(appData + "/" + dir + "/" + line + "/");
                    }
                    //foreach (string z in Directory.GetFiles(appData + "/" + dir + "/" + line, "*", SearchOption.AllDirectories))
                    //    zip.UpdateFile(z, "");
                    zip.UpdateDirectory(appData + "/" + dir + "/" + line,"");
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
                zip.Save(appData + "/" + dir + "/minecraft.zip");
            }
           File.Move(appData + "/" + dir + "/minecraft.zip", appData + "/" + dir + "/.minecraft/bin/minecraft.jar");
        }
    }
} 