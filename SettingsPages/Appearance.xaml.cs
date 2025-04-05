using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App3.SettingsPages
{
    public sealed partial class Appearance : Page
    {

        public Appearance()
        {
            this.InitializeComponent();

            Theme_Selection.SelectedIndex = (Application.Current as App).ThemeSelected;
            ShowCollection_Switch.IsOn = (Application.Current as App).ShowCollection;
            ShowHistory_Switch.IsOn = (Application.Current as App).ShowHistory;
            ShowDownload_Switch.IsOn = (Application.Current as App).ShowDownload;
            ShowFullScreen_Switch.IsOn = (Application.Current as App).ShowFullScreen;
            ShowTabList_Switch.IsOn = (Application.Current as App).ShowTabList;
        }

        private void Theme_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int ThemeSelected = Theme_Selection.SelectedIndex;
            if (ThemeSelected == 0)
            {
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Default;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (App.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    TitleBar.ButtonForegroundColor = Colors.Black;
                    TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.White;
                    TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                }
                else if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    TitleBar.ButtonForegroundColor = Colors.White;
                    TitleBar.ButtonHoverForegroundColor = Colors.White;
                    TitleBar.ButtonPressedForegroundColor = Colors.White;
                    TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                    TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                }
            }
            else if (ThemeSelected == 1)
            {
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                TitleBar.ButtonForegroundColor = Colors.Black;
                TitleBar.ButtonHoverForegroundColor = Colors.Black;
                TitleBar.ButtonPressedForegroundColor = Colors.Black;
                TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                TitleBar.ButtonHoverBackgroundColor = Colors.White;
                TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
            }
            else if (ThemeSelected == 2)
            {
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                TitleBar.ButtonForegroundColor = Colors.White;
                TitleBar.ButtonHoverForegroundColor = Colors.White;
                TitleBar.ButtonPressedForegroundColor = Colors.White;
                TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }
            (Application.Current as App).ThemeSelected = ThemeSelected;

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = new Windows.Storage.ApplicationDataCompositeValue();
            App_Theme["App_Theme"] = Theme_Selection.SelectedIndex;
            LocalSettings.Values["App_Theme"] = App_Theme;
        }

        private void Collection_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowCollection = ShowCollection_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ShowCollectionSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ShowCollectionSwitch["ShowCollectionSwitch"] = ShowCollection_Switch.IsOn;
            LocalSettings.Values["ShowCollectionSwitch"] = ShowCollectionSwitch;
        }
        private void History_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowHistory = ShowHistory_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ShowHistorySwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ShowHistorySwitch["ShowHistorySwitch"] = ShowHistory_Switch.IsOn;
            LocalSettings.Values["ShowHistorySwitch"] = ShowHistorySwitch;
        }
        private void Download_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowDownload = ShowDownload_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ShowDownloadSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ShowDownloadSwitch["ShowDownloadSwitch"] = ShowDownload_Switch.IsOn;
            LocalSettings.Values["ShowDownloadSwitch"] = ShowDownloadSwitch;
        }

        private void FullScreen_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowFullScreen = ShowFullScreen_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ShowFullScreenSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ShowFullScreenSwitch["ShowFullScreenSwitch"] = ShowFullScreen_Switch.IsOn;
            LocalSettings.Values["ShowFullScreenSwitch"] = ShowFullScreenSwitch;
        }

        private void ShowTabList_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowTabList = ShowTabList_Switch.IsOn;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue ShowTabListSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            ShowTabListSwitch["ShowTabListSwitch"] = ShowTabList_Switch.IsOn;
            LocalSettings.Values["ShowTabListSwitch"] = ShowTabListSwitch;
        }
    }
}
