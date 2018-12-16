﻿using MahApps.Metro.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Foundation;
using Windows.Storage;

namespace KSP_WPF
{
    public partial class StoryWriteWindow : MetroWindow
    {
        private struct ImageData
        {
            public Image Image;
            public string Path;
            public string EditKey;
            public void SetKey(string key)
            {
                EditKey = key;
            }
            public string GetKey()
            {
                return EditKey;
            }
            public void Remove(object arg0, dynamic arg1)
            {
                instance.imgs.Remove(this);
                instance.SP_Pictures.Children.Remove(Image);
                Image = null;
                instance.ValidatePanelHeight();
            }
        };
        private readonly List<ImageData> imgs = new List<ImageData>();
        private static StoryWriteWindow instance;
        public List<string> with_ids = new List<string>();
        public List<string> trust_ids = new List<string>();
        private readonly bool isEdit;
        private readonly bool isVideo = false;
        private readonly bool isShare = false;
        private readonly string editFeedID;
        private readonly List<string> editOldMediaPath = new List<string>();
        private readonly string shareFeedID;
        private readonly bool isShared = false;
        private string linkData;
        private string videoPath = null;
        private VideoData.Video videoData = null;
        private string videoMediaPath = null;
        private bool isInit = false;

        public StoryWriteWindow()
        {
            InitializeComponent();
            instance = this;
            isEdit = false;

            MainWindow.SetClickObject(BT_Link);
            MainWindow.SetClickObject(BT_LinkShow);
            MainWindow.SetClickObject(BT_Pic);
            MainWindow.SetClickObject(BT_WithFriend);
            MainWindow.SetClickObject(BT_QuoteFriend);
            MainWindow.SetClickObject(BT_Menu);
            if (!Properties.Settings.Default.DefaultFriendOnly)
                ComboRange.SelectedIndex = 0;
            else
                ComboRange.SelectedIndex = 1;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Main.Focus()));
            isInit = true;
        }
        public StoryWriteWindow(string feedID, bool isAllRead)
        {
            InitializeComponent();
            instance = this;
            BT_Pic.IsEnabled = false;
            BT_Pic.Foreground = Brushes.LightGray;
            BT_LinkShow.IsEnabled = false;
            BT_LinkShow.Foreground = Brushes.LightGray;
            BT_Link.IsEnabled = false;

            isEdit = false;
            isShare = true;
            shareFeedID = feedID;
            Title = "글 공유하기";
            TextBoxHelper.SetWatermark(TB_Main, "친구들에게 공유할 내용을 적어주세요");

            MainWindow.SetClickObject(BT_Link);
            MainWindow.SetClickObject(BT_LinkShow);
            MainWindow.SetClickObject(BT_Pic);
            MainWindow.SetClickObject(BT_WithFriend);
            MainWindow.SetClickObject(BT_QuoteFriend);
            MainWindow.SetClickObject(BT_Menu);

            if (!Properties.Settings.Default.DefaultFriendOnly)
                ComboRange.SelectedIndex = 0;
            else
                ComboRange.SelectedIndex = 1;

            if (!isAllRead)
            {
                CBI_All.Visibility = Visibility.Collapsed;
                ComboRange.SelectedIndex = 1;
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Main.Focus()));
            isInit = true;
        }

        public StoryWriteWindow(string feedID, string text, string permission, List<CommentData.Medium> medias, bool isShared, bool isVideo)
        {
            InitializeComponent();
            instance = this;

            BT_LinkShow.IsEnabled = false;
            BT_LinkShow.Foreground = Brushes.LightGray;
            BT_Link.IsEnabled = false;
            isEdit = true;
            this.isVideo = isVideo;
            editFeedID = feedID;
            if(medias != null)
            {
                foreach(var media in medias)
                {
                    if(!isVideo)
                    {
                        editOldMediaPath.Add(media.media_path);
                        string path = System.IO.Path.GetTempFileName();
                        WebClient client = new WebClient();
                        client.DownloadFile(media.origin_url, path);
                        AddImage(path, media.media_path);
                    }
                }
                ValidatePanelHeight();
            }
            this.isShared = isShared;
            if (isShared)
            {
                BT_Pic.IsEnabled = false;
                BT_Pic.Foreground = Brushes.LightGray;
            }

            MainWindow.SetClickObject(BT_Link);
            MainWindow.SetClickObject(BT_LinkShow);
            MainWindow.SetClickObject(BT_Pic);
            MainWindow.SetClickObject(BT_WithFriend);
            MainWindow.SetClickObject(BT_QuoteFriend);
            MainWindow.SetClickObject(BT_Menu);

            if (permission.Equals("A"))
                ComboRange.SelectedIndex = 0;
            if (permission.Equals("F"))
                ComboRange.SelectedIndex = 1;
            if (permission.Equals("P"))
                ComboRange.SelectedIndex = 2;
            if (permission.Equals("M"))
                ComboRange.SelectedIndex = 3;

            TB_Main.Text = text;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Main.Focus()));
            isInit = true;
        }

        private void TB_Main_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && videoMediaPath == null)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string path in files)
                {
                    if (System.IO.Path.GetExtension(path).ToLower().Equals(".mp4"))
                    {
                        videoPath = path;
                        break;
                    }
                    else if (!AddImage(path)) break;
                }
                ValidatePanelHeight();
                e.Handled = true;
            }
        }

        private bool AddImage(string path)
        {
            return AddImage(path, null);
        }
        private bool AddImage(string path, string editKey)
        {
            try
            {
                if (imgs.Count < 20)
                {
                    Image image = new Image();
                    BitmapImage bi = new BitmapImage(new Uri(path));
                    image.Source = bi;
                    image.Margin = new Thickness(5, 5, 5, 5);
                    image.Width = 80;
                    image.Height = image.Width;
                    image.Stretch = Stretch.UniformToFill;
                    ImageData imgData = new ImageData() { Image = image, Path = path, EditKey=editKey };
                    image.MouseLeftButtonDown += imgData.Remove;
                    imgs.Add(imgData);
                    SP_Pictures.Children.Add(image);
                    videoPath = null;
                    return true;
                }
                else
                {
                    ShowImageAlert();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ValidatePanelHeight()
        {
            if (imgs.Count > 0)
            {
                SP_Pictures.Visibility = Visibility.Visible;
                GD_Link.Visibility = Visibility.Collapsed;
                BT_Link.IsEnabled = false;
                BT_Link.Foreground = Brushes.LightGray;
                BT_LinkShow.Foreground = Brushes.LightGray;
                BT_Video.IsEnabled = false;
                BT_Video.Foreground = Brushes.LightGray;
            }
            else if(videoPath != null || isVideo) {
                SP_Pictures.Visibility = Visibility.Collapsed;
                BT_Link.IsEnabled = false;
                BT_Link.Foreground = Brushes.LightGray;
                BT_LinkShow.Foreground = Brushes.LightGray;
                BT_Pic.IsEnabled = false;
                BT_Pic.Foreground = Brushes.LightGray;
                if (!isVideo)
                {
                    BT_Video.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                }
                else
                {
                    BT_Video.IsEnabled = false;
                    BT_Video.Foreground = Brushes.LightGray;
                }
            }
            else
            {
                SP_Pictures.Visibility = Visibility.Collapsed;
                BT_Link.IsEnabled = true;
                BT_Link.Foreground = Brushes.Gray;
                BT_LinkShow.Foreground = Brushes.Gray;
                BT_Video.IsEnabled = false;
                BT_Video.Foreground = Brushes.LightGray;
            }
        }

        private void ShowImageAlert()
        {
            MessageBox.Show("사진의 최대 갯수는 20장입니다.");
        }

        private void BT_Pic_Click(object sender, RoutedEventArgs e)
        {
            if (imgs.Count < 20)
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.jpeg; *.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|GIF Image File (*.gif)|*.gif",
                    DefaultExt = "jpg",
                    Multiselect = true
                };
                ofd.ShowDialog();
                if (ofd.FileNames.Length > 0)
                {
                    foreach (string path in ofd.FileNames)
                    {
                        if (!AddImage(path))
                            break;
                    }
                    ValidatePanelHeight();
                }
            }
            else
                ShowImageAlert();

            ValidatePanelHeight();
        }
        
        private async Task<bool> WriteText(string text, string permission, bool isCommentable, bool isSharable)
        {
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\n");
            string commentable = isCommentable ? "true" : "false";
            string sharable = isSharable ? "true" : "false";
            List<QuoteData> rawContent = GlobalHelper.GetQuoteDataFromString(text);
            string textContent = Uri.EscapeDataString(JsonConvert.SerializeObject(rawContent, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("permission=" + permission + "&comment_all_writable=" + commentable + "&is_must_read=false&enable_share=" + sharable);
            postDataBuilder.Append("&content=" + textContent);

            List<string> keys = new List<string>();
            foreach (ImageData img in imgs)
            {
                string key;
                if (img.EditKey == null)
                    key = await UploadImage(img);
                else
                    key = img.EditKey;

                keys.Add(key);
            }

            if (with_ids.Count > 0)
                postDataBuilder.Append("&with_tags=" + Uri.EscapeDataString(JsonConvert.SerializeObject(with_ids)));
            if (trust_ids.Count > 0)
                postDataBuilder.Append("&allowed_profile_ids=" + Uri.EscapeDataString(JsonConvert.SerializeObject(trust_ids)));

            if (imgs.Count > 0)
                postDataBuilder.Append("&media_type=image");
            else if (videoData != null)
            {
                postDataBuilder.Append("&media_type=video");
                postDataBuilder.Append("&" + Uri.EscapeDataString("media_path[]") + "=" + Uri.EscapeDataString(videoData.access_key));
            }

            for (int i = 0; i < imgs.Count; i++)
            {
                postDataBuilder.Append("&" + Uri.EscapeDataString("caption[]") + "=");
            }
            foreach (string key in keys)
            {
                postDataBuilder.Append("&" + Uri.EscapeDataString("media_path[]") + "=" + Uri.EscapeDataString(key));
            }
            foreach (string mediaPath in editOldMediaPath)
            {
                postDataBuilder.Append("&" + Uri.EscapeDataString("old_media_path[]") + "=" + Uri.EscapeDataString(mediaPath));
            }

            if (linkData != null)
            {
                postDataBuilder.Append("&scrap_content=" + Uri.EscapeDataString(linkData));
            }

            imgs.Clear();

            string postData = postDataBuilder.ToString();

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            string requestURI = "https://story.kakao.com/a/activities";
            if (isEdit)
                requestURI = "https://story.kakao.com/a/activities/" + editFeedID + "/content";

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            if (isEdit)
                request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com"));
            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            request.Headers["Cache-Control"] = "max-age=0";

            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko";

            request.Headers["DNT"] = "1";

            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "application/json";

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await (new StreamReader(respReader, Encoding.UTF8)).ReadToEndAsync();
            respReader.Close();

            return true;
        }


        private async Task<bool> UploadVideo(string filepath)
        {
            StreamReader fileStream = new StreamReader(filepath);

            string requestURI = "https://up-api-kage-4story-video.kakao.com/web/webstory-video/";

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com"));

            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            request.Headers["Cache-Control"] = "max-age=0";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4";

            request.Headers["DNT"] = "1";

            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "*/*";

            Stream writeStream = await request.GetRequestStreamAsync();

            WriteMultipartForm(writeStream, boundary, null, System.IO.Path.GetFileName(filepath), System.Web.MimeMapping.GetMimeMapping(filepath), fileStream.BaseStream);
            fileStream.Close();

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();

            string respResult = await (new StreamReader(respReader, Encoding.UTF8)).ReadToEndAsync();
            respReader.Close();

            videoData = JsonConvert.DeserializeObject<VideoData.Video>(respResult);
            return true;
        }

        private async Task<string> UploadImage(ImageData img)
        {
            StreamReader fileStream = new StreamReader(img.Path);

            string requestURI = "https://up-api-kage-4story.kakao.com/web/webstory-img/";

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com"));

            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            request.Headers["Cache-Control"] = "max-age=0";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4";

            request.Headers["DNT"] = "1";

            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "*/*";

            Stream writeStream = await request.GetRequestStreamAsync();

            WriteMultipartForm(writeStream, boundary, null, System.IO.Path.GetFileName(img.Path), System.Web.MimeMapping.GetMimeMapping(img.Path), fileStream.BaseStream);
            fileStream.Close();

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();

            string respResult = await (new StreamReader(respReader, Encoding.UTF8)).ReadToEndAsync();
            respReader.Close();

            UploadedImageProp result = JsonConvert.DeserializeObject<UploadedImageProp>(respResult);
            return result.access_key + "/" + result.info.original.filename + "?width=" + result.info.original.width + "&height=" + result.info.original.height + "&avg=" + result.info.original.avg;
        }
        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, Stream fileStream)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            /// Content-Disposition: form-data; name="file_1"; filename="waifu.png"
            //Content - Type: image / png

            string formdataTemplate = "Content-Disposition; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file_1", fileName, fileContentType));
            // Write the file data to the stream.
            byte[] buffer = new byte[4096];
            while ((fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                WriteToStream(s, buffer);
            }
            fileStream.Dispose();
            WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        private async void BT_Submit_Click(object sender, RoutedEventArgs e)
        {
            if(BT_Submit.IsEnabled == true)
            {
                BT_Submit.IsEnabled = false;
                string baseStr = "게시중...";
                BT_Submit.Content = baseStr;
                if(videoPath != null)
                {
                    await UploadVideo(videoPath);
                    await GetPercentVideo(videoData.access_key);
                    await GetMetaVideo(videoData.access_key);
                }
                string permission = "F";

                switch (ComboRange.SelectedIndex)
                {
                    case 0:
                        permission = "A";
                        break;
                    case 1:
                        permission = "F";
                        break;
                    case 2:
                        permission = "P";
                        break;
                    case 3:
                        permission = "M";
                        break;
                }
                if (permission.Equals("P") && trust_ids.Count == 0)
                {
                    MessageBox.Show("편한 친구를 선택해주세요.");
                }
                else
                {
                    try
                    {
                        if(!isShare)
                            await WriteText(TB_Main.Text, permission, (bool)CB_Comment.IsChecked, (bool)CB_Share.IsChecked);
                        else
                            await KakaoRequestClass.ShareFeed(shareFeedID, TB_Main.Text, permission, (bool)CB_Comment.IsChecked, with_ids, trust_ids);
                        if (MainWindow.profileTimeLineWindow != null)
                            await MainWindow.profileTimeLineWindow.RefreshTimeline(null, true);
                        if (MainWindow.timeLineWindow != null)
                            await MainWindow.timeLineWindow.RefreshTimeline(null, true);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("업로드 도중 오류가 발생했습니다.\n"+ex.Message);
                    }
                }
                BT_Submit.IsEnabled = true;
                BT_Submit.Content = "게시";
            }
        }

        private async Task<bool> GetPercentVideo(string access_key)
        {
            string requestURI = "https://story.kakao.com/a/kage/video/wcheck/" + access_key + "/?_t=0";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);

            request.Method = "GET";
            
            request.CookieContainer = new CookieContainer();
            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";

            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko";

            request.Headers["DNT"] = "1";

            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com/";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "application/json";

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            var respReader = response.GetResponseStream();
            string respResult = new StreamReader(respReader).ReadToEnd();
            respReader.Close();
            response.Close();
            VideoData.Percent pecrentData = JsonConvert.DeserializeObject<VideoData.Percent>(respResult);
            if (pecrentData.code == 200 && pecrentData.percent == 100)
            {
                return true;
            }
            else
            {
                await Task.Delay(1500);
                return await GetPercentVideo(access_key);
            }
        }

        private async Task<bool> GetMetaVideo(string access_key)
        {
            string requestURI = "https://story.kakao.com/a/kage/video/dn/" + access_key + "/meta.json";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);

            request.Method = "GET";

            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            request.Headers["Cache-Control"] = "max-age=0";

            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko";

            request.Headers["DNT"] = "1";

            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com/";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "application/json";

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await (new StreamReader(respReader, Encoding.UTF8)).ReadToEndAsync();
            respReader.Close();
            return true;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int threshold = 24;
            ScrollViewer scv = (ScrollViewer)sender;
            double target = scv.HorizontalOffset - Math.Min(Math.Max(e.Delta, -threshold), threshold);
            scv.ScrollToHorizontalOffset(target);
            e.Handled = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PasteImage();
        }

        private void PasteImage()
        {
            if(Clipboard.ContainsImage())
            {
                var image = System.Windows.Forms.Clipboard.GetImage();
                string tempFile = System.IO.Path.GetTempFileName();
                image.Save(tempFile, ImageFormat.Jpeg);
                image.Dispose();
                AddImage(tempFile);
                ValidatePanelHeight();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.storyWriteWindow = null;
        }

        private void TB_Main_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TB_Main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V && !isShare && !isShared)
            {
                PasteImage();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                BT_Submit_Click(BT_Submit, null);
            }
        }

        private void BT_WithFriend_Click(object sender, RoutedEventArgs e)
        {
            FriendSelectWindow fsw = new FriendSelectWindow((ids, names) =>
            {
                with_ids.Clear();
                foreach (string id in ids)
                {
                    with_ids.Add(id);
                }
            }, false)
            {
                Title = "필독 친구 선택",
                Owner = this
            };
            fsw.ShowDialog();
        }

        private void ComboRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            if (box.SelectedIndex == 2 && isInit)
            {
                FriendSelectWindow friendSelectWindow = new FriendSelectWindow((ids, names) =>
                   {
                       trust_ids.Clear();
                       if (ids.Count == 0)
                       {
                           MessageBox.Show("편한 친구를 선택해주세요.");
                           box.SelectedIndex = 1;
                       }
                       else
                       {
                           foreach (string id in ids)
                           {
                               trust_ids.Add(id);
                           }
                       }
                   }, false)
                {
                    Title = "편한 친구 선택",
                    Owner = this
                };
                FriendSelectWindow fsw = friendSelectWindow;
                fsw.ShowDialog();
            }
        }

        private void BT_QuoteFriend_Click(object sender, RoutedEventArgs e)
        {
            FriendSelectWindow fsc = new FriendSelectWindow((ids, names) =>
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    string append = "{!{{" + "\"id\":\"" + ids[i] + "\", \"type\":\"profile\", \"text\":\"" + names[i] + "\"}}!} ";
                    if (i + 1 < ids.Count)
                    {
                        append += ", ";
                    }
                    int lastPos = TB_Main.CaretIndex;
                    TB_Main.Text = TB_Main.Text.Insert(TB_Main.CaretIndex, append);
                    TB_Main.CaretIndex = lastPos + append.Length;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Main.Focus()));
                }
            }, false)
            {
                Owner = this
            };
            fsc.ShowDialog();
        }

        private async void BT_Link_Click(object sender, RoutedEventArgs e)
        {
            if (linkData == null)
            {
                string url = TB_Link.Text;
                BT_Link.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressUpload;
                BT_Link.IsEnabled = false;
                linkData = await KakaoRequestClass.GetScrapData(url);
                if (linkData != null)
                {
                    BT_Link.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                    BT_Pic.IsEnabled = false;
                    BT_Pic.Foreground = Brushes.LightGray;
                }
                else
                {
                    MessageBox.Show("오류가 발생했습니다.\n다시 시도해보세요.");
                    BT_Link.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                }
                BT_Link.IsEnabled = true;
            }
            else
            {
                linkData = null;
                BT_Link.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                BT_Pic.IsEnabled = true;
                BT_Pic.Foreground = Brushes.Gray;
            }
        }

        private void BT_Pic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Pic_Click(null, null);
            e.Handled = true;
        }

        private void BT_Link_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(BT_Link.IsEnabled)
                BT_Link_Click(null, null);
            e.Handled = true;
        }

        private void BT_LinkShow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BT_Link.IsEnabled && !isEdit)
            {
                if(GD_Link.Visibility == Visibility.Collapsed)
                    GD_Link.Visibility = Visibility.Visible;
                else
                    GD_Link.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_QuoteFriend_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_QuoteFriend_Click(null, null);
            e.Handled = true;
        }

        private void BT_WithFriend_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_WithFriend_Click(null, null);
            e.Handled = true;
        }

        private void BT_Menu_Click(object sender, RoutedEventArgs e)
        {
            FL_Menu.IsOpen = !FL_Menu.IsOpen;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                FL_Menu.IsOpen = false;
            }
        }

        private void TB_Link_KeyDown(object sender, KeyEventArgs e)
        {
            if (BT_Link.IsEnabled && e.Key == Key.Enter)
                BT_Link_Click(null, null);
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(!(e.Source is System.Windows.Controls.TextBox) && !(e.Source is System.Windows.Controls.ComboBox) && !(e.Source is System.Windows.Controls.ComboBoxItem))
                e.Handled = true;
        }

        private void BT_Video_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(BT_Video.IsEnabled && videoPath == null && videoMediaPath == null)
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Video File (*.mp4)|*.mp4",
                    DefaultExt = "mp4",
                };
                ofd.ShowDialog();
                if (ofd.FileName != null)
                {
                    videoPath = ofd.FileName;
                    ValidatePanelHeight();
                }
            }
            else if(videoPath != null)
            {
                videoPath = null;
                SP_Pictures.Visibility = Visibility.Collapsed;
                BT_Link.IsEnabled = true;
                BT_Link.Foreground = Brushes.Gray;
                BT_LinkShow.Foreground = Brushes.Gray;
                BT_Pic.IsEnabled = true;
                BT_Pic.Foreground = Brushes.Gray;
                BT_Video.Foreground = Brushes.Gray;
                BT_Video.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camcorder;
            }
            e.Handled = true;
        }
    }
}
