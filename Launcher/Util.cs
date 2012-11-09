using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Additional includes
using System.IO;
using System.Net;
using System.Diagnostics;


namespace Launcher
{
    public class Util
    {
        /// <summary>
        /// Used to generate a session with login.minecraft.net for online use only
        /// </summary>
        /// <param name="username">The player's username</param>
        /// <param name="password">The player's password</param>
        /// <param name="clientVer">The client version (look here http://wiki.vg/Session)</param>
        /// <returns>Returns 5 values split by ":" Current Version : Download Ticket : Username : Session ID : UID</returns>
        public static string generateSession(string username, string password, int clientVer)
        {
            string netResponse = httpGET("https://login.minecraft.net?user=" + username + "&password=" + password + "&version=" + clientVer);
            return netResponse;
        }

        public static string httpGET(string URI)
        {
            WebRequest req = WebRequest.Create(URI);
            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        /// <summary>
        /// Used to start minecraft with certain arguments
        /// </summary>
        /// <param name="mode">True for online and false for offline</param>
        /// <param name="ramMin">Equivalent to the -Xms argument</param>
        /// <param name="ramMax">Equivalent to the -Xmx argument</param>
        /// <param name="username">The player's username</param>
        /// <param name="sessionID">The session id generated from login.minecraft.net</param>
        /// <param name="debug">True for console boot and false for no console</param>
        public static void startMinecraft(bool mode, int ramMin, int ramMax, string username, string sessionID, bool debug)
        {
            
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\";
            Process proc = new Process();
            if (debug == true)
            {
                proc.StartInfo.FileName = "java";
            }
            else
            {
                proc.StartInfo.FileName = "javaw";
            }

            //Online and offline modes
            if (mode == true)
            {
                proc.StartInfo.Arguments = "-Xms" + ramMin + "M -Xmx" + ramMax + "M -Djava.library.path=" + appData + ".xylotech/.minecraftbin/natives -cp " + appData + ".xylotech/.minecraftbin/minecraft.jar;" + appData + ".xylotech/.minecraftbin/jinput.jar;" + appData + ".xylotech/.minecraftbin/lwjgl.jar;" + appData + ".xylotech/.minecraftbin/lwjgl_util.jar net.minecraft.client.Minecraft " + username + " " + sessionID;
            }
            else
            {
                proc.StartInfo.Arguments = "-Xms" + ramMin + "M -Xmx" + ramMax + "M -Djava.library.path=" + appData + ".xylotech/.minecraftbin/natives -cp " + appData + ".xylotech/.minecraftbin/minecraft.jar;" + appData + ".xylotech/.minecraftbin/jinput.jar;" + appData + ".xylotech/.minecraftbin/lwjgl.jar;" + appData + ".xylotech/.minecraftbin/lwjgl_util.jar net.minecraft.client.Minecraft " + username;
            }
            proc.Start();
        }
    }
}
