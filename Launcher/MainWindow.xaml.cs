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
using AppLimit.NetSparkle;
namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<string[]> modpacks = new List<string[]>();
        LoginManager loginM = new LoginManager();
        BackgroundWorker downWorker = new BackgroundWorker();
        BackgroundWorker updateWorker = new BackgroundWorker();
        Properties.Settings settings = new Properties.Settings();
        ModPackManager modM = new ModPackManager();
        int Selected = 0;
        public MainWindow()
        {
            InitializeComponent();
            /// XMl Parse - Start 
            string xml = new System.Net.WebClient().DownloadString(settings.ModPack);
            List<TextBlock> tempText = new List<TextBlock>();
            List<string[]> tempArray = new List<string[]>();
            modM.ParseModPackFile(xml, out tempArray, out tempText);
            modpacks.AddRange(tempArray);
            foreach (TextBlock t in tempText) {
               modsList.Items.Add(t);
            }
            /// XML Parse - End
            
            /// Setup Background Workers - Start
            downWorker.DoWork += DownloadWorker_Work;
            downWorker.RunWorkerCompleted += DownloadWorker_Complete;
            updateWorker.DoWork += UpdateWorker_Work;
            updateWorker.RunWorkerCompleted += UpdateWorker_Complete;
            // Setup Background Workers - End

            /// Create Folders - Start
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + modpacks[Selected][6] + "/"))
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + modpacks[Selected][6] + "/");
            /// Create Folders - End

            /// Fill Content - Start
            Head.Source = loginM.GetHeadIcon(modpacks[Selected][6]);
            FileStream logindata = modM.CreateConfigs(modpacks[Selected][6]);
            using (StreamReader reader = new StreamReader(logindata))
            {
                Username.Text = reader.ReadLine();
                Password.Password = reader.ReadLine();
            }
            modsList.SelectedIndex = 0;
            /// Fill Content - End
        }
        /// <summary>
        /// When someone clicks on a diffrent modpack it sets all of the varibles correctly. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        Selected = i-1;
                        newsBrowser.Source = new Uri(sa[2]);
                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.UriSource = new Uri("http://xylocraft.com/ModPack/" + sa[3]);
                        logo.EndInit();
                        Splash.Source = logo;
                        Name.Text = sa[4];
                        if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + modpacks[Selected][6] + "/"))
                            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + modpacks[Selected][6] + "/");
                        modM.CreateConfigs(modpacks[Selected][6]);
                    }
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (loginM.CheckLogin(Username.Text, Password.Password) == true)
            {
                Progress.Visibility = Visibility.Visible;
                if (settings.Password == true)
                {
                    loginM.SaveLoginData(Username.Text, Password.Password,modpacks[Selected][6]);
                }
                
                string modPackDown;
                using (StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + modpacks[Selected][6] + "/config"))
                {
                    modPackDown = reader.ReadLine();
                }
                if (modPackDown == null)
                {
                    downWorker.RunWorkerAsync();
                }
                else if (modPackDown == "true")
                {

                    loginM.LaunchMinecraft(Username.Text, Password.Password, modpacks[Selected][6]);
                    Progress.Visibility = Visibility.Hidden;
                    if (settings.CloseLaunch == true)
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }
        private void DownloadWorker_Complete(object s, RunWorkerCompletedEventArgs e)
        {
            loginM.LaunchMinecraft(Username.Text, Password.Password, modpacks[Selected][6]);
            Progress.Visibility = Visibility.Hidden;
            if (settings.CloseLaunch == true)
            {
                Application.Current.Shutdown();
            }
        }
        private void DownloadWorker_Work(object sender, DoWorkEventArgs e)
        {
            ModPackManager modM = new ModPackManager();
            modM.DownloadModPack(modpacks[Selected][5], modpacks[Selected][6]);
            modM.InstallModPack(modpacks[Selected][5], modpacks[Selected][6]);
        }
        private void UpdateWorker_Work(object sender, DoWorkEventArgs e) {
            ModPackManager modM = new ModPackManager();
            modM.DeleteCurrentPack(modpacks[Selected][6]);
            modM.DownloadModPack(modpacks[Selected][5], modpacks[Selected][6]);
            modM.InstallModPack(modpacks[Selected][5], modpacks[Selected][6]);
        }
        private void UpdateWorker_Complete(object sender, RunWorkerCompletedEventArgs e) {
            Progress.Visibility = Visibility.Hidden;
            if (loginM.CheckLogin(Username.Text, Password.Password))
            {
                if (settings.LaunchUpdate == true)
                {
                    loginM.LaunchMinecraft(Username.Text, Password.Password, modpacks[Selected][6]);
                    if (settings.CloseLaunch == true)
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        } 
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            updateWorker.RunWorkerAsync();

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sW = new SettingsWindow();
            sW.Show();
        }

    }
}
