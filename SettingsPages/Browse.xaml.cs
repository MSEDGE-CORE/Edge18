using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App3.SettingsPages
{
    public sealed partial class Browse : Page
    {
        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public Browse()
        {
            this.InitializeComponent();


            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue WebView_Mode = (ApplicationDataCompositeValue)LocalSettings.Values["WebView_Mode"];
            if (WebView_Mode != null)
            {
                WebView_Selection.SelectedIndex = (int)WebView_Mode["WebView_Mode"];
            }
            else
            {
                WebView_Selection.SelectedIndex = 0;
            }

            Restart_Button.Visibility = Visibility.Collapsed;

            if ((Application.Current as App).SearchToolLink == "https://cn.bing.com/search?q=")
            {
                Search_Selection.SelectedIndex = 0;
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }
            else if ((Application.Current as App).SearchToolLink == "https://www.baidu.com/s?wd=")
            {
                Search_Selection.SelectedIndex = 1;
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }
            else if ((Application.Current as App).SearchToolLink == "https://www.google.com/search?q=")
            {
                Search_Selection.SelectedIndex = 2;
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }
            else if ((Application.Current as App).SearchToolLink == "https://www.sogou.com/web?query=")
            {
                Search_Selection.SelectedIndex = 3;
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }
            else if ((Application.Current as App).SearchToolLink == "https://www.so.com/s?q=")
            {
                Search_Selection.SelectedIndex = 4;
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                Search_Selection.SelectedIndex = 5;
                SetSearch_Button.Visibility = Visibility.Visible;
            }

            if ((Application.Current as App).HomePageLink == "about:blank")
            {
                Home_Selection.SelectedIndex = 0;
                SetHome_Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                Home_Selection.SelectedIndex = 1;
                SetHome_Button.Visibility = Visibility.Collapsed;
            }

            ForbidJavaScript_Switch.IsOn = (Application.Current as App).ForbidJavaScript;
            AutoSavePassword_Switch.IsOn = (Application.Current as App).AutoSavePassword;
        }

        private void SetSearch_Click(object sender, RoutedEventArgs e)
        {
            Search_Link.Text = (Application.Current as App).SearchToolLink;
        }

        private void SetSearch_Complete(object sender, RoutedEventArgs e)
        {
            string Link = Search_Link.Text;

            MatchCollection IsMatch = Regex.Matches(Link, @"^(https?)://");

            int CanWebNav = 0;
            foreach (Match m in IsMatch)
            {
                CanWebNav++;
            }

            if (CanWebNav > 0)
            {
                Set_SearchLink_Flyout.Hide();
                (Application.Current as App).SearchToolLink = Link;
            }
            else
            {
                IsMatch = Regex.Matches(Link, @"\.");
                foreach (Match m in IsMatch)
                {
                    CanWebNav++;
                }
                if (CanWebNav > 0)
                {
                    Link = "https://" + Search_Link.Text;

                    Set_SearchLink_Flyout.Hide();
                    (Application.Current as App).SearchToolLink = Link;
                }
                else
                {

                }
            }
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue SearchLink = new Windows.Storage.ApplicationDataCompositeValue();
            SearchLink["SearchLink"] = (Application.Current as App).SearchToolLink;
            LocalSettings.Values["SearchLink"] = SearchLink;
        }

        int SearchTool = 0;
        private void Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTool = Search_Selection.SelectedIndex;
            if (Search_Selection.SelectedIndex == 5)
            {
                SetSearch_Button.Visibility = Visibility.Visible;
            }
            else
            {
                SetSearch_Button.Visibility = Visibility.Collapsed;
            }

            if (Search_Selection.SelectedIndex == 0)
            {
                (Application.Current as App).SearchToolLink = "https://cn.bing.com/search?q=";
            }
            else if (Search_Selection.SelectedIndex == 1)
            {
                (Application.Current as App).SearchToolLink = "https://www.baidu.com/s?wd=";
            }
            else if (Search_Selection.SelectedIndex == 2)
            {
                (Application.Current as App).SearchToolLink = "https://www.google.com/search?q=";
            }
            else if (Search_Selection.SelectedIndex == 3)
            {
                (Application.Current as App).SearchToolLink = "https://www.sogou.com/web?query=";
            }
            else if (Search_Selection.SelectedIndex == 4)
            {
                (Application.Current as App).SearchToolLink = "https://www.so.com/s?q=";
            }
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue SearchLink = new Windows.Storage.ApplicationDataCompositeValue();
            SearchLink["SearchLink"] = (Application.Current as App).SearchToolLink;
            LocalSettings.Values["SearchLink"] = SearchLink;
        }


        private void Restart(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).WebViewSelected = WebView_Selection.SelectedIndex;
            (sender as Button).Visibility = Visibility.Collapsed;
            Browser.SettingsBack_Click(null, null);
            //await CoreApplication.RequestRestartAsync(string.Empty);
        }

        private void WebView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int WebViewSelected = WebView_Selection.SelectedIndex;
            //(Application.Current as App).WebViewSelected = WebViewSelected;

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue WebView_Mode = new Windows.Storage.ApplicationDataCompositeValue();
            WebView_Mode["WebView_Mode"] = WebView_Selection.SelectedIndex;
            LocalSettings.Values["WebView_Mode"] = WebView_Mode;

            Restart_Button.Visibility = Visibility.Visible;
        }

        private void AutoSavePassword_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).AutoSavePassword = AutoSavePassword_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue AutoSavePasswordSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            AutoSavePasswordSwitch["AutoSavePasswordSwitch"] = AutoSavePassword_Switch.IsOn;
            LocalSettings.Values["AutoSavePasswordSwitch"] = AutoSavePasswordSwitch;
        }

        private void ForbidJavaScript_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ForbidJavaScript = ForbidJavaScript_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ForbidJavaScriptSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ForbidJavaScriptSwitch["ForbidJavaScriptSwitch"] = ForbidJavaScript_Switch.IsOn;
            LocalSettings.Values["ForbidJavaScriptSwitch"] = ForbidJavaScriptSwitch;
        }

        private void Home_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home_Selection.SelectedIndex == 1)
            {
                SetHome_Button.Visibility = Visibility.Visible;
            }
            else
            {
                SetHome_Button.Visibility = Visibility.Collapsed;
            }

            if (Home_Selection.SelectedIndex == 0)
            {
                (Application.Current as App).HomePageLink = "about:blank";
            }
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue HomeLink = new Windows.Storage.ApplicationDataCompositeValue();
            HomeLink["HomePageLink"] = (Application.Current as App).HomePageLink;
            LocalSettings.Values["HomePageLink"] = HomeLink;
        }

        private void SetHome_Button_Click(object sender, RoutedEventArgs e)
        {
            Home_Link.Text = (Application.Current as App).HomePageLink;
        }

        private void SetHome_Complete(object sender, RoutedEventArgs e)
        {
            string Link = Home_Link.Text;

            MatchCollection IsMatch = Regex.Matches(Link, @"^(https?)://");

            int CanWebNav = 0;
            foreach (Match m in IsMatch)
            {
                CanWebNav++;
            }

            if (CanWebNav > 0)
            {
                Set_HomeLink_Flyout.Hide();
                (Application.Current as App).HomePageLink = Link;
            }
            else
            {
                IsMatch = Regex.Matches(Link, @"\.");
                foreach (Match m in IsMatch)
                {
                    CanWebNav++;
                }
                if (CanWebNav > 0)
                {
                    Link = "https://" + Home_Link.Text;

                    Set_HomeLink_Flyout.Hide();
                    (Application.Current as App).HomePageLink = Link;
                }
                else
                {

                }
            }
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue HomeLink = new Windows.Storage.ApplicationDataCompositeValue();
            HomeLink["HomePageLink"] = (Application.Current as App).HomePageLink;
            LocalSettings.Values["HomePageLink"] = HomeLink;
        }
    }
}
