using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using DesktopNotifications;
using Microsoft.Win32;
using System.Threading.Tasks;
using MessageBox = System.Windows.MessageBox;
using System.Text;
using Microsoft.QueryStringDotNET;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace KSP_WPF
{
    public partial class MainWindow : MetroWindow
    {
        public static bool IsLoggedIn = false;
        public static MainWindow instance;
        public static string APP_ID = "kck4156.KSP.WPF";

        public bool isClose = false;
        public static bool notificationRequested = true;
        public static bool isOffline = false;
        private static DateTime? LastMessage = null;
        private readonly NotifyIcon _notifyIcon = null;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string OfflineStr = " (오프라인)";
        public static StoryWriteWindow storyWriteWindow = null;
        public static TimeLineWindow timeLineWindow = null;
        public static TimeLineWindow profileTimeLineWindow = null;
        public static NotificationsWindow notificationsWindow = null;
        public static SettingsWindow settingsWindow = null;
        public static FriendSelectWindow friendListWindow = null;
        public static Dictionary<string, PostWindow> posts = new Dictionary<string, PostWindow>();
        public static FriendInitData.FriendData FriendData;

        public MainWindow()
        {
            InitializeComponent();


            CB_AutoLogin.IsChecked = Properties.Settings.Default.AutoLogin;

            Environment.CurrentDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"),
                Visible = true
            };
            _notifyIcon.MouseDoubleClick += (s, e) =>
            {
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Normal;
                else
                    Show();
                
                Activate();
            };

            ContextMenu menu = new ContextMenu();;
            MenuItem timeline = new MenuItem
            {
                Index = 0,
                Text = "타임라인"
            };
            timeline.Click += (s, a) =>
            {
                BT_TimeLine_Click(null, null);
            };
            menu.MenuItems.Add(timeline);

            MenuItem write = new MenuItem
            {
                Index = 0,
                Text = "게시글 작성"
            };
            write.Click += (s, a) =>
            {
                BT_Write_Click(null, null);
            };
            menu.MenuItems.Add(write);

            MenuItem notification = new MenuItem
            {
                Index = 0,
                Text = "알림 확인"
            };
            notification.Click += (s, a) =>
            {
                BT_Notifiations_Click(null, null);
            };
            menu.MenuItems.Add(notification);

            MenuItem profile = new MenuItem
            {
                Index = 0,
                Text = "내 프로필"
            };
            profile.Click += (s, a) =>
            {
                BT_MyProfile_Click(null, null);
            };
            menu.MenuItems.Add(profile);

            MenuItem settings = new MenuItem
            {
                Index = 0,
                Text = "설정"
            };
            settings.Click += (s, a) =>
            {
                BT_Settings_Click(null, null);
            };
            menu.MenuItems.Add(settings);

            MenuItem exit = new MenuItem
            {
                Index = 0,
                Text = "종료"
            };
            exit.Click += (s, a) =>
            {
                isClose = true;
                Environment.Exit(0);
            };
            menu.MenuItems.Add(exit);

            _notifyIcon.ContextMenu = menu;

            if (Properties.Settings.Default.AutoLogin)
            {
                TBX_Email.Text = Properties.Settings.Default.AutoEmail;
                TBX_Password.Password = Properties.Settings.Default.AutoPassword;
                Dispatcher.InvokeAsync(() =>
                {
                    TryLogin();   
                });
            }

            SetClickObject(GD_Friends);
            SetClickObject(GD_Write);
            SetClickObject(GD_Timeline);
            SetClickObject(GD_Notifications);
            SetClickObject(GD_Settings);
            SetClickObject(GD_Friends);
            SetClickObject(BT_Login);
            SetClickObject(EL_Profile);
            SetClickObject(TB_MyProfile);
            SetClickObject(TB_Tray);
            SetClickObject(IMG_Power);

            Dispatcher.Invoke(async() =>
            {
                await RequestNotification(false);
            });
        }

        public static void ClickEventMouseMove(object s, System.Windows.Input.MouseEventArgs e)
        {
            if(((IInputElement) s).IsEnabled && Mouse.OverrideCursor != System.Windows.Input.Cursors.Hand)
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
        }
        public static void ClickEventMouseLeave(object s, System.Windows.Input.MouseEventArgs e)
        {
            if (((IInputElement)s).IsEnabled)
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
        public static void SetClickObject(IInputElement element)
        {
            element.MouseMove += ClickEventMouseMove;
            element.MouseLeave += ClickEventMouseLeave;
        }

        public static async Task<bool> LikeComment(string FeedID, string commentID, bool isDelete)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/comments/" + commentID + "/likes";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);

            if (isDelete == true)
                request.Method = "DELETE";
            else
                request.Method = "POST";

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
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            return true;
        }

        public static async Task<bool> FriendRequest(string id, bool isDelete)
        {
            string requestURI;
            string key;
            if (isDelete == true)
            {
                requestURI = "https://story.kakao.com/a/invitations/cancel";
                key = "user_id";
            }
            else
            {
                requestURI = "https://story.kakao.com/a/invitations";
                key = "friend_id";
            }

            string postData = $"{key}={id}&has_profile=true";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";

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

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public static async Task<bool> FriendAccept(string id, bool isDelete)
        {
            string requestURI;
            if (isDelete != true)
                requestURI = "https://story.kakao.com/a/invitations/accept";
            else
                requestURI = "https://story.kakao.com/a/invitations/ignore";

            string postData = $"inviter_id={id}&has_profile=true";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";

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

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public static async Task<bool> FavoriteRequest(string id, bool isUnpin)
        {
            string requestURI = "https://story.kakao.com/a/friends/"+id+"/favorite";
            string method;
            if (isUnpin != true)
                method = "POST";
            else
                method = "DELETE";
            
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = method;

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

            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public static async Task<bool> DeleteFriend(string id)
        {
            string requestURI = "https://story.kakao.com/a/friends/" + id;

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "DELETE";

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

            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public async static Task<bool> ReplyToFeed(string FeedID, string text, string id, string name)
        {
            return await ReplyToFeed(FeedID, text, id, name, null);
        }
        public async static Task<bool> ReplyToFeed(string FeedID, string text, string id, string name, UploadedImageProp img)
        {
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\n");

            if(id != null && name != null)
                text = text.Insert(0, "{!{{" + "\"id\":\"" + id + "\", \"type\":\"profile\", \"text\":\"" + name + "\"}}!} ");

            List<QuoteData> rawContent = StoryWriteWindow.GetQuoteDataFromString(text, true);
            string textContent = Uri.EscapeDataString(JsonConvert.SerializeObject(rawContent).Replace("\"id\":null,", ""));

            string postData;
            string imageData = "";
            string imageData2 = "";
            string textNow = StoryWriteWindow.GetStringFromQuoteData(rawContent, false);

            if (img != null)
            {
                imageData2 = "(Image) ";
                imageData = "{\"media_path\":\"" + img.access_key + "/" + img.info.original.filename + "?width=" + img.info.original.width + "&height=" + img.info.original.height + "&avg=" + img.info.original.avg + "\",\"type\":\"image\",\"text\":\"(Image) \"},";
                textContent = textContent.Insert(3, Uri.EscapeDataString(imageData));
            }
            
            postData = "text=" + Uri.EscapeDataString(imageData2 + textNow) + "&decorators=" + textContent;

            postData = postData.Replace("%20", "+");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);


            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/comments";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
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

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public async static Task<bool> ShareFeed(string FeedID, string text, string permission, bool commentable, List<string> with_ids, List<string> trust_ids)
        {
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\n");

            List<QuoteData> rawContent = StoryWriteWindow.GetQuoteDataFromString(text);
            string textContent = Uri.EscapeDataString(JsonConvert.SerializeObject(rawContent).Replace("\"id\":null,", ""));

            string postData = "content=" + textContent
                + "&permission=" + permission + "&comment_all_writable=" + (commentable ? "true" : "false")
                + "&is_must_read=false&enable_share=true";

            if (with_ids.Count > 0)
                postData += "&with_tags=" + Uri.EscapeDataString(JsonConvert.SerializeObject(with_ids));
            if (trust_ids.Count > 0)
                postData += "&allowed_profile_ids=" + Uri.EscapeDataString(JsonConvert.SerializeObject(trust_ids));

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);


            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/share";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
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

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public async static Task<bool> UPFeed(string FeedID, bool isDelete)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/sympathy";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            if (isDelete)
                request.Method = "DELETE";
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

            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public async static Task<bool> LikeFeed(string FeedID, string emotion)
        {
            string postData;
            if (emotion == null)
                postData = "";
            else
                postData = "emotion=" + emotion;

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/like";
            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            if (emotion == null)
                request.Method = "DELETE";
            else
                request.Method = "POST";
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

            Stream writeStream = await request.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            try
            {
                var readStream = await request.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                respReader.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public async void GetNotification(object sender, EventArgs e1)
        {
            await RequestNotification(false);
            return;
        }

        public static async Task<List<CSNotification>> RequestNotification(bool isReturn)
        {
            if (IsLoggedIn)
            {
                string RequestURI = "https://story.kakao.com/a/notifications";

                HttpWebRequest webRequest = WebRequest.CreateHttp(RequestURI);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json; charset=utf-8";

                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

                webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
                webRequest.Headers["X-Kakao-ApiLevel"] = "45";
                webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
                webRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
                webRequest.Headers["Cache-Control"] = "max-age=0";

                webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
                webRequest.Headers["Accept-Language"] = "ko";

                webRequest.Headers["DNT"] = "1";

                webRequest.Headers["authority"] = "story.kakao.com";
                webRequest.Referer = "https://story.kakao.com/";
                webRequest.KeepAlive = true;
                webRequest.UseDefaultCredentials = true;
                webRequest.Host = "story.kakao.com";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
                webRequest.Accept = "application/json";
                webRequest.Timeout = 2000;

                webRequest.AutomaticDecompression = DecompressionMethods.GZip;
                webRequest.Date = DateTime.Now;
                try
                {
                    var ReadStream = await webRequest.GetResponseAsync();
                    var RespReader = ReadStream.GetResponseStream();
                    string RespResult = await new StreamReader(RespReader).ReadToEndAsync();
                    RespReader.Close();
                    List<CSNotification> obj = JsonConvert.DeserializeObject<List<CSNotification>>(RespResult);
                    if (isReturn) return obj;
                    int countTemp = 0;
                    for (int count = 0; count < obj.Count; count++)
                    {
                        countTemp = count;
                        DateTime CurrentMessage = obj[count].created_at;

                        if (LastMessage != null && ToUnixTime(CurrentMessage) > ToUnixTime((DateTime)LastMessage))
                        {
                            string Message = obj[count].message;
                            string Content = obj[count].content ?? "내용 없음";

                            string Profile = null;
                            string Identity = null;
                            string Uri = "https://story.kakao.com/";
                            if (obj[count].scheme != null && (obj[count].scheme.Contains("?profile_id=")))
                            {
                                var ObjStr = obj[count].scheme?.Split(new string[] { "?profile_id=" }, StringSplitOptions.None);
                                Profile = ObjStr[1];
                                Identity = ObjStr[0].Split('.')[1];
                                Uri += Profile + "/" + Identity + "!" + ObjStr[0];
                            }
                            if (obj[count].scheme != null && (obj[count].scheme.Contains("kakaostory://profiles/")))
                            {
                                Uri += obj[count].scheme.Replace("kakaostory://profiles/", "");
                            }
                            bool willShow = true;

                            if (Properties.Settings.Default.Disable_Like && obj[count].emotion != null)
                                willShow = false;
                            else if (Properties.Settings.Default.Disable_VIP && obj[count].decorators != null && obj[count].decorators[0] != null && obj[count].decorators[0].text != null
                                && obj[count].decorators[0].text.StartsWith("관심친구"))
                                willShow = false;

                            if (willShow && obj[count].is_new)
                            {
                                ShowNotification(Message, Content, Uri, obj[count].comment_id, obj[count].actor?.id, obj[count].actor?.display_name, Profile, Identity, obj[count].thumbnail_url ?? obj[count].actor?.profile_thumbnail_url);
                            }
                            try
                            {
                                string activityID = Uri.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                                if (posts.ContainsKey(activityID))
                                {
                                    posts[activityID].Refresh();
                                    posts[activityID].SV_Comment.ScrollToEnd();
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (isOffline)
                    {
                        if (!Properties.Settings.Default.Disable_Message)
                            ShowNotification("안내", "인터넷 연결이 복구됐습니다.", null);
                        instance.Title = instance.Title.Replace(OfflineStr, "");
                        isOffline = false;
                    }
                    if (obj.Count > 0)
                        LastMessage = obj[0].created_at;
                }
                catch (Exception)
                {
                    if (!isOffline)
                    {
                        instance.Title = instance.Title.Replace(OfflineStr, "") + OfflineStr;
                        isOffline = true;
                        if (!Properties.Settings.Default.Disable_Message)
                            ShowNotification("일반 오류", "인터넷 연결에 문제가 발생했습니다. 자동으로 복구를 시도합니다.", null);
                    }
                }
            }
            if (isReturn == false)
            {
                await Task.Delay(2000);
                await RequestNotification(isReturn);
            }
            return null;
        }

        public static void SaveGIFImage(System.Windows.Controls.Image image)
        {
            string uri = (string) image.Tag;
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
            ImageViewerWindow imageViewer = new ImageViewerWindow();
            imageViewer.Show();
            imageViewer.Activate();
            imageViewer.Focus();
            imageViewer.IMG_Main.Source = ((System.Windows.Controls.Image)source).Source;
            e.Handled = true;
        }

        public static void CopyImageHandler(object source, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Controls.Image img = (System.Windows.Controls.Image)source;
                System.Windows.Clipboard.SetImage(img.Source as BitmapImage);
                MessageBox.Show("클립보드에 이미지가 복사됐습니다");
            }
            catch (Exception)
            {
                System.Windows.Controls.Image image = (System.Windows.Controls.Image)source;
                if (image.Tag is string uri)
                    SaveGIFImage(image);
            }
            if(e != null)
                e.Handled = true;
        }

        public static void SaveImageToFile(System.Windows.Controls.Image image)
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

        public static async Task<ProfileData.ProfileObject> GetProfileFeed(string id, string from)
        {
            string RequestURI = "https://story.kakao.com/a/profiles/" + id + "?with=activities";
            if (from != null)
            {
                RequestURI += "&since=" + from;
            }
            HttpWebRequest webRequest = WebRequest.CreateHttp(RequestURI);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            webRequest.Headers["X-Kakao-ApiLevel"] = "45";
            webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            webRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            webRequest.Headers["Cache-Control"] = "max-age=0";

            webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            webRequest.Headers["Accept-Language"] = "ko";

            webRequest.Headers["DNT"] = "1";

            webRequest.Headers["authority"] = "story.kakao.com";
            webRequest.Referer = "https://story.kakao.com/";
            webRequest.KeepAlive = true;
            webRequest.UseDefaultCredentials = true;
            webRequest.Host = "story.kakao.com";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            webRequest.Accept = "application/json";

            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.Date = DateTime.Now;

            var ReadStream = await webRequest.GetResponseAsync();
            var RespReader = ReadStream.GetResponseStream();
            string RespResult = await new StreamReader(RespReader).ReadToEndAsync();
            RespReader.Close();
            ProfileData.ProfileObject obj = JsonConvert.DeserializeObject<ProfileData.ProfileObject>(RespResult);
            return obj;
        }

        public static async Task<ProfileRelationshipData.ProfileRelationship> GetProfileRelationship(string id)
        {
            string RequestURI = "https://story.kakao.com/a/profiles/" + id + "?profile_only=true";
            HttpWebRequest webRequest = WebRequest.CreateHttp(RequestURI);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            webRequest.Headers["X-Kakao-ApiLevel"] = "45";
            webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            webRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            webRequest.Headers["Cache-Control"] = "max-age=0";

            webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            webRequest.Headers["Accept-Language"] = "ko";

            webRequest.Headers["DNT"] = "1";

            webRequest.Headers["authority"] = "story.kakao.com";
            webRequest.Referer = "https://story.kakao.com/";
            webRequest.KeepAlive = true;
            webRequest.UseDefaultCredentials = true;
            webRequest.Host = "story.kakao.com";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            webRequest.Accept = "application/json";

            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.Date = DateTime.Now;

            var ReadStream = await webRequest.GetResponseAsync();
            var RespReader = ReadStream.GetResponseStream();
            string RespResult = await new StreamReader(RespReader).ReadToEndAsync();
            RespReader.Close();
            ProfileRelationshipData.ProfileRelationship obj = JsonConvert.DeserializeObject<ProfileRelationshipData.ProfileRelationship>(RespResult);
            return obj;
        }
        public static async Task<TimeLineData.TimeLine> GetFeed(string from)
        {
            string RequestURI = "https://story.kakao.com/a/feeds";

            if (from != null)
            {
                RequestURI += "?since=" + from;
            }

            HttpWebRequest webRequest = WebRequest.CreateHttp(RequestURI);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            webRequest.Headers["X-Kakao-ApiLevel"] = "45";
            webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            webRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            webRequest.Headers["Cache-Control"] = "max-age=0";

            webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            webRequest.Headers["Accept-Language"] = "ko";

            webRequest.Headers["DNT"] = "1";

            webRequest.Headers["authority"] = "story.kakao.com";
            webRequest.Referer = "https://story.kakao.com/";
            webRequest.KeepAlive = true;
            webRequest.UseDefaultCredentials = true;
            webRequest.Host = "story.kakao.com";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            webRequest.Accept = "application/json";

            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.Date = DateTime.Now;
            try
            {
                var ReadStream = await webRequest.GetResponseAsync();
                var RespReader = ReadStream.GetResponseStream();
                string RespResult = await new StreamReader(RespReader).ReadToEndAsync();
                RespReader.Close();
                TimeLineData.TimeLine obj = JsonConvert.DeserializeObject<TimeLineData.TimeLine>(RespResult);
                return obj;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
        }

        public static async Task<string> GetScrapData(string url)
        {
            string RequestURI = "https://story.kakao.com/a/scraper?url=" + Uri.EscapeDataString(url);

            HttpWebRequest webRequest = WebRequest.CreateHttp(RequestURI);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            webRequest.Headers["X-Kakao-ApiLevel"] = "45";
            webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            webRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            webRequest.Headers["Cache-Control"] = "max-age=0";

            webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            webRequest.Headers["Accept-Language"] = "ko";

            webRequest.Headers["DNT"] = "1";

            webRequest.Headers["authority"] = "story.kakao.com";
            webRequest.Referer = "https://story.kakao.com/";
            webRequest.KeepAlive = true;
            webRequest.UseDefaultCredentials = true;
            webRequest.Host = "story.kakao.com";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            webRequest.Accept = "application/json";

            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            webRequest.Date = DateTime.Now;
            try
            {
                var ReadStream = await webRequest.GetResponseAsync();
                var RespReader = ReadStream.GetResponseStream();
                string RespResult = await new StreamReader(RespReader).ReadToEndAsync();
                RespReader.Close();
                return RespResult;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                return null;
            }
        }

        private static long ToUnixTime(DateTime date)
        {
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClose)
            {
                e.Cancel = true;
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Minimized;
                else
                    Hide();
                if (!Properties.Settings.Default.Disable_Message)
                    ShowNotification("안내", "프로그램이 최소화됐습니다.\r\n시스템 트레이의 프로그램 아이콘을 더블클릭하여 창을 복구할 수 있습니다.", null);
            }
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

        public void StartTimer()
        {
            Dispatcher.InvokeAsync(async() =>
            {
                IsLoggedIn = true;
                await Task.Delay(1);
                if (!Properties.Settings.Default.Disable_Message)
                    ShowNotification("안내", "프로그램이 최소화됐습니다.\r\n시스템 트레이의 프로그램 아이콘을 클릭하여 창을 복구할 수 있습니다.", null);

                FriendData = JsonConvert.DeserializeObject<FriendInitData.FriendData>(WebViewWindow.rawDataNow.Replace("\\x", ""));
                await Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                {
                    GD_Login.Visibility = Visibility.Collapsed;
                    GD_Profile.Visibility = Visibility.Visible;
                    TB_Name.Text = FriendData.profile.display_name;
                    TB_Email.Text = TBX_Email.Text;
                    TB_Login.Visibility = Visibility.Collapsed;
                    TB_LoginProgress.Visibility = Visibility.Collapsed;
                    TB_Logout.Visibility = Visibility.Visible;
                    IMG_Login.Visibility = Visibility.Collapsed;
                    EL_Profile.Fill = new ImageBrush(new BitmapImage(new Uri(FriendData.profile.profile_image_url)));
                    BT_Login.IsEnabled = true;
                }));
                if(Properties.Settings.Default.AutoMinimize)
                {
                    TB_Tray_MouseLeftButtonDown(null, null);
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BT_Login.IsEnabled)
            {
                if (!IsLoggedIn)
                {
                    TryLogin();
                }
                else
                {
                    GD_Login.Visibility = Visibility.Visible;
                    GD_Profile.Visibility = Visibility.Collapsed;
                    TB_Login.Visibility = Visibility.Visible;
                    IMG_Login.Visibility = Visibility.Visible;
                    TB_LoginProgress.Visibility = Visibility.Collapsed;
                    TB_Logout.Visibility = Visibility.Collapsed;
                    BT_Login.IsEnabled = true;
                    IsLoggedIn = false;
                }
            }
        }

        private void TryLogin()
        {
            if (BT_Login.IsEnabled)
            {
                TB_Login.Visibility = Visibility.Collapsed;
                TB_LoginProgress.Visibility = Visibility.Visible;
                TB_Logout.Visibility = Visibility.Collapsed;
                IMG_Login.Visibility = Visibility.Collapsed;
                BT_Login.IsEnabled = false;
                instance = this;
                if (Properties.Settings.Default.AutoLogin)
                {
                    Properties.Settings.Default.AutoEmail = TBX_Email.Text;
                    Properties.Settings.Default.AutoPassword = TBX_Password.Password;
                    Properties.Settings.Default.Save();
                }
                
                try
                {
                    WebViewWindow webViewWindow = new WebViewWindow();
                    webViewWindow.Show();
                    webViewWindow.Hide();
                }
                catch (Exception e)
                {
                    TB_Login.Text = "로그인";
                    BT_Login.IsEnabled = true;
                    MessageBox.Show("로그인 에러가 발생했습니다.\n"+ e.StackTrace);
                }
            }
        }

        private void BT_Write_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn && !isOffline)
                ShowOfflineMessage();
            else
            {
                if (storyWriteWindow == null)
                {
                    storyWriteWindow = new StoryWriteWindow();
                    storyWriteWindow.Show();
                    storyWriteWindow.Activate();
                }
                else
                {
                    storyWriteWindow.Show();
                    storyWriteWindow.Activate();
                }
            }
        }

        private async void BT_TimeLine_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !isOffline)
            {
                if (timeLineWindow == null)
                {
                    timeLineWindow = new TimeLineWindow
                    {
                        fromMainMenu = true
                    };
                    timeLineWindow.Show();
                    timeLineWindow.Activate();
                }
                else
                {
                    await timeLineWindow.RefreshTimeline(null, true);
                    timeLineWindow.Show();
                    timeLineWindow.Activate();
                }
            }
            else
                ShowOfflineMessage();
        }

        public void BT_Notifiations_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !isOffline)
            {
                if (notificationsWindow == null)
                {
                    notificationsWindow = new NotificationsWindow();
                    notificationsWindow.Show();
                    notificationsWindow.Activate();
                }
                else
                {
                    notificationsWindow.Refresh();
                    notificationsWindow.Show();
                    notificationsWindow.Activate();
                }
            }
            else
                ShowOfflineMessage();
        }
        
        public static void ShowOfflineMessage()
        {
            MessageBox.Show("로그인상태가 아니거나 오프라인 상태입니다.");
        }

        private async void BT_MyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !isOffline)
            {
                if(profileTimeLineWindow == null)
                {
                    profileTimeLineWindow = new TimeLineWindow(FriendData.profile.id)
                    {
                        fromMainMenu = true
                    };
                    profileTimeLineWindow.Show();
                    profileTimeLineWindow.Activate();
                }
                else
                {
                    await profileTimeLineWindow.RefreshTimeline(null, true);
                    profileTimeLineWindow.Show();
                    profileTimeLineWindow.Activate();
                }
            }
            else
                ShowOfflineMessage();
        }

        private void BT_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Show();
                settingsWindow.Activate();
            }
            else
            {
                settingsWindow.Show();
                settingsWindow.Activate();
            }
        }

        private void CB_AutoLogin_Click(object sender, RoutedEventArgs e)
        {
            bool check = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            Properties.Settings.Default.AutoLogin = check;
        }

        private void BT_Friends_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !isOffline)
            {
                if (friendListWindow == null)
                {
                    friendListWindow = new FriendSelectWindow(null, true);
                    friendListWindow.BT_Submit.Visibility = Visibility.Collapsed;
                    friendListWindow.RD_Submit.Height = new GridLength(0);
                    friendListWindow.Show();
                    friendListWindow.Activate();
                }
                else
                {
                    friendListWindow.Show();
                    friendListWindow.Activate();
                }
            }
            else
                ShowOfflineMessage();
        }

        private void TB_Tray_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.DefaultMinimize)
                WindowState = WindowState.Minimized;
            else
                Hide();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isClose = true;
            Environment.Exit(0);
        }

        private void TB_MyProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_MyProfile_Click(null, null);
        }

        private void GD_Friends_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Friends_Click(null, null);
        }

        private void BT_Login_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(null, null);
        }

        private void GD_Write_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Write_Click(null, null);
        }

        private void GD_Timeline_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_TimeLine_Click(null, null);
        }

        private void GD_Notifications_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Notifiations_Click(null, null);
        }

        private void GD_Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Settings_Click(null, null);
        }

        private void MetroWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Minimized;
                else
                    Hide();
                e.Handled = true;
            }
        }

        private void TBX_Email_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Return)
                TBX_Password.Focus();
        }

        private void TBX_Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Button_Click(null, null);
        }

        private void BT_Memory_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.Collect(2);
            GC.WaitForFullGCComplete();
            GC.Collect();
            GC.Collect(2);
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox) && !(e.Source is System.Windows.Controls.PasswordBox))
                e.Handled = true;
        }
    }
}
