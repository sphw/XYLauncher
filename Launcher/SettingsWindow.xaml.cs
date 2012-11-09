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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Net;
namespace Launcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        Properties.Settings s = new Properties.Settings();
        public SettingsWindow()
        {
            InitializeComponent();
            ModPack.Text = s.ModPack;
            Update.IsChecked = s.LaunchUpdate;
            CloseLaunch.IsChecked = s.CloseLaunch;
            Password.IsChecked = s.Password;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Uri siteUri = new Uri(ModPack.Text);
            WebRequest wr = WebRequest.Create(siteUri);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        s.ModPack = ModPack.Text;
                    }
                    response.Close();
                }
            }
            catch { ErrorWindow error = new ErrorWindow(); error.Title = "Error"; error.Error.Content = "The URL could not be found."; error.Show(); }
            s.LaunchUpdate = Update.IsChecked.Value;
            s.Password = Password.IsChecked.Value;
            s.CloseLaunch = CloseLaunch.IsChecked.Value;
            s.Save();
            this.Close();
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Uri siteUri = new Uri(ModPack.Text);
            WebRequest wr = WebRequest.Create(siteUri);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        s.ModPack = ModPack.Text;
                    }
                    response.Close();
                }
            }
            catch { ErrorWindow error = new ErrorWindow(); error.Title = "Error"; error.Error.Content = "The URL could not be found."; error.Show(); }
            s.LaunchUpdate = Update.IsChecked.Value;
            s.Password = Password.IsChecked.Value;
            s.CloseLaunch = CloseLaunch.IsChecked.Value;
            s.Save();
        }
       
    }
}
