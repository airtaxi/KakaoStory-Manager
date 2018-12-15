using DesktopNotifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Windows.Media;
using XamlAnimatedGif;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using Newtonsoft.Json;
using System.Windows.Input;

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
            ShowNotification(title, message, URL, null, null, null, null, null, null);
        }

        public static void ShowNotification(string title, string message, string URL, string commentID, string id, string name, string writer, string identity, string thumbnailURL)
        {
            try
            {
                var Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children = {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                Text = message
                            }
                        }
                    }
                };
                if (thumbnailURL != null)
                {
                    Visual.BindingGeneric.HeroImage = new ToastGenericHeroImage()
                    {
                        Source = thumbnailURL,
                    };
                }
                ToastActionsCustom Action;
                if (URL == null)
                {
                    Action = new ToastActionsCustom();
                }
                else
                {
                    if (commentID != null)
                    {
                        Action = new ToastActionsCustom()
                        {
                            Inputs = {
                                new ToastTextBox("tbReply")
                                {
                                    PlaceholderContent = "답장 작성하기",
                                },
                            },
                            Buttons =
                            {
                                new ToastButton("보내기", URL + "REPLY!@#$%" + "R!@=!!" + id + "R!@=!!" + name + "R!@=!!" + writer + "R!@=!!" + identity)
                                {
                                    ActivationType = ToastActivationType.Background,
                                    TextBoxId = "tbReply"
                                },
                                new ToastButton("좋아요", URL + "LIKE!@#$%" + commentID),
                                new ToastButton("열기", URL)
                            },
                        };
                    }
                    else
                    {
                        Action = new ToastActionsCustom()
                        {
                            Buttons =
                            {
                                new ToastButton("열기", URL)
                            },
                        };
                    }
                }
                var toastContent = new ToastContent()
                {
                    Visual = Visual,
                    Actions = Action,
                };
                var toastXml = new XmlDocument();
                toastXml.LoadXml(toastContent.GetContent());
                var toast = new ToastNotification(toastXml);
                DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
            }
            catch (Exception) { }
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
        
        public static async void AssignImage(dynamic image, string uri)
        {
            if (image == null) return;
            if (uri != null && uri.Length > 0)
            {
                string hash = GetHash(uri);
                string path = System.IO.Path.Combine(defaultPath, hash + ".proftemp");
                bool isGIF = uri.Contains(".gif");
                if (File.Exists(path))
                {
                    if (image is Image)
                        SetImage(image as Image, path, uri, isGIF);
                    else if (image is Ellipse)
                        SetImage(image as Ellipse, path, uri, isGIF);
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
                    TB_Content.Inlines.Add(new Run(decorator.text.Replace("\\n", "\n")));
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
