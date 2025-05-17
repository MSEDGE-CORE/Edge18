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
using System.Diagnostics;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;

namespace App3.SettingsPages
{
    public sealed partial class File : Page
    {
        public File()
        {
            this.InitializeComponent();
        }

        private async void Collection_Import(object sender, RoutedEventArgs e)
        {
            Collection_Import_Button.IsEnabled = false;

            Windows.Storage.Pickers.FileOpenPicker OpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            OpenPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            OpenPicker.FileTypeFilter.Add(".html");
            OpenPicker.FileTypeFilter.Add(".json");
            Windows.Storage.StorageFile file = await OpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                if (file.FileType == ".json")
                {
                    using (var stream = await file.OpenStreamForReadAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string text = await reader.ReadToEndAsync();
                        var importedCollection = JsonConvert.DeserializeObject<ObservableCollection<Collection_List>>(text);
                        if (importedCollection != null)
                        {
                            foreach (var CollectionItem in importedCollection)
                            {
                                (Application.Current as App).CollectionList.Add(CollectionItem);
                            }
                        }
                    }
                }
                else if (file.FileType == ".html")
                {
                    // 创建 HtmlDocument 对象并加载 HTML 文件
                    var htmlDoc = new HtmlDocument();
                    try
                    {
                        htmlDoc.Load(await file.OpenStreamForReadAsync());
                    }
                    catch
                    {
                        return;
                    }

                    // 查找所有的 <dt> 节点，因为书签和文件夹通常包含在 <dt> 标签内
                    var dtNodes = htmlDoc.DocumentNode.SelectNodes("//dt");
                    if (dtNodes != null)
                    {
                        foreach (var dtNode in dtNodes)
                        {
                            // 查找 <a> 标签，它代表书签
                            var aNode = dtNode.SelectSingleNode("a");
                            if (aNode != null)
                            {
                                string title = aNode.InnerText.Trim();
                                string url = aNode.GetAttributeValue("href", "");
                                if (!string.IsNullOrEmpty(url))
                                {
                                    (Application.Current as App).CollectionList.Add(new Collection_List { CollectionTitle = title, CollectionUri = url });
                                }
                            }
                        }
                    }
                }
            }
            else
            {

            }

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string CollectionJson = JsonConvert.SerializeObject((Application.Current as App).CollectionList);
            try
            {
                Windows.Storage.StorageFile CollectionFile = await StorageFolder.CreateFileAsync("LocalStorage2\\Collections.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(CollectionFile, CollectionJson);
            }
            catch { }

            Collection_Import_Button.IsEnabled = true;
        }

        private async void History_Clear(object sender, RoutedEventArgs e)
        {
            HistoryClearFlyout.Hide();

            (Application.Current as App).HistoryList.Clear();

            string HistoryJson = JsonConvert.SerializeObject((Application.Current as App).HistoryList);
            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                Windows.Storage.StorageFile HistoryFile = await StorageFolder.CreateFileAsync("LocalStorage2\\History.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(HistoryFile, HistoryJson);
            }
            catch { }
        }

        private async void Collection_ExportToHtml(object sender, RoutedEventArgs e)
        {
            Collection_Export_Button.IsEnabled = false;

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile CollectionFile;

            // 创建 HTML 文档
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(@"<!DOCTYPE html><html><head><body></body></head></html>"); // 初始化基本结构

            try
            {
                // 获取头部，添加元数据和标题
                var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");

                var metaNode = new HtmlNode(HtmlNodeType.Element, headNode.OwnerDocument, 0);
                metaNode.Name = "meta";
                metaNode.Attributes.Add("charset", "UTF-8");
                headNode.AppendChild(metaNode);

                // 创建主体内容
                var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                var mainHeading = HtmlNode.CreateNode("<h1>收藏夹</h1>");
                bodyNode.AppendChild(mainHeading);

                // 创建无序列表
                var listNode = HtmlNode.CreateNode("<dl></dl>");
                bodyNode.AppendChild(listNode);

                // 遍历数据，生成列表项
                foreach (var item in (Application.Current as App).CollectionList)
                {
                    var listItem = HtmlNode.CreateNode("<dt></dt>");
                    var link = HtmlNode.CreateNode($"<a href=\"{(item.CollectionUri)}\">{(item.CollectionTitle)}</a>");
                    listItem.AppendChild(link);
                    listNode.AppendChild(listItem);
                }
            }
            catch { }

            Windows.Storage.Pickers.FileSavePicker SavePicker = new Windows.Storage.Pickers.FileSavePicker();
            SavePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            SavePicker.FileTypeChoices.Add("HTML", new List<string>() { ".html" });
            SavePicker.DefaultFileExtension = ".html";
            SavePicker.SuggestedFileName = "EdgeCollections " + DateTime.Now;
            CollectionFile = await SavePicker.PickSaveFileAsync();
            if (CollectionFile != null)
            {
                using (var stream = await CollectionFile.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.Write(htmlDoc.DocumentNode.OuterHtml);
                        stream.SetLength(stream.Position);
                    }
                }
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(CollectionFile);

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

        private async void Collection_ExportToJson(object sender, RoutedEventArgs e)
        {
            Collection_Export_Button.IsEnabled = false;

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile CollectionFile;
            string CollectionJson = JsonConvert.SerializeObject((Application.Current as App).CollectionList);

            Windows.Storage.Pickers.FileSavePicker SavePicker = new Windows.Storage.Pickers.FileSavePicker();
            SavePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            SavePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });
            SavePicker.DefaultFileExtension = ".json";
            SavePicker.SuggestedFileName = "EdgeCollections " + DateTime.Now;
            CollectionFile = await SavePicker.PickSaveFileAsync();
            if (CollectionFile != null)
            {
                using (var stream = await CollectionFile.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.Write(CollectionJson);
                        stream.SetLength(stream.Position);
                    }
                }
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(CollectionFile);

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

        private async void History_Export_Button_Click(object sender, RoutedEventArgs e)
        {
            History_Export_Button.IsEnabled = false;

            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile HistoryFile;
            string HistoryJson = JsonConvert.SerializeObject((Application.Current as App).HistoryList);

            Windows.Storage.Pickers.FileSavePicker SavePicker = new Windows.Storage.Pickers.FileSavePicker();
            SavePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            SavePicker.FileTypeChoices.Add("JSON", new List<string>() { ".json" });
            SavePicker.DefaultFileExtension = ".json";
            SavePicker.SuggestedFileName = "EdgeHistory " + DateTime.Now;
            HistoryFile = await SavePicker.PickSaveFileAsync();
            if (HistoryFile != null)
            {
                using (var stream = await HistoryFile.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.Write(HistoryJson);
                    }
                }
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(HistoryFile);

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

            History_Export_Button.IsEnabled = true;
        }

        private async void History_Import_Button_Click(object sender, RoutedEventArgs e)
        {
            History_Import_Button.IsEnabled = false;

            Windows.Storage.Pickers.FileOpenPicker OpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            OpenPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            OpenPicker.FileTypeFilter.Add(".json");
            Windows.Storage.StorageFile file = await OpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    string text = await reader.ReadToEndAsync();
                    var importedHistory = JsonConvert.DeserializeObject<ObservableCollection<History_List>>(text);
                    if (importedHistory != null)
                    {
                        int i = 0;
                        foreach (var historyItem in importedHistory)
                        {
                            (Application.Current as App).HistoryList.Insert(i, historyItem);
                            i++;
                        }
                    }
                }
            }

            try
            {
                string HistoryJson = JsonConvert.SerializeObject((Application.Current as App).HistoryList);
                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile HistoryFile = await StorageFolder.CreateFileAsync("LocalStorage2\\History.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(HistoryFile, HistoryJson);
            }
            catch { }

            History_Import_Button.IsEnabled = true;
        }
    }
}
