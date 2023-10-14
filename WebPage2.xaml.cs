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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WebPage2 : Page
    {
        DispatcherTimer Timer;
        bool IsDownloadPageOpening = false;

        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public WebPage2()
        {
            this.InitializeComponent();

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
        }

        bool LinkTyping = false;
        bool WebNavigating = false;
        private void Timer_Tick(object sender, object e)
        {
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

            if (Browser.SelectedTab.Content == this.Frame)
            {
                (Application.Current as App).WebLink = EdgeWebView.Source.ToString();
                if (EdgeWebView.Source.ToString() == "about:blank")
                {
                    Browser.SelectedTab.Header = "新标签页";
                }
                else if (EdgeWebView.CoreWebView2.DocumentTitle == "")
                {
                    Browser.SelectedTab.Header = "加载中";
                }
                else
                {
                    Browser.SelectedTab.Header = EdgeWebView.CoreWebView2.DocumentTitle;
                }
            }

            if (WebNavigating == true && (EdgeWebView.Source.ToString() != "about:blank"))
            {
                if (ProgressBar.Margin == new Thickness(0, 50, 0, 0))
                {
                    ProgressBar.IsIndeterminate = false;
                    ProgressBar.IsIndeterminate = true;
                    ProgressBar.Margin = new Thickness(0, 46, 0, 0);
                }
            }
            else
            {
                if (ProgressBar.Margin == new Thickness(0, 46, 0, 0))
                {
                    ProgressBar.Margin = new Thickness(0, 50, 0, 0);
                }
            }

            if((Application.Current as App).ShowCollection)
            {
                CollectionButton.Visibility = Visibility.Visible;
            }
            else
            {
                CollectionButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowHistory)
            {
                HistoryButton.Visibility = Visibility.Visible;
            }
            else
            {
                HistoryButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowDownload)
            {
                DownloadButton.Visibility = Visibility.Visible;
            }
            else
            {
                DownloadButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowFullScreen)
            {
                FullScreenButton.Visibility = Visibility.Visible;
            }
            else
            {
                FullScreenButton.Visibility = Visibility.Collapsed;
            }

            if (LinkTyping == false)
            {
                string LastPage = LinkBox.Text;
                LinkBox.Text = EdgeWebView.Source.ToString();
                if (EdgeWebView.Source.ToString() == "about:blank")
                {
                    LinkBox.Text = "";
                    SearchBox.Margin = new Thickness(180, 180, 180, 0);
                }
                else
                {
                    SearchBox.Margin = new Thickness(180, -128, 180, 0);
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
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            WebNavigating = true;
        }
        private async void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            WebNavigating = false;

            if (EdgeWebView.Source.ToString() != "about:blank")
            {
                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                int HistoryCount = 1;
                try
                {
                    Windows.Storage.StorageFile file;
                    file = await StorageFolder.GetFileAsync("History\\HistoryCount");
                    var HCount = await Windows.Storage.FileIO.ReadLinesAsync(file);
                    HistoryCount += Int32.Parse(HCount[0]);
                }
                catch
                {

                }
                string HistoryTitle = EdgeWebView.CoreWebView2.DocumentTitle + "\n" + EdgeWebView.Source.ToString();
                string FileHistoryTitle = "History\\HistoryTitle" + HistoryCount.ToString();
                Windows.Storage.StorageFile Count = await StorageFolder.CreateFileAsync("History\\HistoryCount", Windows.Storage.CreationCollisionOption.OpenIfExists);
                Windows.Storage.StorageFile Title = await StorageFolder.CreateFileAsync(FileHistoryTitle, Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(Count, HistoryCount.ToString());
                await Windows.Storage.FileIO.WriteTextAsync(Title, HistoryTitle);

                (Application.Current as App).HistoryList.Clear();
                (Application.Current as App).GetHistory();
            }

            if (!isLoaded)
            {
                LinkBox.Text = "";
                if ((Application.Current as App).TabStartLink != "about:blank")
                {
                    EdgeWebView.CoreWebView2.Navigate((Application.Current as App).TabStartLink);
                    EdgeWebView.VerticalAlignment = VerticalAlignment.Stretch;
                }
                (Application.Current as App).TabStartLink = "about:blank";

                Timer = new DispatcherTimer();
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
        }

        private void LinkToChanging(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            LinkTyping = true;
        }
        private void LinkToChanged(object sender, RoutedEventArgs e)
        {
            LinkTyping = false;
        }
        private void LinkIME(object sender, TextCompositionStartedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            LinkTyping = true;
        }

        private void LinkChanged(object sender, KeyRoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            LinkTyping = true;
            if ((EdgeWebView.VerticalAlignment != VerticalAlignment.Stretch || LinkBox.Text.ToString() != EdgeWebView.Source.ToString()) && LinkBox.Text != "" && e.Key == Windows.System.VirtualKey.Enter)
            {
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
                    MatchCollection IsMatch;

                    int CanWebNav = 0;

                    IsMatch = Regex.Matches(Link, @" ");
                    foreach (Match m in IsMatch)
                    {
                        CanWebNav++;
                    }
                    if (CanWebNav > 0)
                    {
                        Link = (Application.Current as App).SearchToolLink + Link;
                        LinkBox.Text = Link;
                        EdgeWebView.CoreWebView2.Navigate((Link));
                    }
                    else
                    {
                        IsMatch = Regex.Matches(Link, @"^(about?)");
                        foreach (Match m in IsMatch)
                        {
                            CanWebNav++;
                        }
                        IsMatch = Regex.Matches(Link, @"^(https?)://");
                        foreach (Match m in IsMatch)
                        {
                            CanWebNav++;
                        }
                        IsMatch = Regex.Matches(Link, @"^(edge?)://");
                        foreach (Match m in IsMatch)
                        {
                            CanWebNav++;
                        }
                        if (CanWebNav > 0)
                        {
                            EdgeWebView.CoreWebView2.Navigate((Link));
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
                                Link = "https://" + LinkBox.Text;
                                LinkBox.Text = Link;
                                EdgeWebView.CoreWebView2.Navigate((Link));
                            }
                            else
                            {
                                Link = (Application.Current as App).SearchToolLink + Link;
                                LinkBox.Text = Link;
                                EdgeWebView.CoreWebView2.Navigate((Link));
                            }
                        }
                    }
                }
                
            }
            else if(LinkBox.Text == "" && e.Key == Windows.System.VirtualKey.Enter && EdgeWebView.VerticalAlignment == VerticalAlignment.Stretch && WebNavigating == false)
            {
                LinkBox.Text = EdgeWebView.Source.ToString();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            if (EdgeWebView.CanGoBack == true)
            {
                EdgeWebView.GoBack();
            }
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            if (EdgeWebView.CanGoForward == true)
            {
                EdgeWebView.GoForward();
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            EdgeWebView.CoreWebView2.Reload();
        }

        private void Collection(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            Collection_Flyout.Content = new Collection();
        }

        private void History(object sender, RoutedEventArgs e)
        {
            EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
            History_Flyout.Content = new History();
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            try
            {
                IsDownloadPageOpening = EdgeWebView.CoreWebView2.IsDefaultDownloadDialogOpen;
            }
            catch
            {

            }

            if (!IsDownloadPageOpening)
            {
                EdgeWebView.CoreWebView2.OpenDefaultDownloadDialog();
            }
            else
            {
                EdgeWebView.CoreWebView2.CloseDefaultDownloadDialog();
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

        private void Settings(object sender, RoutedEventArgs e)
        {
            IsDownloadPageOpening = false;

            Browser.Settings();
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            (Application.Current as App).TabStartLink = args.Uri.ToString();
            Browser.TabView_AddButtonClick(null, null);
            args.Handled = true;
        }

        private void SearchChanged(object sender, KeyRoutedEventArgs e)
        {
            if (SearchBox.Text != "" && e.Key == Windows.System.VirtualKey.Enter && EdgeWebView.VerticalAlignment != VerticalAlignment.Stretch)
            {
                string Link = (Application.Current as App).SearchToolLink + SearchBox.Text;
                LinkBox.Text = Link;
                EdgeWebView.CoreWebView2.Navigate((Link));
            }
        }

        public void CloseWebView()
        {
            EdgeWebView.Close();
        }

        private void EdgeWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        bool isLoaded = false;
        private void EdgeWebView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
