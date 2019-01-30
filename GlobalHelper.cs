using DesktopNotifications;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using XamlAnimatedGif;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using Newtonsoft.Json;
using System.Windows.Input;
using WPFMediaKit.DirectShow.Controls;
using System.Text.RegularExpressions;

namespace KSP_WPF
{
    class GlobalHelper
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(DateTime date)
        {
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
        
        public static void HandleScroll(object sender, MouseWheelEventArgs e)
        {
            int threshold = 48;
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            double target = scrollViewer.VerticalOffset - Math.Min(Math.Max(e.Delta, -threshold), threshold);
            scrollViewer.ScrollToVerticalOffset(target);
            e.Handled = true;
        }
        
        public static void SubContentMouseEvent(object s, MouseButtonEventArgs e)
        {
            string id = (string)((FrameworkElement)s).Tag;
            try
            {
                TimeLineWindow tlw = new TimeLineWindow(id);
                tlw.Show();
                tlw.Activate();
            }
            catch (Exception)
            {
                MessageBox.Show("접근이 불가능한 스토리입니다.");
            }
            e.Handled = true;
        }
        
        public static void ShowNotification(string title, string message, string URL)
        {
            if (Environment.OSVersion.Version.Major == 10)
            {
                ShowNotification(title, message, URL, null, null, null, null, null, null);
            }
            else
            {
                MainWindow.instance._notifyIcon.ShowBalloonTip(5, title, message, System.Windows.Forms.ToolTipIcon.Info);
                MainWindow.instance._notifyIcon.Tag = URL;
            }
        }

        public static void CopyImageHandler(object source, MouseButtonEventArgs e)
        {
            try
            {
                Image img = (Image)source;
                System.Windows.Clipboard.SetImage(img.Source as BitmapImage);
                MessageBox.Show("클립보드에 이미지가 복사됐습니다");
            }
            catch (Exception)
            {
                Image image = (Image)source;
                if (image.Tag is string uri)
                    SaveGIFImage(image);
            }
            if (e != null)
                e.Handled = true;
        }

        public static void SaveGIFImage(Image image)
        {
            string uri = (string)image.Tag;
            if (uri != null)
            {
                Microsoft.Win32.SaveFileDialog sdf = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "image.gif",
                    Filter = "GIF Image|*.gif",
                    Title = "GIF 이미지 저장"
                };

                if (sdf.ShowDialog() == true)
                    using (WebClient client = new WebClient())
                        client.DownloadFile(uri, sdf.FileName);
            }
        }

        public static void SaveImageHandler(object source, MouseButtonEventArgs e)
        {
            Image image = (Image)source;
            ImageViewerWindow imageViewer = new ImageViewerWindow();
            imageViewer.Show();
            imageViewer.Activate();
            imageViewer.Focus();
            imageViewer.currentImage = image;
            if(image.Tag is string)
                AssignImage(imageViewer.IMG_Main, (string) image.Tag);
            else
                imageViewer.IMG_Main.Source = ((Image)source).Source;
            imageViewer.ZB_Main.FitToBounds();
            e.Handled = true;
        }

        public static void ShowOfflineMessage()
        {
            MessageBox.Show("로그인상태가 아니거나 오프라인 상태입니다.");
        }

        public async static Task<bool> CheckUpdate()
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp("https://project-6817963503898600526.firebaseapp.com/KSP/latest.version");
            webRequest.Method = "GET";
            try
            {
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                if (respResult.Equals(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                    return true;
                else
                    return false;
            }
            catch (Exception) { }
            return true;
        }

        public static void SaveImageToFile(Image image)
        {
            if (image.Tag is string uri)
            {
                if (uri.Contains(".gif"))
                {
                    SaveGIFImage(image);
                    return;
                }
            }
            Microsoft.Win32.SaveFileDialog sdf = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "image.png",
                Filter = "Png Image|*.png",
                Title = "이미지 저장"
            };

            if (sdf.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                FileStream stream = new FileStream(sdf.FileName, FileMode.Create);
                encoder.Save(stream);
            }
        }

        public static void ShowNotification(string title, string message, string URL, string commentID, string id, string name, string writer, string identity, string thumbnailURL)
        {
            if(Environment.OSVersion.Version.Major == 10)
            {
                try
                {
                    var Visual = new Microsoft.Toolkit.Uwp.Notifications.ToastVisual()
                    {
                        BindingGeneric = new Microsoft.Toolkit.Uwp.Notifications.ToastBindingGeneric()
                        {
                            Children = {
                                new Microsoft.Toolkit.Uwp.Notifications.AdaptiveText()
                                {
                                    Text = title
                                },

                                new Microsoft.Toolkit.Uwp.Notifications.AdaptiveText()
                                {
                                    Text = message
                                }
                            }
                        }
                    };
                    if (thumbnailURL != null)
                    {
                        Visual.BindingGeneric.HeroImage = new Microsoft.Toolkit.Uwp.Notifications.ToastGenericHeroImage()
                        {
                            Source = thumbnailURL,
                        };
                    }
                    Microsoft.Toolkit.Uwp.Notifications.ToastActionsCustom Action;
                    if (URL == null)
                    {
                        Action = new Microsoft.Toolkit.Uwp.Notifications.ToastActionsCustom();
                    }
                    else
                    {
                        if (commentID != null)
                        {
                            Action = new Microsoft.Toolkit.Uwp.Notifications.ToastActionsCustom()
                            {
                                Inputs = {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastTextBox("tbReply")
                                    {
                                        PlaceholderContent = "답장 작성하기",
                                    },
                                },
                                Buttons =
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastButton("보내기", URL + "REPLY!@#$%" + "R!@=!!" + id + "R!@=!!" + name + "R!@=!!" + writer + "R!@=!!" + identity)
                                    {
                                        ActivationType = Microsoft.Toolkit.Uwp.Notifications.ToastActivationType.Background,
                                        TextBoxId = "tbReply"
                                    },
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastButton("좋아요", URL + "LIKE!@#$%" + commentID),
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastButton("열기", URL)
                                },
                            };
                        }
                        else
                        {
                            Action = new Microsoft.Toolkit.Uwp.Notifications.ToastActionsCustom()
                            {
                                Buttons =
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastButton("열기", URL)
                                },
                            };
                        }
                    }
                    var toastContent = new Microsoft.Toolkit.Uwp.Notifications.ToastContent()
                    {
                        Visual = Visual,
                        Actions = Action,
                    };
                    var toastXml = new Windows.Data.Xml.Dom.XmlDocument();
                    toastXml.LoadXml(toastContent.GetContent());
                    var toast = new Windows.UI.Notifications.ToastNotification(toastXml);
                    DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
                }
                catch (Exception) { }
            }
        }

        public static TextBlock GetWithFriendTB(CommentData.PostData data)
        {
            TextBlock TB_Closest_With = new TextBlock();
            CommentData.ClosestWithTag first = data.closest_with_tags.First();
            string headStr;
            string tailStr = "과 함께";
            if (data.closest_with_tags.Count > 1)
                headStr = $"{first.display_name}님 외 {(data.closest_with_tags.Count - 1).ToString()}명";
            else
                headStr = $"{first.display_name}님";

            Bold boldContent = new Bold(new Run(headStr));
            MainWindow.SetClickObject(boldContent);
            List<string> ids = new List<string>();
            foreach (var tag in data.closest_with_tags)
            {
                ids.Add(tag.id);
            }
            boldContent.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (data.closest_with_tags.Count > 1)
                {
                    UserInfoWindow userInfoWindow = new UserInfoWindow(ids)
                    {
                        Title = "함께하는 친구 목록"
                    };
                    userInfoWindow.Show();
                    userInfoWindow.Focus();
                }
                else
                {
                    TimeLineWindow tlw = new TimeLineWindow(first.id);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                }
                e.Handled = true;
            };

            TB_Closest_With.Inlines.Add(boldContent);
            TB_Closest_With.Inlines.Add(new Run(tailStr));

            return TB_Closest_With;
        }
        public static string GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();
            var rawHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in rawHash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static string defaultPath = System.IO.Path.GetTempPath();

        private static void SetImage(Ellipse image, string path, string uri, bool isGIF)
        {
            image.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(path);
                    bi.EndInit();
                    var fill = new ImageBrush(bi);
                    image.Fill = fill;
                }
                catch (Exception)
                {
                    await Task.Delay(100);
                    SetImage(image, path, uri, isGIF);
                }
            });
        }
        private static void SetImage(Image image, string path, string uri, bool isGIF)
        {
            if (image == null) return;
            if (isGIF)
                image.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        image.Tag = uri;
                        AnimationBehavior.SetSourceUri(image, new Uri(path));
                        AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                        AnimationBehavior.SetAutoStart(image, true);
                    }
                    catch (Exception)
                    {
                        await Task.Delay(100);
                        SetImage(image, path, uri, isGIF);
                    }
                });
            else
                image.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(path);
                        bi.EndInit();
                        bi.Freeze();
                        image.Source = bi;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(100);
                        SetImage(image, path, uri, isGIF);
                    }
                });
        }
        private static void SetImage(MediaElement image, string path, string uri, bool isGIF)
        {
            if (image == null) return;
            image.Dispatcher.Invoke(async () =>
            {
                try
                {
                    image.LoadedBehavior = MediaState.Manual;
                    image.Source = new Uri(path, UriKind.Absolute);
                    image.Volume = 1;
                    image.Position = TimeSpan.Zero;

                    image.MediaEnded += (s, e) =>
                    {
                        image.Position = TimeSpan.FromMilliseconds(1);
                        image.Play();
                    };
                    image.Play();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.StackTrace);
                    await Task.Delay(100);
                    SetImage(image, path, uri, isGIF);
                }
            });
        }

        public static async void AssignImage(dynamic image, string uri)
        {
            if (image == null) return;
            if (uri != null && uri.Length > 0)
            {
                string hash = GetHash(uri);
                string path;
                if (image is MediaElement && uri.ToLower().Contains("mp4?"))
                    path = System.IO.Path.Combine(defaultPath, hash + ".mp4");
                else if (image is MediaElement && uri.ToLower().Contains(".jpg?"))
                    path = System.IO.Path.Combine(defaultPath, hash + ".jpg");
                else if (image is MediaElement && uri.ToLower().Contains(".gif?"))
                    path = System.IO.Path.Combine(defaultPath, hash + ".gif");
                else
                    path = System.IO.Path.Combine(defaultPath, hash + ".proftemp");
                bool isGIF = uri.Contains(".gif");
                if (File.Exists(path))
                {
                    if (image is Image)
                        SetImage(image as Image, path, uri, isGIF);
                    else if (image is Ellipse)
                        SetImage(image as Ellipse, path, uri, isGIF);
                    else if (image is MediaElement)
                        SetImage(image as MediaElement, path, uri, isGIF);
                }
                else
                    await Task.Run(async () =>
                    {
                        try
                        {
                            WebClient client = new WebClient();
                            client.DownloadFile(uri, path);
                            client.Dispose();
                        }
                        catch (Exception)
                        {
                            await Task.Delay(100);
                            AssignImage(image, uri);
                        }
                        if (image is Image)
                            SetImage(image as Image, path, uri, isGIF);
                        else if (image is Ellipse)
                            SetImage(image as Ellipse, path, uri, isGIF);
                        else if (image is MediaElement)
                            SetImage(image as MediaElement, path, uri, isGIF);
                    });
            }
        }

        public static void RefreshScrap(TimeLineData.Scrap data, ScrapControl Scrap)
        {
            Scrap.Visibility = Visibility.Visible;
            if (data.image == null || data.image.Count == 0)
                Scrap.CD_Image.Width = new GridLength(0);
            else
                AssignImage(Scrap.Image, data.image[0]);

            Scrap.TB_Title.Text = data.title;
            Scrap.TB_Desc.Text = data.description;
            Scrap.TB_BaseURL.Text = data.host;

            MainWindow.SetClickObject(Scrap.Grid);
            Scrap.Grid.ToolTip = data.title;
            Scrap.Grid.MouseLeftButtonDown += (s, e) =>
            {
                System.Diagnostics.Process.Start(data.url);
                e.Handled = true;
            };
        }

        private static string[] SplitWithDelimiters(string input, List<string> delimiters)
        {
            if (delimiters.Count > 0)
            {
                string pattern = pattern = "\\b(" + string.Join("|", delimiters.Select(d => Regex.Escape(d))) + ")\\b";
                string[] result = Regex.Split(input, pattern);
                return result;
            }
            return null;
        }

        public static void RefreshContent(List<QuoteData> content_decorators, string content, TextBlock TB_Content)
        {
            TB_Content.Tag = new object[] { content_decorators, content };
            TB_Content.Inlines.Clear();
            if (content.Length == 0)
                TB_Content.Visibility = Visibility.Collapsed;
            else
                TB_Content.Visibility = Visibility.Visible;

            foreach (var decorator in content_decorators)
            {
                if (decorator.type.Equals("profile"))
                {
                    Bold content2 = new Bold(new Run(decorator.text));
                    MainWindow.SetClickObject(content2);
                    content2.MouseLeftButtonDown += (s, e) =>
                    {
                        TimeLineWindow tlw = new TimeLineWindow(decorator.id);
                        tlw.Show();
                        tlw.Focus();
                        e.Handled = true;
                    };
                    TB_Content.Inlines.Add(content2);
                }
                else if (decorator.type.Equals("text") || decorator.type.Equals("emoticon"))
                {
                    string text = decorator.text.Replace("\\n", "\n");
                    if(text.Contains("http://") || text.Contains("https://"))
                    {
                        int count = 0;
                        string[] splitted = SplitWithDelimiters(text, new List<string> { "http://", "https://" });
                        string lastDelimiter = null;
                        foreach(string splittedText in splitted)
                        {
                            count++;
                            if (splittedText.Equals("https://") || splittedText.Equals("http://"))
                                lastDelimiter = splittedText;
                            else if(lastDelimiter != null && splittedText.Length > 0)
                            {
                                int endPos = Math.Min(splittedText.LastIndexOf(" ") - 1, splittedText.LastIndexOf("\\n") - 1);
                                if(endPos < 0)
                                {
                                    endPos = splittedText.Length - 1;
                                    if (count == splitted.Length)
                                        endPos++;
                                }
                                string uriText = lastDelimiter + splittedText.Substring(0, endPos);
                                lastDelimiter = null;
                                var uriSpan = new Bold(new Underline(new Run(uriText)))
                                {
                                    Foreground = Brushes.Blue
                                };
                                MainWindow.SetClickObject(uriSpan);
                                uriSpan.PreviewMouseLeftButtonDown += (s, e) =>
                                {
                                    e.Handled = true;
                                    System.Diagnostics.Process.Start(uriText);
                                };
                                TB_Content.Inlines.Add(uriSpan);
                                TB_Content.Inlines.Add(new Run(splittedText.Substring(endPos)));
                            }
                            else
                            {
                                TB_Content.Inlines.Add(new Run(splittedText));
                            }
                        }
                    }

                    else
                    {
                        TB_Content.Inlines.Add(new Run(text));
                    }

                }
                else if (decorator.type.Equals("hashtag"))
                {
                    TB_Content.Inlines.Add(new Bold(new Run(decorator.text.Replace("\\n", "\n"))));
                }
            }
        }

        public static string GetStringFromQuoteData(List<QuoteData> datas, bool preserveQuote)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var data in datas)
            {
                if (preserveQuote)
                {
                    if (data.type.Equals("profile"))
                    {
                        sb.Append("{!{" + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) + "}!}");
                    }
                    else
                        sb.Append(data.text);
                }
                else
                    sb.Append(data.text);
            }
            return sb.ToString();
        }

        public static List<QuoteData> GetQuoteDataFromString(string text)
        {
            return GetQuoteDataFromString(text, false);
        }

        public static List<QuoteData> GetQuoteDataFromString(string text, bool escapeHashtag)
        {
            string[] fragmentBases = text.Split(new string[] { "{!{" }, StringSplitOptions.None);
            List<QuoteData> returnData = new List<QuoteData>();
            int count = 0;
            foreach (string fragmentBase in fragmentBases)
            {
                if (count % 2 == 0)
                {
                    string str = fragmentBase.Contains("}!}") ? fragmentBase.Split(new string[] { "}!}" }, StringSplitOptions.None)[1] : fragmentBase;
                    str = str.Replace("\\n", "\n");
                    str = str.Replace("\\r\\n", "\n");
                    str = str.Replace("\\\"", "\"");
                    if (str.Contains("#") && !escapeHashtag)
                    {
                        string[] rawStr = str.Split(new string[] { "#" }, StringSplitOptions.None);
                        if (rawStr[0].Length > 0)
                        {
                            returnData.Add(new QuoteData()
                            {
                                type = "text",
                                text = rawStr[0]
                            });
                        }
                        for (int i = 1; i < rawStr.Length; i++)
                        {
                            string strNow = rawStr[i];
                            str = str.Replace("\\n", "\n");
                            str = str.Replace("\\r\\n", "\n");
                            str = str.Replace("\r\n", "\n");
                            int splitCounter = Math.Min(strNow.IndexOf(" "), strNow.IndexOf("\n"));
                            if (splitCounter >= 0)
                            {
                                string hashTag = strNow.Substring(0, splitCounter);
                                string otherStr = strNow.Substring(splitCounter);
                                if (hashTag.Length > 0)
                                {
                                    returnData.Add(new QuoteData()
                                    {
                                        type = "hashtag",
                                        hashtag_type = "",
                                        hashtag_type_id = "",
                                        text = "#" + hashTag
                                    });
                                }
                                else
                                {
                                    returnData.Add(new QuoteData()
                                    {
                                        type = "text",
                                        text = "#"
                                    });
                                }
                                if (otherStr.Length > 0)
                                {
                                    returnData.Add(new QuoteData()
                                    {
                                        type = "text",
                                        text = otherStr
                                    });
                                }
                            }
                            else
                            {
                                returnData.Add(new QuoteData()
                                {
                                    type = "hashtag",
                                    hashtag_type = "",
                                    hashtag_type_id = "",
                                    text = "#" + strNow
                                });
                            }
                        }
                    }
                    else
                    {
                        QuoteData quoteData = new QuoteData()
                        {
                            type = "text",
                            text = str
                        };
                        returnData.Add(quoteData);
                    }
                    count++;
                }
                else
                {
                    string[] strs = fragmentBase.Split(new string[] { "}!}" }, StringSplitOptions.None);
                    string jsonStr = strs[0];
                    jsonStr = jsonStr.Replace("\\n", "\n");
                    jsonStr = jsonStr.Replace("\\\"", "\"");
                    QuoteData quoteData = JsonConvert.DeserializeObject<QuoteData>(jsonStr);
                    count++;
                    returnData.Add(quoteData);
                    if (strs.Length == 2)
                    {
                        QuoteData quoteData2 = new QuoteData()
                        {
                            type = "text",
                            text = strs[1].Replace("\\n", "\n").Replace("\\\"", "\"")
                        };
                        returnData.Add(quoteData2);
                        count++;
                    }
                }
            }
            return returnData;
        }
    }
}
