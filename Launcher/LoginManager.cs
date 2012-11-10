using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
namespace Launcher
{
    class LoginManager
    {
        public BitmapImage GetHeadIcon(string dir)
        {
            BitmapImage headBit = new BitmapImage();
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + dir + "/" + "logindata"))
            {
                headBit.BeginInit();
                string line1;
                string line2;
                string line3;
                using (StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + dir + "/" + "logindata"))
                {
                    line1 = reader.ReadLine();
                    line2 = reader.ReadLine();
                    line3 = reader.ReadLine();
                }
                headBit.UriSource = new Uri("https://minotar.net/avatar/" + line3);
                headBit.EndInit();
                return headBit;
            }
            else
            {
                headBit.BeginInit();
                headBit.UriSource = new Uri("https://minotar.net/avatar/char");
                headBit.EndInit();
                return headBit;
            }
        }
        public bool CheckLogin(string email, string pass) {
            string res = Util.generateSession(email, pass,13);
            if (res == "Bad login")
            {
                ErrorWindow error = new ErrorWindow();
                error.Error.Content = "Bad Login";
                error.Show();
                return false;
            }
            else if (res == "Servers Down" || res == "" || res == null) {
                ErrorWindow error = new ErrorWindow();
                error.Error.Content = "Minecraft login servers are down";
                error.Show();
                return false;
            }
            else if (res.Split(':').Length != 4) {
                ErrorWindow error = new ErrorWindow();
                error.Error.Content = "A unkown error has occured. It may be that the login servers are down";
                error.Show();
                return false;
            }
            else
            {
                return true;
            }
        }
        public void SaveLoginData(string email, string pass, string dir)
        {
            string res = Util.generateSession(email, pass, 13);
            string user = res.Split(':')[2];
            string[] lines = { email, pass,user };
            System.IO.File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + dir + "/" + "logindata", lines);
        }
        public void LaunchMinecraft(string email, string pass, string dir)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\"; 
            string res = Util.generateSession(email, pass, 13);
            ErrorWindow error = new ErrorWindow();
            string sesID = res.Split(':')[3];
            string user = res.Split(':')[2];
            //Login.startMinecraft(true,256,1024,user,sesID,true);
            //Debug.Write("java -Xincgc -Xmx1024m -cp \"%APPDATA%/.xylotech/.minecraft/bin/minecraft.jar;%APPDATA%/.xylotech/.minecraft/bin/lwjgl.jar;%APPDATA%/.xylotech/.minecraft/bin/lwjgl_util.jar;%APPDATA%/.xylotech/.minecraft/bin/jinput.jar\" -Djava.library.path=\"%APPDATA%/.xylotech/.minecraft/bin/natives\" net.minecraft.client.Minecraft " + user + " " + sesID);
            Debug.Write(appData + ".xylotech/" + "launch.bat");
            Process proc = new Process();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = appData + dir + '/' + "launch.bat";
            proc.StartInfo.Arguments = user + " " + sesID + " " + dir;
            //System.Diagnostics.Process.Start(appData  + ".xylotech/" + "launch.bat", user + " " + sesID);
            proc.Start();
        }
       

    }
}
