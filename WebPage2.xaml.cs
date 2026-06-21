using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using App3;
using Microsoft.Web.WebView2.Core;
using static App3.App;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media.Animation;
using System.Linq.Expressions;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App3
{
    public sealed partial class WebPage2 : Page
    {
        DispatcherTimer Timer;
        bool IsDownloadPageOpening = false;
        int LayoutState = 0;
        bool PageReqFS = false;
        string StartupLink = "about:blank";

        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public string WebLink
        {
            get { return EdgeWebView.Source.ToString(); }
            set { StartupLink = value; }
        }

        public WebPage2()
        {
            this.InitializeComponent();

            Timer = new DispatcherTimer();
        }

        bool LinkTyping = false;
        bool WebNavigating = false;
        private void Timer_Tick(object sender, object e)
        {
            if(EdgeWebView.CoreWebView2 == null)
            {
                CoreWebView2_WindowCloseRequested(null, null);
            }

            Page_SizeChanged();
            isLoaded = true;

            if (EdgeWebView.VerticalAlignment == VerticalAlignment.Stretch)
            {
                EdgeWebView.Visibility = Visibility.Visible;
            }
            else if(EdgeWebView.VerticalAlignment == VerticalAlignment.Top)
            {
                //EdgeWebView.Visibility = Visibility.Collapsed;
            }

            if (WebNavigating == true && (EdgeWebView.Source.ToString() != "about:blank"))
            {
                if (LoadingBar.Visibility == Visibility.Collapsed)
                {
                    LoadingBar.IsIndeterminate = false;
                    LoadingBar.IsIndeterminate = true;
                    LoadingBar.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (LoadingBar.Visibility == Visibility.Visible)
                {
                    LoadingBar.Visibility = Visibility.Collapsed;
                }
            }

            if (LinkTyping == false)
            {
                string LastPage = LinkBox.Text;
                LinkBox.Text = EdgeWebView.Source.ToString();
                if (EdgeWebView.Source.ToString() == "about:blank")
                {
                    LinkBox.Text = "";
                }
            }

            if (EdgeWebView.Source.ToString() != "about:blank")
            {
                if (EdgeWebView.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    EdgeWebView.VerticalAlignment = VerticalAlignment.Stretch;
                }
            }
            else if (EdgeWebView.VerticalAlignment != VerticalAlignment.Top && WebNavigating == false)
            {
                EdgeWebView.VerticalAlignment = VerticalAlignment.Top;
                EdgeWebView.CoreWebView2.Reload();
            }

            if (EdgeWebView.VerticalAlignment == VerticalAlignment.Stretch)
            {
                SearchBox.Text = "";
            }

            if(Browser.PageTabStopSet == 0)
            {
                EdgeWebView.IsTabStop = SearchBox.IsTabStop = true;
                Button_Back.IsTabStop = Button_Forward.IsTabStop = Button_Refresh.IsTabStop = ButtonM_Back.IsTabStop = ButtonM_Forward.IsTabStop = ButtonM_Refresh.IsTabStop = ButtonM_NewTab.IsTabStop = ButtonM_TabList.IsTabStop = ButtonM_More.IsTabStop = CollectionButton.IsTabStop = HistoryButton.IsTabStop = DownloadButton.IsTabStop = MoreButton.IsTabStop = true;
                LinkBox.IsTabStop = true;
            }
            else if(Browser.PageTabStopSet == 1)
            {
                EdgeWebView.IsTabStop = SearchBox.IsTabStop = false;
                Button_Back.IsTabStop = Button_Forward.IsTabStop = Button_Refresh.IsTabStop = ButtonM_Back.IsTabStop = ButtonM_Forward.IsTabStop = ButtonM_Refresh.IsTabStop = ButtonM_NewTab.IsTabStop = ButtonM_TabList.IsTabStop = ButtonM_More.IsTabStop = CollectionButton.IsTabStop = HistoryButton.IsTabStop = DownloadButton.IsTabStop = MoreButton.IsTabStop = true;
                LinkBox.IsTabStop = true;
            }
            else if(Browser.PageTabStopSet == 2)
            {
                EdgeWebView.IsTabStop = SearchBox.IsTabStop = false;
                Button_Back.IsTabStop = Button_Forward.IsTabStop = Button_Refresh.IsTabStop = ButtonM_Back.IsTabStop = ButtonM_Forward.IsTabStop = ButtonM_Refresh.IsTabStop = ButtonM_NewTab.IsTabStop = ButtonM_TabList.IsTabStop = ButtonM_More.IsTabStop = CollectionButton.IsTabStop = HistoryButton.IsTabStop = DownloadButton.IsTabStop = MoreButton.IsTabStop = false;
                LinkBox.IsTabStop = false;
            }
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            WebNavigating = true;
            EdgeWebView.CoreWebView2.Settings.UserAgent = (Application.Current as App).BrowserUA;
        }
        private async void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            WebNavigating = false;
            PageReqFS = false;

            if (EdgeWebView.Source.ToString() != "about:blank" && EdgeWebView.Source.ToString() != "")
            {
                try
                { 
                    (Application.Current as App).HistoryList.Insert(0, new History_List { HistoryTitle = EdgeWebView.CoreWebView2.DocumentTitle, HistoryUri = EdgeWebView.Source.ToString() }); 
                }
                catch { }

                try
                {
                    string HistoryJson = JsonConvert.SerializeObject((Application.Current as App).HistoryList);
                    Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile HistoryFile = await StorageFolder.CreateFileAsync("LocalStorage2\\History.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                    await Windows.Storage.FileIO.WriteTextAsync(HistoryFile, HistoryJson);
                }
                catch { }
            }

            if (!isLoaded)
            {
                LinkBox.Text = "";
                if (StartupLink != "about:blank")
                {
                    string Link = StartupLink;
                    try
                    {
                        EdgeWebView.CoreWebView2.Navigate(Link);
                    }
                    catch
                    {
                        try
                        {
                            Link = "https://" + Link;
                            EdgeWebView.CoreWebView2.Navigate(Link);
                        }
                        catch
                        {
                            try
                            {
                                Link = (Application.Current as App).SearchToolLink + Link;
                                EdgeWebView.CoreWebView2.Navigate(Link);
                            }
                            catch
                            {

                            }
                        }
                    }
                    EdgeWebView.VerticalAlignment = VerticalAlignment.Stretch;
                }
                StartupLink = "about:blank";
            }

            if(!Timer.IsEnabled)
            {
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
        }

        private void LinkToChanging(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            LinkTyping = true;
            //(sender as TextBox).SelectAll();
        }
        private void LinkToChanged(object sender, RoutedEventArgs e)
        {
            if (LinkBox.Text == EdgeWebView.Source.ToString())
            {
                //LinkBox.Text = EdgeWebView.Source.ToString();
                LinkTyping = false;
            }
        }
        private void LinkIME(object sender, TextCompositionStartedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            LinkTyping = true;
        }

        private void LinkChanged(object sender, KeyRoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            LinkTyping = true;
            if ((EdgeWebView.VerticalAlignment != VerticalAlignment.Stretch || LinkBox.Text.ToString() != EdgeWebView.Source.ToString()) && LinkBox.Text != "" && e.Key == Windows.System.VirtualKey.Enter)
            {
                Browser.ShowSideWindow(0);
                Browser.ShowTabListWindow(0);

                LinkTyping = false;
                string Link = LinkBox.Text;
                if (Link[0] == '@')
                {
                    char[] CLink = Link.ToCharArray();
                    CLink[0] = ' ';
                    Link = new string(CLink);
                    Link = Link.TrimStart();
                    Link = (Application.Current as App).SearchToolLink + Link;
                    EdgeWebView.CoreWebView2.Navigate((Link));
                }
                else
                {
                    try
                    {
                        EdgeWebView.CoreWebView2.Navigate(Link);
                    }
                    catch
                    {
                        try
                        {
                            Link = "https://" + Link;
                            EdgeWebView.CoreWebView2.Navigate(Link);
                        }
                        catch
                        {
                            try
                            {
                                Link = (Application.Current as App).SearchToolLink + Link;
                                EdgeWebView.CoreWebView2.Navigate(Link);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                
            }
            else if((LinkBox.Text == "" && e.Key == Windows.System.VirtualKey.Enter) && EdgeWebView.VerticalAlignment == VerticalAlignment.Stretch && WebNavigating == false)
            {
                LinkBox.Text = EdgeWebView.Source.ToString();
            }
            else if(e.Key == Windows.System.VirtualKey.Escape)
            {
                LinkTyping = false;
                if(EdgeWebView.Source.ToString() != "about:blank")
                    LinkBox.Text = EdgeWebView.Source.ToString();
            }
        }

        public void Back(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            if (EdgeWebView.CanGoBack == true)
            {
                EdgeWebView.GoBack();
            }

            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);
        }

        public void Forward(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            if (EdgeWebView.CanGoForward == true)
            {
                EdgeWebView.GoForward();
            }

            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);
        }

        public void Refresh(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            if (isLoaded)
                EdgeWebView.CoreWebView2.Reload();

            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);
        }

        private void Collection(object sender = null, RoutedEventArgs e = null)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            Browser.ShowTabListWindow(0);
            Browser.ShowSideWindow(1);
        }

        private void History(object sender = null, RoutedEventArgs e = null)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            Browser.ShowSideWindow(2);
            Browser.ShowTabListWindow(0);
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);

            if (isLoaded)
            {
                IsDownloadPageOpening = EdgeWebView.CoreWebView2.IsDefaultDownloadDialogOpen;

                if (EdgeWebView.Visibility == Visibility.Collapsed && false)
                {
                    EdgeWebView.CoreWebView2.Navigate("edge://downloads");
                }
                else if (!IsDownloadPageOpening)
                {
                    EdgeWebView.CoreWebView2.OpenDefaultDownloadDialog();
                }
                else
                {
                    EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
                }

            }
        }

        private void FullScreen(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (!view.IsFullScreenMode)
            {
                view.TryEnterFullScreenMode();
            }
            else
            {
                view.ExitFullScreenMode();
            }
        }

        private void Settings(object sender = null, RoutedEventArgs e = null)
        {
            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);

            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();

            Browser.Settings();
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            Browser.AddTab(args.Uri.ToString());
            args.Handled = true;
        }

        private void SearchChanged(object sender, KeyRoutedEventArgs e)
        {
            if (SearchBox.Text != "" && e.Key == Windows.System.VirtualKey.Enter && EdgeWebView.VerticalAlignment != VerticalAlignment.Stretch)
            {
                string Link = (Application.Current as App).SearchToolLink + SearchBox.Text;
                LinkBox.Text = Link;
                EdgeWebView.CoreWebView2.Navigate(Link);
            }
        }

        public void CloseWebView()
        {
            Timer.Stop();
            EdgeWebView.Close();
        }

        private async void EdgeWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            try
            {
                sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                sender.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
                sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                sender.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
                sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
                sender.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;

                if(StartupLink == "about:blank")
                    LinkBox.Focus(FocusState.Keyboard);

                EdgeWebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = (Application.Current as App).AutoSavePassword;
                EdgeWebView.CoreWebView2.Settings.IsScriptEnabled = !(Application.Current as App).ForbidJavaScript;
                EdgeWebView.CoreWebView2.Settings.IsReputationCheckingRequired = true;
            }
            catch
            {
                CloseWebView();
                this.Frame.Navigate(typeof(WebPage),null, new SuppressNavigationTransitionInfo());
                (this.Frame.Content as WebPage).WebLink = StartupLink;
            }/*
            Timer.Stop();
            EdgeWebView.Close();

            this.Frame.Navigate(typeof(WebPage), null, new SuppressNavigationTransitionInfo());
            (this.Frame.Content as WebPage).WebLink = StartupLink;*/

        }

        private void CoreWebView2_PermissionRequested(CoreWebView2 sender, CoreWebView2PermissionRequestedEventArgs args)
        {/*
            Trace.WriteLine(args.PermissionKind.ToString());
            
            try
            {
                string PermissionKind = args.PermissionKind.ToString();
                if(PermissionKind == "Camera")
                    PermissionKind = "相机"
                ContentDialog dialog = new ContentDialog();
                dialog.Title = PermissionKind;
                dialog.PrimaryButtonText = "允许";
                dialog.SecondaryButtonText = "禁止";
                dialog.DefaultButton = ContentDialogButton.Secondary;
                dialog.Content = "\"" + EdgeWebView.CoreWebView2.DocumentTitle + "\" 正在访问 " + PermissionKind;
                dialog.FontFamily = new FontFamily("HarmonyOS Sans SC");

                var result = await dialog.ShowAsync();
                if (result.Equals(ContentDialogResult.Primary))
                {
                    args.State = CoreWebView2PermissionState.Allow;
                }
                else if (result.Equals(ContentDialogResult.Secondary))
                {
                    args.State = CoreWebView2PermissionState.Deny;
                }
            }
            catch { }*/
            args.State = CoreWebView2PermissionState.Allow;
            args.Handled = true;
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            for (int i = 0; i < ((Browser.Content as Grid).Children[0] as TabView).TabItems.Count; i++)
            {
                if ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i] as TabViewItem).Content == this.Frame)
                {
                    try
                    {
                        Uri faviconUri = new Uri(EdgeWebView.CoreWebView2.FaviconUri);
                        Microsoft.UI.Xaml.Controls.BitmapIconSource iconsource = new Microsoft.UI.Xaml.Controls.BitmapIconSource() { UriSource = faviconUri, ShowAsMonochrome = false };
                        ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i]) as TabViewItem).IconSource = iconsource;
                    }
                    catch
                    {
                        ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i]) as TabViewItem).IconSource = null;
                    }
                }
            }
            Browser.TabsChanged();
        }

        private void CoreWebView2_WindowCloseRequested(CoreWebView2 sender, object args)
        {
            for (int i = 0; i < ((Browser.Content as Grid).Children[0] as TabView).TabItems.Count; i++)
            {
                if ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i] as TabViewItem).Content == this.Frame)
                {
                    CloseWebView();
                    ((Browser.Content as Grid).Children[0] as TabView).TabItems.RemoveAt(i);
                    break;
                }
            }
        }

        private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            for (int i = 0; i < ((Browser.Content as Grid).Children[0] as TabView).TabItems.Count; i++)
            {
                if ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i] as TabViewItem).Content == this.Frame)
                {
                    if (EdgeWebView.Source.ToString() == "about:blank")
                    {
                        ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i]) as TabViewItem).Header = "新标签页";
                    }
                    else if (EdgeWebView.CoreWebView2.DocumentTitle == "")
                    {
                        ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i]) as TabViewItem).Header = "加载中";
                    }
                    else
                    {
                        ((((Browser.Content as Grid).Children[0] as TabView).TabItems[i]) as TabViewItem).Header = EdgeWebView.CoreWebView2.DocumentTitle;
                    }
                }
            }
        }

        bool isLoaded = false;
        private void EdgeWebView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MoreMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewTab_Click(object sender = null, RoutedEventArgs e = null)
        {
            Browser.AddTab();
            Browser.ShowSideWindow(0);
            Browser.ShowTabListWindow(0);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
        }

        private void Page_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            if((Application.Current as App).LayoutState == 0)
            {
                if (ActualWidth > 680)
                    LayoutState = 1;
                else
                    LayoutState = 2;
            }
            else
            {
                LayoutState = (Application.Current as App).LayoutState;
            }

            if ((App.Current.RequestedTheme == ApplicationTheme.Light && (Application.Current as App).ThemeSelected == 0) || (Application.Current as App).ThemeSelected == 1)
            {
                SeparateLineDark.StrokeThickness = 0;
                SeparateLineLight.StrokeThickness = 0.5;
            }
            else if ((App.Current.RequestedTheme == ApplicationTheme.Dark && (Application.Current as App).ThemeSelected == 0) || (Application.Current as App).ThemeSelected == 2)
            {
                SeparateLineDark.StrokeThickness = 0.5;
                SeparateLineLight.StrokeThickness = 0;
            }

            SeparateLineDark.X2 = 10 * ActualWidth;
            SeparateLineLight.X2 = 10 * ActualWidth;

            int i = 1;
            if ((Application.Current as App).ShowCollection)
            {
                CollectionButton.Visibility = Visibility.Visible;
                i++;
            }
            else
            {
                CollectionButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowHistory)
            {
                HistoryButton.Visibility = Visibility.Visible;
                i++;
            }
            else
            {
                HistoryButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowDownload)
            {
                DownloadButton.Visibility = Visibility.Visible;
                i++;
            }
            else
            {
                DownloadButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowFullScreen)
            {
                FullScreenButton.Visibility = Visibility.Visible;
                i++;
            }
            else
            {
                FullScreenButton.Visibility = Visibility.Collapsed;
            }


            if(LayoutState == 1)
            {
                EdgeTitleBar.Visibility = Visibility.Visible;
                EdgeBottomBar.Visibility = Visibility.Collapsed;
                EdgeWebView.Margin = new Thickness(0, 50, 0, 0);

                if (EdgeWebView.Source.ToString() == "about:blank")
                {
                    SearchBox.Margin = new Thickness(180, 180, 180, 0);
                }
                else
                {
                    SearchBox.Margin = new Thickness(180, -128, 180, 0);
                }

                LinkBox.Margin = new Thickness(140, 0, 40 * i + 20, 0);
                LinkBox.Height = 32;
                LinkBox.FontSize = 15;
                LoadingBar.VerticalAlignment = VerticalAlignment.Top;
                EdgeLinkGrid.VerticalAlignment = VerticalAlignment.Top;
                SeparateLineLight.Y1 = SeparateLineLight.Y2 = SeparateLineDark.Y1 = SeparateLineDark.Y2 = 50;
                
            }
            else if(LayoutState == 2)
            {
                EdgeTitleBar.Visibility = Visibility.Collapsed;
                EdgeBottomBar.Visibility = Visibility.Visible;
                EdgeWebView.Margin = new Thickness(0, 0, 0, 100);

                if (EdgeWebView.Source.ToString() == "about:blank")
                {
                    SearchBox.Margin = new Thickness(40, 120, 40, 0);
                }
                else
                {
                    SearchBox.Margin = new Thickness(180, -128, 180, 0);
                }

                LinkBox.Margin = new Thickness(12, 4, 56, 0);
                LinkBox.Height = 36;
                LinkBox.FontSize = 17;
                LoadingBar.VerticalAlignment = VerticalAlignment.Bottom;
                EdgeLinkGrid.VerticalAlignment = VerticalAlignment.Bottom;
                SeparateLineLight.Y1 = SeparateLineLight.Y2 = SeparateLineDark.Y1 = SeparateLineDark.Y2 = ActualHeight - 100;
            }
        }

        private void TabList_Click(object sender, RoutedEventArgs e)
        {
            Browser.ShowTabListWindow(1);
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(CoreWebView2 sender, object args)
        {
            PageReqFS = EdgeWebView.CoreWebView2.ContainsFullScreenElement;
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (!view.IsFullScreenMode && PageReqFS)
            {
                view.TryEnterFullScreenMode();
            }
            else if (view.IsFullScreenMode && !PageReqFS)
            {
                view.ExitFullScreenMode();
            }
        }

        private void MobilePage_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuFlyoutItem).Text == ResourceLoader.GetForCurrentView().GetString("C移动设备视图"))
            {
                (sender as MenuFlyoutItem).Text = ResourceLoader.GetForCurrentView().GetString("C桌面视图");
                if (isLoaded)
                {
                    EdgeWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Mobile/15E148 Safari/604.1";
                    EdgeWebView.CoreWebView2.Reload();
                } 
            }
            else
            {
                (sender as MenuFlyoutItem).Text = ResourceLoader.GetForCurrentView().GetString("C移动设备视图");
                if (isLoaded)
                {
                    EdgeWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36";
                    EdgeWebView.CoreWebView2.Reload();
                }
            }
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).CreateNewWindow();
        }

        private void NewWindowOpen_Click(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                (Application.Current as App).CreateNewWindow(WebLink);
                //CoreWebView2_WindowCloseRequested(null, null);
            }
        }

        public Windows.UI.Xaml.Media.ImageSource GetIcon_WindowsUiXamlControlsIconSource()
        {
            try
            {
                if(EdgeWebView.CoreWebView2 != null)
                {
                    Uri faviconUri = new Uri(EdgeWebView.CoreWebView2.FaviconUri);
                    Windows.UI.Xaml.Media.ImageSource iconsource = new BitmapImage() { UriSource = faviconUri };
                    return iconsource;
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
