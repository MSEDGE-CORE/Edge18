using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App3
{
    public sealed partial class Collection : Page
    {
        public static MainPage Browser
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public bool isEditing = false;
        int CollectionCount = 0;

        public Collection()
        {
            this.InitializeComponent();

            Add_Collection_Flyout.ShouldConstrainToRootBounds = false;
            PageLoaded();

            ListView.ItemsSource = (Application.Current as App).CollectionList;
        }

        private void PageLoaded()
        {
            CollectionCount = (Application.Current as App).CollectionList.Count();
        }

        private void Add_Collection_Click(object sender, RoutedEventArgs e)
        {
            Collection_Title.Text = Browser.SelectedTab.Header.ToString();
            if(((Browser.SelectedTab.Content as Frame).Content.GetType() == typeof(WebPage2)))
            {
                Collection_Uri.Text = ((Browser.SelectedTab.Content as Frame).Content as WebPage2).WebLink;
            }
            else if(((Browser.SelectedTab.Content as Frame).Content.GetType() == typeof(WebPage)))
            {
                Collection_Uri.Text = ((Browser.SelectedTab.Content as Frame).Content as WebPage).WebLink;
            }
        }

        private async void Add_Collection_Complete_Click(object sender, RoutedEventArgs e)
        {
            Add_Collection_Flyout.Hide();
            if (Collection_Uri.Text == "about:blank")
            {
                return;
            }

            (Application.Current as App).CollectionList.Add(new Collection_List() { CollectionTitle = Collection_Title.Text, CollectionUri = Collection_Uri.Text });

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            CollectionCount += 1;
            string CollectionJson = JsonConvert.SerializeObject((Application.Current as App).CollectionList);
            try
            {
                Windows.Storage.StorageFile CollectionFile = await StorageFolder.CreateFileAsync("LocalStorage2\\Collections.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(CollectionFile, CollectionJson);
            }
            catch { }
        }

        private void Edit_Collection_Click(object sender, RoutedEventArgs e)
        {
            ListView.SelectedIndex = -1;
            isEditing = true;
            DefaultToolBar.Visibility = Visibility.Collapsed;
            EditToolBar.Visibility = Visibility.Visible;

            ListView.CanReorderItems = true;
            ListView.ReorderMode = ListViewReorderMode.Enabled;
            ListView.AllowDrop = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count > 0 && ListView.SelectedItem != null)
            {
                (Application.Current as App).CollectionList.RemoveAt(ListView.SelectedIndex);
                CollectionCount--;
            }
        }

        private async void Complete_Button_Click(object sender, RoutedEventArgs e)
        {
            ListView.SelectedIndex = -1;
            isEditing = false;
            DefaultToolBar.Visibility = Visibility.Visible;
            EditToolBar.Visibility = Visibility.Collapsed;
            ListView.CanReorderItems = false;
            ListView.ReorderMode = ListViewReorderMode.Disabled;
            ListView.AllowDrop = false;

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string CollectionJson = JsonConvert.SerializeObject((Application.Current as App).CollectionList);
            try
            {
                Windows.Storage.StorageFile CollectionFile = await StorageFolder.CreateFileAsync("LocalStorage2\\Collections.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(CollectionFile, CollectionJson);
            }
            catch { }
        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count > 0 && ListView.SelectedItem != null && ListView.SelectedIndex != 0)
            {
                string WTitle = (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionTitle;
                string WUri = (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionUri;
                (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionTitle = (Application.Current as App).CollectionList[ListView.SelectedIndex - 1].CollectionTitle;
                (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionUri = (Application.Current as App).CollectionList[ListView.SelectedIndex - 1].CollectionUri;
                (Application.Current as App).CollectionList[ListView.SelectedIndex - 1].CollectionTitle = WTitle;
                (Application.Current as App).CollectionList[ListView.SelectedIndex - 1].CollectionUri = WUri;
                int SIndex = ListView.SelectedIndex;
                ListView.ItemsSource = null;
                ListView.ItemsSource = (Application.Current as App).CollectionList;
                ListView.SelectedIndex = SIndex - 1;
            }
        }

        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItems.Count > 0 && ListView.SelectedItem != null && ListView.SelectedIndex != CollectionCount - 1)
            {
                string WTitle = (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionTitle;
                string WUri = (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionUri;
                (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionTitle = (Application.Current as App).CollectionList[ListView.SelectedIndex + 1].CollectionTitle;
                (Application.Current as App).CollectionList[ListView.SelectedIndex].CollectionUri = (Application.Current as App).CollectionList[ListView.SelectedIndex + 1].CollectionUri;
                (Application.Current as App).CollectionList[ListView.SelectedIndex + 1].CollectionTitle = WTitle;
                (Application.Current as App).CollectionList[ListView.SelectedIndex + 1].CollectionUri = WUri;
                int SIndex = ListView.SelectedIndex;
                ListView.ItemsSource = null;
                ListView.ItemsSource = (Application.Current as App).CollectionList;
                ListView.SelectedIndex = SIndex + 1;
            }
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
            if (!isEditing)
            {
                MainPage.Browser.AddTab((Application.Current as App).CollectionList[ListView.Items.IndexOf(e.ClickedItem)].CollectionUri.ToString());
                Browser.SideWindowBackground_Click();
            }
        }
    }
}
