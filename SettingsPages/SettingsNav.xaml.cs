using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

namespace App3.SettingsPages
{
    public sealed partial class SettingsNav : Page
    {
        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public SettingsNav()
        {
            this.InitializeComponent();
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            TitleBar.ButtonBackgroundColor = Colors.Transparent;
            TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);

            LeftBar.Visibility = Visibility.Visible;
            NavigationButtons.HorizontalAlignment = HorizontalAlignment.Left;
            NavigationButtons.VerticalAlignment = VerticalAlignment.Center;
            NavigationButtons.Orientation = Orientation.Vertical;
            NavigationButtons.Margin = new Thickness(2, 0, 0, 0);
            ContentFrame.Margin = new Thickness(68, 0, 0, 0);

            ContentFrame.Navigate(typeof(SettingsPages.Appearance));
            NavAppearanceButton.IsChecked = true;
            NavBrowseButton.IsChecked = false;
            NavFileButton.IsChecked = false;
            NavAboutButton.IsChecked = false;
        }

        private void NavBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(SettingsPages.Browse))
            {
                ContentFrame.Navigate(typeof(SettingsPages.Browse), null, new SuppressNavigationTransitionInfo());
            }
            NavAppearanceButton.IsChecked = false;
            NavBrowseButton.IsChecked = true;
            NavFileButton.IsChecked = false;
            NavAboutButton.IsChecked = false;
        }

        private void NavAboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(SettingsPages.About))
            {
                ContentFrame.Navigate(typeof(SettingsPages.About), null, new SuppressNavigationTransitionInfo());
            }
            NavAppearanceButton.IsChecked = false;
            NavBrowseButton.IsChecked = false;
            NavFileButton.IsChecked = false;
            NavAboutButton.IsChecked = true;
        }

        private void NavFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(SettingsPages.File))
            {
                ContentFrame.Navigate(typeof(SettingsPages.File), null, new SuppressNavigationTransitionInfo());
            }
            NavAppearanceButton.IsChecked = false;
            NavBrowseButton.IsChecked = false;
            NavFileButton.IsChecked = true;
            NavAboutButton.IsChecked = false;
        }

        private void NavAppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(SettingsPages.Appearance))
            {
                ContentFrame.Navigate(typeof(SettingsPages.Appearance), null, new SuppressNavigationTransitionInfo());
            }
            NavAppearanceButton.IsChecked = true;
            NavBrowseButton.IsChecked = false;
            NavFileButton.IsChecked = false;
            NavAboutButton.IsChecked = false;
        }
    }
}
