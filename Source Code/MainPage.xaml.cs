using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer Timer;

        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public MainPage()
        {
            this.InitializeComponent();

            SetTitleBar();
        }

        public void SetTitleBar()
        {
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            TitleBar.ButtonBackgroundColor = Colors.Transparent;
            TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(AppTitleBar);
        }

        public TabViewItem SelectedTab
        {
            get
            {
                TabViewItem selectedItem = (TabViewItem)MicrosoftEdge.SelectedItem;
                return selectedItem;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            (Application.Current as App).WebViewHeight = (int)ActualHeight;

            if ((Application.Current as App).IsNewTab == true)
            {
                (Application.Current as App).IsNewTab = false;
                TabViewItem NewTabPage = CreateNewTab();
                MicrosoftEdge.TabItems.Add(NewTabPage);
                MicrosoftEdge.SelectedItem = NewTabPage;
            }

            if (SettingsFrame.Visibility == Visibility.Visible && SettingsFrame.Opacity == 0)
            {
                SettingsFrame.Visibility = Visibility.Collapsed;
            }
        }

        public void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            TabViewItem NewTabPage = CreateNewTab();
            MicrosoftEdge.TabItems.Add(NewTabPage);
            MicrosoftEdge.SelectedItem = NewTabPage;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        public void TabView_AddButtonClick(TabView sender, object args)
        {
            TabViewItem NewTabPage = CreateNewTab();
            MicrosoftEdge.TabItems.Add(NewTabPage);
            MicrosoftEdge.SelectedItem = NewTabPage;
        }

        public void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {

            if((Application.Current as App).WebViewSelected == 0)
            {
                (((Frame)args.Tab.Content).Content as WebPage2).CloseWebView();
            }
            else if((Application.Current as App).WebViewSelected == 1)
            {
                (((Frame)args.Tab.Content).Content as WebPage).CloseWebView();
            }
            sender.TabItems.Remove(args.Tab);

            if (sender.TabItems.Count == 0)
            {
                CoreApplication.Exit();
            }
        }

        public TabViewItem CreateNewTab()
        {
            TabViewItem newItem = new TabViewItem();
            Frame frame = new Frame();
            if((Application.Current as App).WebViewSelected == 0)
            {
                frame.Navigate(typeof(WebPage2));
            }
            else if((Application.Current as App).WebViewSelected == 1) 
            {
                frame.Navigate(typeof(WebPage));
            }
            newItem.Content = frame;
            //newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Stop };
            newItem.Header = "新标签页";
            return newItem;
        }

        public void Settings()
        {
            if(SettingsFrame.Visibility != Visibility.Visible)
            {
                SettingsFrame.Visibility = Visibility.Visible;
                SettingsFrame.Navigate(typeof(Settings), null, new SuppressNavigationTransitionInfo());
                SettingsBackControl.Visibility = Visibility.Visible;

                SettingsStoryBoardDoubleAnimation.From = 0.001;
                SettingsStoryBoardDoubleAnimation.To = 1;
                SettingsStoryBoard.Begin();
            }
        }

        private void SettingsBack_Click(object sender, RoutedEventArgs e)
        {
            SettingsBackControl.Visibility = Visibility.Collapsed;

            SettingsStoryBoardDoubleAnimation.From = 1;
            SettingsStoryBoardDoubleAnimation.To = 0;
            SettingsStoryBoard.Begin();

            Browser.SetTitleBar();
        }
    }
}
