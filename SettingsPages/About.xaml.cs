using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App3.SettingsPages
{
    public sealed partial class About : Page
    {
        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public About()
        {
            this.InitializeComponent();
            TextVersion.Text = string.Format("版本：{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
        }

        private void ProjectHome_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://github.com/MSEDGE-CORE/Fringe");
        }

        private void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://github.com/MSEDGE-CORE/Fringe/releases");
        }

        private void SendFeedBack_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://github.com/MSEDGE-CORE/Fringe/issues");
        }

        private void LinkWinUI_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://github.com/microsoft/microsoft-ui-xaml");
        }

        private void LinkGxapSpartan_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://github.com/ZJH365");
        }

        private void LinkHtmlPack_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://html-agility-pack.net/");
        }

        private void LinkJson_Click(object sender, RoutedEventArgs e)
        {
            Browser.SettingsBack_Click(sender, e);
            MainPage.Browser.AddTab("https://www.newtonsoft.com/json");
        }
    }
}
