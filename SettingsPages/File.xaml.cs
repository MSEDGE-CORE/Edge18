using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Provider;
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
    public sealed partial class File : Page
    {
        public File()
        {
            this.InitializeComponent();

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue History_Items = (ApplicationDataCompositeValue)LocalSettings.Values["HistoryItems"];
            if (History_Items != null)
            {
                HistoryItems_Selection.SelectedIndex = (int)History_Items["HistoryItems"];
            }
            else
            {
                HistoryItems_Selection.SelectedIndex = 0;
            }
        }

        private void HistoryItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int HistoryItems = HistoryItems_Selection.SelectedIndex;
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue History_Items = new Windows.Storage.ApplicationDataCompositeValue();
            History_Items["HistoryItems"] = HistoryItems_Selection.SelectedIndex;
            LocalSettings.Values["HistoryItems"] = History_Items;

            (Application.Current as App).HistoryList.Clear();
            (Application.Current as App).GetHistory();
        }

        private async void Collection_Import(object sender, RoutedEventArgs e)
        {
            Collection_Import_Button.IsEnabled = false;

            List<string> CollectionFile = new List<string> { };
            Windows.Storage.Pickers.FileOpenPicker OpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            OpenPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            OpenPicker.FileTypeFilter.Add(".txt");
            Windows.Storage.StorageFile file = await OpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                int CollectionCount = 0;

                using (var stream = await file.OpenStreamForReadAsync())
                {
                    using (var tw = new StreamReader(stream))
                    {
                        CollectionCount = Int32.Parse(tw.ReadLine());
                        for (int i = 1; i <= CollectionCount; i++)
                        {
                            CollectionFile.Add(tw.ReadLine());
                            CollectionFile.Add(tw.ReadLine());
                        }
                    }
                }

                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile Count = await StorageFolder.CreateFileAsync("Collection\\CollectionCount", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(Count, (CollectionCount + (Application.Current as App).CollectionList.Count()).ToString());
                for (int i = 1; i <= CollectionCount; i++)
                {
                    string CollectionTitle = CollectionFile[i * 2 - 2] + "\n" + CollectionFile[i * 2 - 1] + "\n";
                    string FileCollectionTitle = "Collection\\CollectionTitle" + (i + (Application.Current as App).CollectionList.Count()).ToString();
                    Windows.Storage.StorageFile Title = await StorageFolder.CreateFileAsync(FileCollectionTitle, Windows.Storage.CreationCollisionOption.OpenIfExists);
                    await Windows.Storage.FileIO.WriteTextAsync(Title, CollectionTitle);
                }

                (Application.Current as App).GetCollection();
            }
            else
            {

            }

            Collection_Import_Button.IsEnabled = true;
        }

        private async void Collection_Export(object sender, RoutedEventArgs e)
        {
            Collection_Export_Button.IsEnabled = false;

            int CollectionCount = 0;
            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file;
            try
            {
                file = await StorageFolder.GetFileAsync("Collection\\CollectionCount");
                var Count = await Windows.Storage.FileIO.ReadLinesAsync(file);
                CollectionCount = Int32.Parse(Count[0]);
            }
            catch
            {

            }
            string CollectionFile = CollectionCount.ToString() + "\n";
            for (int i = 0; i < CollectionCount; i++)
            {
                CollectionFile += (Application.Current as App).CollectionList[i].CollectionTitle + "\n" + (Application.Current as App).CollectionList[i].CollectionUri + "\n";
            }

            Windows.Storage.Pickers.FileSavePicker SavePicker = new Windows.Storage.Pickers.FileSavePicker();
            SavePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            SavePicker.FileTypeChoices.Add("文本文档", new List<string>() { ".txt" });
            SavePicker.DefaultFileExtension = ".txt";
            SavePicker.SuggestedFileName = "EdgeCollections";
            file = await SavePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.WriteLine(CollectionFile);
                    }
                }
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {

                }
                else
                {

                }
            }
            else
            {

            }

            Collection_Export_Button.IsEnabled = true;
        }

        private async void History_Clear(object sender, RoutedEventArgs e)
        {
            HistoryClearFlyout.Hide();

            (Application.Current as App).HistoryList.Clear();

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile Count = await StorageFolder.CreateFileAsync("History\\HistoryCount", Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.WriteTextAsync(Count, "0");
        }
    }
}
