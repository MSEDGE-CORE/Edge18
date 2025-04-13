using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App3
{
    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();

            ListView.ItemsSource = (Application.Current as App).HistoryList;
        }

        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Browser.SideWindowBackground_Click();
        }
        public void OnClosing()
        {
            ListView.ItemsSource = null;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView.SelectedIndex = ListView.Items.IndexOf(e.ClickedItem);
            if (ListView.SelectedItems.Count > 0 && ListView.SelectedItem != null)
            {
                MainPage.Browser.AddTab((Application.Current as App).HistoryList[ListView.SelectedIndex].HistoryUri.ToString());
            }
        }
    }
}
