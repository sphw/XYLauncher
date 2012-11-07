using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<string[]> modpacks = new List<string[]>();
        LoginManager loginM = new LoginManager();
        BackgroundWorker worker = new BackgroundWorker();
        int Selected;
        public MainWindow()
        {
            string xml = new System.Net.WebClient().DownloadString("http://xylocraft.com/ModPack/modpacks.xml");
            InitializeComponent();
            worker.DoWork += BackgroundWorker_Work;
            worker.RunWorkerCompleted += BackgroundWorker_Complete;
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/"))
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/");
            Head.Source = loginM.GetHeadIcon();
            FileStream logindata = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/logindata", FileMode.OpenOrCreate);
            using (FileStream config = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/config", FileMode.OpenOrCreate)) { }
            using (StreamReader reader = new StreamReader(logindata))
            {
                Username.Text = reader.ReadLine();
                Password.Password = reader.ReadLine();
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    StringBuilder tempString = new StringBuilder();
                    if (reader.Name == "modpack")
                    {
                        string[] tempArray = new string[6];
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
                        modsList.Items.Add(tempText);
                        reader.MoveToAttribute("id");
                        tempArray[0] = reader.Value;
                        reader.MoveToAttribute("newsURL");
                        tempArray[1] = tempString.ToString();
                        tempArray[2] = reader.Value;
                        reader.MoveToAttribute("image");
                        tempArray[3] = reader.Value;
                        reader.MoveToAttribute("url");
                        tempArray[5] = reader.Value;
                        modpacks.Add(tempArray);

                    }
                }
            }
            modsList.SelectedIndex = 0;
        }
        private void changed_Modpack(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            foreach (TextBlock s in e.AddedItems)
            {
                foreach (string[] sa in modpacks)
                {
                    i++;
                    if (s.Text == sa[1])
                    {
                        Selected = i;
                        newsBrowser.Source = new Uri(sa[2]);
                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.UriSource = new Uri("http://xylocraft.com/ModPack/" + sa[3]);
                        logo.EndInit();
                        Splash.Source = logo;
                        Name.Text = sa[4];
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            loginM.SaveLoginData(Username.Text, Password.Password);
            DownloadManager downM = new DownloadManager();
            string modPackDown;
            using (StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.xylotech/config"))
            {
                modPackDown = reader.ReadLine();
            }
            if (modPackDown == null)
            {
                worker.RunWorkerAsync();
            }
            else if(modPackDown == "true") {
                loginM.LaunchMinecraft(Username.Text, Password.Password);
                Progress.Visibility = Visibility.Hidden;
                Application.Current.Shutdown();
            }
        }
        private void BackgroundWorker_Complete(object s, RunWorkerCompletedEventArgs e)
        {
            loginM.LaunchMinecraft(Username.Text, Password.Password);
            Progress.Visibility = Visibility.Hidden;
            Application.Current.Shutdown();
            
        }
        private void BackgroundWorker_Work(object sender, DoWorkEventArgs e)
        {
            DownloadManager downM = new DownloadManager();
            downM.DownloadModPack(modpacks[Selected][5]);
            downM.InstallModPack(modpacks[Selected][5]);
        }
        string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadLine();
            }
        }
    }
}
