using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NaiveMediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".mp4");



            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                FileName.Text = "正在播放 " + file.Name;
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                naiveMediaplayer.SetSource(stream, file.ContentType);

                naiveMediaplayer.Play();
            }
            else
            {
                FileName.Text = "Operation cancelled.";
            }
        }

            private async void CacheButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    var httpClient = new HttpClient();
                    Uri url = new Uri(uriString: URLBox.Text);
                    var buffer = await httpClient.GetBufferAsync(url);
                    FileName.Text = url.ToString();
                    FileName.Text = FileName.Text.Substring(FileName.Text.LastIndexOf('/') + 1);

                    if (buffer != null && buffer.Length > 0u)
                    {
                        var file = await KnownFolders.MusicLibrary.CreateFileAsync(FileName.Text, CreationCollisionOption.ReplaceExisting);
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        await stream.FlushAsync();
                   
                    }
                    var streamfile = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    naiveMediaplayer.SetSource(streamfile, file.ContentType);
                    naiveMediaplayer.Play();
                        return;
                    }
                }
                catch { }
                return;

            }
            private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            Uri url = new Uri(uriString: URLBox.Text);
            naiveMediaplayer.Source = url;
            FileName.Text = naiveMediaplayer.Source.ToString();
            FileName.Text = FileName.Text.Substring(FileName.Text.LastIndexOf('/') + 1);
            naiveMediaplayer.Play();

        }
    }
}
