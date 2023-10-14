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
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();

            Grid.Height = Grid.MinHeight = Grid.MaxHeight = (Application.Current as App).WebViewHeight - 216;

            ListView.ItemsSource = (Application.Current as App).HistoryList;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListView.SelectedItems.Count > 0 && ListView.SelectedItem != null)
            {
                (Application.Current as App).TabStartLink = (Application.Current as App).HistoryList[ListView.SelectedIndex].HistoryUri.ToString(); 
                MainPage.Browser.TabView_AddButtonClick(null, null);
            }
        }
    }
}
