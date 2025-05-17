using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace App3
{
    public sealed partial class MainPage : Page
    {
        DispatcherTimer Timer;
        public bool IsSideWindowOpen = false;
        public bool IsTabListWindowOpen = false;
        int LayoutState = 0;
        public int PageTabStopSet = 0; //1:Webview 2:All

        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public MainPage()
        {
            this.InitializeComponent();

            SetTitleBar();
            ApplicationView.GetForCurrentView().Consolidated += MainPage_Consolidated;
        }

        private void MainPage_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            for (int i = 0; i < MicrosoftEdge.TabItems.Count; i++)
            {
                if (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage2))
                {
                    (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content as WebPage2).CloseWebView();
                }
                else if (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage))
                {
                    (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content as WebPage).CloseWebView();
                }
            }
            Timer.Stop();
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

        private async void Timer_Tick(object sender, object e)
        {
            Page_SizeChanged(null,null);

            if (MicrosoftEdge.TabItems.Count == 0)
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                //CoreApplication.Exit();
            }

            if (SettingsFrame.Visibility == Visibility.Visible && SettingsFrame.Opacity == 0)
            {
                SettingsFrame.Visibility = Visibility.Collapsed;
            }

            SideGrid.Visibility = (SideGridTransform.X < SideWindow.Width && !IsSideWindowOpen || IsSideWindowOpen) ? Visibility.Visible : Visibility.Collapsed;
            if(SideGrid.Visibility == Visibility.Collapsed && SideWindow.Content != null)
            {
                if (SideWindow.Content.GetType() == typeof(Collection))
                    ((SideWindow.Content) as Collection).OnClosing();
                else if (SideWindow.Content.GetType() == typeof(History))
                    ((SideWindow.Content) as History).OnClosing();
                SideWindow.Content = null;
            }
            TabListGrid.Visibility = (TabListGridTransform.X < TabListWindow.Width && !IsTabListWindowOpen || IsTabListWindowOpen) ? Visibility.Visible : Visibility.Collapsed;
        
            for(int i = 0; i < ListView.Items.Count; i++)
            {
                if (ListView.Items.Count != MicrosoftEdge.TabItems.Count)
                {
                    TabsChanged();
                    break;
                }
                if (TabList[i].Title != (MicrosoftEdge.TabItems[i] as TabViewItem).Header.ToString())
                {
                    TabsChanged();
                    break;
                }
            }
            
        }

        public void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            if(MicrosoftEdge.TabItems.Count == 0)
                AddTab();

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            Timer.Start();

            ListView.ItemsSource = TabList;
            TabsChanged();
        }

        public void AddTab(string Link = "")
        {
            if (Link == "" && (Application.Current as App).HomePageLink != "about:blank")
                Link = (Application.Current as App).HomePageLink;

            TabViewItem NewTabPage = new TabViewItem();
            Frame frame = new Frame();
            if ((Application.Current as App).WebViewSelected == 0 || Link.StartsWith("file://"))
            {
                frame.Navigate(typeof(WebPage2));
            }
            else if ((Application.Current as App).WebViewSelected == 1)
            {
                frame.Navigate(typeof(WebPage));
            }
            NewTabPage.Content = frame;
            NewTabPage.Header = "新标签页";
            //newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Stop };

            MicrosoftEdge.TabItems.Add(NewTabPage);
            MicrosoftEdge.SelectedItem = NewTabPage;

            if(Link != "")
            {
                if((Browser.SelectedTab.Content as Frame).Content.GetType() == typeof(WebPage2))
                    ((Browser.SelectedTab.Content as Frame).Content as WebPage2).WebLink = Link;
                else if((Browser.SelectedTab.Content as Frame).Content.GetType() == typeof(WebPage))
                    ((Browser.SelectedTab.Content as Frame).Content as WebPage).WebLink = Link;
            }

            ShowSideWindow(0);
            ShowTabListWindow(0);
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            AddTab();
        }

        public void TabView_TabCloseRequested(TabView sender = null, TabViewTabCloseRequestedEventArgs args = null)
        {
            if(((((Frame)args.Tab.Content).Content.GetType() == typeof(WebPage2))))
            {
                (((Frame)args.Tab.Content).Content as WebPage2).CloseWebView();
            }
            else if (((((Frame)args.Tab.Content).Content.GetType() == typeof(WebPage))))
            {
                (((Frame)args.Tab.Content).Content as WebPage).CloseWebView();
            }
            sender.TabItems.Remove(args.Tab);

            ShowSideWindow(0);
            ShowTabListWindow(0);
        }

        public void Settings()
        {
            if(SettingsFrame.Visibility != Visibility.Visible)
            {
                SettingsFrame.Visibility = Visibility.Visible;
                SettingsBackControl.Visibility = Visibility.Visible;

                SettingsStoryBoardDoubleAnimation.From = 0.001;
                SettingsStoryBoardDoubleAnimation.To = 1;
                SettingsStoryBoardDoubleAnimation.Duration = TimeSpan.FromMilliseconds(100);
                SettingsStoryBoard.Begin();
                SettingsFrame.Navigate(typeof(SettingsPages.Blank), null, new SuppressNavigationTransitionInfo());
                SettingsFrame.Navigate(typeof(SettingsPages.SettingsNav), null, new DrillInNavigationTransitionInfo());
            }

            ShowSideWindow(0);
            ShowTabListWindow(0);

            MicrosoftEdge.IsTabStop = false;
            TabListButton.IsTabStop = false;
            PageTabStopSet = 2;
        }

        public void SettingsBack_Click(object sender, RoutedEventArgs e)
        {
            if(SettingsFrame.CanGoBack)
            {
                SettingsFrame.GoBack();
            }

            SettingsBackControl.Visibility = Visibility.Collapsed;

            SettingsStoryBoardDoubleAnimation.From = 1;
            SettingsStoryBoardDoubleAnimation.To = 0;
            SettingsStoryBoardDoubleAnimation.Duration = TimeSpan.FromMilliseconds(300);
            SettingsStoryBoard.Begin();

            Browser.SetTitleBar();
            ShowSideWindow(0);
            ShowTabListWindow(0);

            MicrosoftEdge.IsTabStop = true;
            TabListButton.IsTabStop = true;
            PageTabStopSet = 0;
        }

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            MainPage.Browser.TabView_AddButtonClick(null, null);
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);
            if ((((InvokedTabView.SelectedItem as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage2)))
            {
                (((InvokedTabView.SelectedItem as TabViewItem).Content as Frame).Content as WebPage2).CloseWebView();
            }
            else if ((((InvokedTabView.SelectedItem as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage)))
            {
                (((InvokedTabView.SelectedItem as TabViewItem).Content as Frame).Content as WebPage).CloseWebView();
            }

            if (((TabViewItem)InvokedTabView.SelectedItem).IsClosable)
            {
                InvokedTabView.TabItems.Remove(InvokedTabView.SelectedItem);
            }

            ShowSideWindow(0);
            ShowTabListWindow(0);
        }

        public ObservableCollection<Tab_List> TabList { get; } = new ObservableCollection<Tab_List>();
        public void TabsChanged()
        {
            TabList.Clear();
            for (int i = 0; i < MicrosoftEdge.TabItems.Count; i++)
            {
                Windows.UI.Xaml.Media.ImageSource iconSource = null;
                if (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage2))
                    iconSource = (((MicrosoftEdge.TabItems[i] as TabViewItem).Content as Frame).Content as WebPage2).GetIcon_WindowsUiXamlControlsIconSource();
                TabList.Add(new Tab_List()
                {
                    Title = (MicrosoftEdge.TabItems[i] as TabViewItem).Header.ToString(),
                    iconSource = iconSource
                });
            }
            ListView.SelectedIndex = MicrosoftEdge.SelectedIndex;
        }

        public void SideWindowBackground_Click(object sender = null, RoutedEventArgs e = null)
        {
            ShowSideWindow(0);
            ShowTabListWindow(0);
        }

        public void ShowSideWindow(int ToOpen = 0)
        {
            
            if (SideWindow.Content != null && ((ToOpen == 1 && IsSideWindowOpen && SideWindow.Content.GetType() == typeof(Collection)) || (ToOpen == 2 && IsSideWindowOpen && SideWindow.Content.GetType() == typeof(History))))
            {
                (SideFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
                ToOpen = 0;
            }
            else if (SideWindow.Content != null && ((ToOpen == 2 && IsSideWindowOpen && SideWindow.Content.GetType() == typeof(Collection)) || (ToOpen == 1 && IsSideWindowOpen && SideWindow.Content.GetType() == typeof(History))))
            {
                /*SideGridTransform.X = SideWindow.Width;
                SideGrid.Opacity = 1;*/

                if (SideWindow.Content.GetType() == typeof(Collection))
                    ((SideWindow.Content) as Collection).OnClosing();
                else if (SideWindow.Content.GetType() == typeof(History))
                    ((SideWindow.Content) as History).OnClosing();
            }

            if (ToOpen == 1)
            {
                (SideFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
                SideWindow.Navigate(typeof(Collection), null, new SuppressNavigationTransitionInfo());
            }
            else if (ToOpen == 2)
            {
                (SideFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
                SideWindow.Navigate(typeof(History), null, new SuppressNavigationTransitionInfo());
            }

            IsSideWindowOpen = ToOpen != 0 ? true : false;
            SideFlowIn.From = ToOpen != 0 ? SideGridTransform.X : SideGridTransform.X;
            if (SideWindow.Width > 0)
                SideFlowIn.To = ToOpen != 0 ? 0 : SideWindow.Width;
            SideOpacity.From = ToOpen != 0 ? SideGrid.Opacity : SideGrid.Opacity;
            SideOpacity.To = ToOpen != 0 ? 1 : 1;
            SideStoryBoard.Begin();
            SideWindowBackground.Opacity = 0;
            SideWindowBackground.Visibility = ToOpen != 0 ? Visibility.Visible : Visibility.Collapsed;
            SideWindowBackground.Opacity = 0;
            SideGrid.Visibility = Visibility.Visible;
            SideSeparateBar.Visibility = ToOpen != 0 ? Visibility.Visible : Visibility.Collapsed;

            PageTabStopSet = (IsTabListWindowOpen ? 2 : (IsSideWindowOpen ? 1 : 0));
        }

        private void MicrosoftEdge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SideWindow.Width > 0)
            {
                ShowSideWindow(0);
                ShowTabListWindow(0);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool fromPage = sender == null ? false:true;

            if ((Application.Current as App).LayoutState == 0)
            {
                if (ActualWidth > 680)
                {
                    LayoutState = 1;
                }
                else if(ActualWidth <= 680)
                {
                    LayoutState = 2;
                }
            }
            else
            {
                LayoutState = (Application.Current as App).LayoutState;
            }

            if(LayoutState == 1)
            {
                AppTitleBar.Margin = new Thickness(0, 0, 200, 0);
                SideWindow.Width = 360;
                SideGrid.Margin = new Thickness(0, 90, 0, 0);
                SideWindowBackground.Margin = new Thickness(0, 90, 0, 0);
                TabListWindow.Width = 240;
                TabListGrid.Margin = new Thickness(0, 40, 0, 0);
                TabListBackground.Margin = new Thickness(0, 40, 0, 0);

                if ((Application.Current as App).ShowTabList)
                {
                    TabListButton.Visibility = Visibility.Visible;
                }
                else
                {
                    TabListButton.Visibility = Visibility.Collapsed;
                }
                ((TabListWindow.Content as Grid).Children[0] as ListView).VerticalAlignment = VerticalAlignment.Top;
                //(Application.Current as App).WebViewUA = 0;
            }
            else if(LayoutState == 2)
            {
                SideWindow.Width = ActualWidth;
                SideGrid.Margin = new Thickness(0, 40, 0, 101);
                SideWindowBackground.Margin = new Thickness(0, 40, 0, 100);
                TabListButton.Visibility = Visibility.Collapsed;
                TabListWindow.Width = ActualWidth;
                TabListGrid.Margin = new Thickness(0, 40, 0, 101);
                TabListBackground.Margin = new Thickness(0, 40, 0, 100);
                ((TabListWindow.Content as Grid).Children[0] as ListView).VerticalAlignment = VerticalAlignment.Bottom;
                //(Application.Current as App).WebViewUA = 1;
            }

            if(!IsSideWindowOpen && fromPage)
            {
                SideGridTransform.X = SideWindow.Width;
                ShowSideWindow(0);
            }
            if(!IsTabListWindowOpen && fromPage)
            {
                TabListGridTransform.X = -TabListWindow.Width;
                ShowTabListWindow(0);
            }

            if (ApplicationView.GetForCurrentView().IsFullScreenMode && fromPage && LayoutState == 1)
            {
                TitleDTransform.From = TitleBarTransform.Y;
                TitleDTransform.To = -92;
                TitleStoryBoard.Begin();
                MicrosoftEdge.Margin = new Thickness(0, 0, 0, -92);

                ExitFS.Visibility = Visibility.Visible;
                ExitFS.Height = 16;
                ExitFS.VerticalAlignment = VerticalAlignment.Top;
                ExitFS.Margin = new Thickness(0, 0, 0, 0);
                ShowSideWindow(0);
                ShowTabListWindow(0);
            }
            else if (fromPage)
            {
                TitleDTransform.From = TitleBarTransform.Y;
                TitleDTransform.To = 0;
                TitleStoryBoard.Begin();
                MicrosoftEdge.Margin = new Thickness(0, 0, 0, 0);

                ExitFS.Visibility = Visibility.Collapsed;
            }
        }

        private void TabListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTabListWindow(1);
        }

        public void ShowTabListWindow(int ToOpen = 0)
        {
            if (IsTabListWindowOpen && ToOpen == 1)
            {
                ToOpen = 0;
                (TabListFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
            }
            if (ToOpen == 1)
            { 
                if(MicrosoftEdge.SelectedIndex >= 0 && ListView.Items.Count > MicrosoftEdge.SelectedIndex)
                    ListView.SelectedIndex = MicrosoftEdge.SelectedIndex;
                (TabListFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
            }
            IsTabListWindowOpen = ToOpen != 0 ? true : false;
            TabListFlowIn.From = ToOpen != 0 ? TabListGridTransform.X : TabListGridTransform.X;
            if (TabListWindow.Width > 0)
                TabListFlowIn.To = ToOpen != 0 ? 0 : -TabListWindow.Width;
            TabListOpacity.From = ToOpen != 0 ? TabListGrid.Opacity : TabListGrid.Opacity;
            TabListOpacity.To = ToOpen != 0 ? 1 : 1;
            TabListStoryBoard.Begin();
            TabListBackground.Opacity = 0;
            TabListBackground.Visibility = ToOpen != 0 ? Visibility.Visible : Visibility.Collapsed;
            TabListBackground.Opacity = 0;
            TabListGrid.Visibility = Visibility.Visible;
            TabListSeparateBar.Visibility = ToOpen != 0 ? Visibility.Visible : Visibility.Collapsed;

            PageTabStopSet = (IsTabListWindowOpen ? 2 : (IsSideWindowOpen ? 1 : 0));
        }

        private void TabListBackground_Click(object sender, RoutedEventArgs e)
        {
            ShowTabListWindow(0);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 

        }
        private void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            (sender as ListView).SelectedIndex = ListView.Items.IndexOf(e.ClickedItem);

            if (ListView.SelectedIndex >= 0 && ListView.SelectedIndex != MicrosoftEdge.SelectedIndex)
            {
                MicrosoftEdge.SelectedIndex = ListView.SelectedIndex;
                ShowTabListWindow(0);
                //ListView.SelectedIndex = -1;
            }
        }

        private void EscKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ShowTabListWindow(0);
        }

        double SWindowX = 0;

        private void SideGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (IsSideWindowOpen)
            {
                SWindowX = SWindowX + e.Delta.Translation.X;
                if (SWindowX > 0 && e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse && (this.SideWindow.Content.GetType() != typeof(Collection) || (this.SideWindow.Content.GetType() == typeof(Collection) && !(SideWindow.Content as Collection).isEditing)))
                {
                    SideGridTransform.X += e.Delta.Translation.X;
                }
                else
                {
                    SideGridTransform.X = 0;
                }
            }
        }

        private void SideGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Velocities.Linear.X >= 0.5 && e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse && (this.SideWindow.Content.GetType() != typeof(Collection) || (this.SideWindow.Content.GetType() == typeof(Collection) && !(SideWindow.Content as Collection).isEditing)))
            {
                (SideFlowIn.EasingFunction as ExponentialEase).Exponent = (e.Velocities.Linear.X) * 4;
                ShowSideWindow(0);
            }
            else
            {
                SideFlowIn.From = SideGridTransform.X;
                SideFlowIn.To = 0;
                SideOpacity.From = SideGrid.Opacity;
                SideOpacity.To = 1;
                (SideFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
                SideStoryBoard.Begin();
            }
        }
        
        private void SideGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            SWindowX = 0;
        }

        double TWindowX = 0;

        private void TabListGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Velocities.Linear.X <= -0.5 && e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                (TabListFlowIn.EasingFunction as ExponentialEase).Exponent = -(e.Velocities.Linear.X) * 4;
                ShowTabListWindow(0);
            }
            else
            {
                TabListFlowIn.From = TabListGridTransform.X;
                TabListFlowIn.To = 0;
                TabListOpacity.From = TabListGrid.Opacity;
                TabListOpacity.To = 1;
                (TabListFlowIn.EasingFunction as ExponentialEase).Exponent = 8;
                TabListStoryBoard.Begin();
            }
        }

        private void TabListGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            TWindowX = 0;
        }

        private void TabListGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (IsTabListWindowOpen)
            {
                TWindowX = TWindowX + e.Delta.Translation.X;
                if (TWindowX < 0 && e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
                {
                    TabListGridTransform.X += e.Delta.Translation.X;
                }
                else
                {
                    TabListGridTransform.X = 0;
                }
            }
        }

        private void TabListCloseItem_Click(object sender, RoutedEventArgs e)
        {
            ShowTabListWindow(0);
            ListView.SelectedIndex = ListView.Items.IndexOf((sender as FrameworkElement).DataContext);
            if(ListView.SelectedIndex == -1)
            {
                return;
            }
            if (((MicrosoftEdge.TabItems[ListView.SelectedIndex] as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage2))
            {
                (((MicrosoftEdge.TabItems[ListView.SelectedIndex] as TabViewItem).Content as Frame).Content as WebPage2).CloseWebView();
            }
            else if (((MicrosoftEdge.TabItems[ListView.SelectedIndex] as TabViewItem).Content as Frame).Content.GetType() == typeof(WebPage))
            {
                (((MicrosoftEdge.TabItems[ListView.SelectedIndex] as TabViewItem).Content as Frame).Content as WebPage).CloseWebView();
            }

            MicrosoftEdge.TabItems.RemoveAt(ListView.SelectedIndex);
            TabList.RemoveAt(ListView.SelectedIndex);
            //TabsChanged();
            ShowTabListWindow(1);
        }

        private void ExitFS_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode && TitleDTransform.To == -92)
            {
                TitleDTransform.From = TitleBarTransform.Y;
                TitleDTransform.To = 0;
                TitleStoryBoard.Begin();

                (sender as Button).Height = ActualHeight;
                (sender as Button).VerticalAlignment = VerticalAlignment.Stretch;
                (sender as Button).Margin = new Thickness(0, 160, 0, 0);
            }
            else if(ApplicationView.GetForCurrentView().IsFullScreenMode && TitleDTransform.To == 0)
            {
                TitleDTransform.From = TitleBarTransform.Y;
                TitleDTransform.To = -92;
                TitleStoryBoard.Begin();

                (sender as Button).Height = 16;
                (sender as Button).VerticalAlignment = VerticalAlignment.Top;
                (sender as Button).Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                TitleDTransform.From = TitleBarTransform.Y;
                TitleDTransform.To = 0;
                TitleStoryBoard.Begin();

                (sender as Button).Visibility = Visibility.Collapsed;
            }
        }
    }
}
