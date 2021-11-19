using MahApps.Metro.Controls;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using MessageBox = System.Windows.Forms.MessageBox;

namespace KSP_WPF
{
    /// <summary>
    /// WebViewWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WebViewWindow : MetroWindow
    {
        public static string rawDataNow;
        public static CookieContainer cookieContainer;
        public static string cookieString;
        public WebViewWindow()
        {
            InitializeComponent();
            WebBrowserView.Loaded += (s, e) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        WebBrowserView.Navigate("https://story.kakao.com/s/logout");
                        WebBrowserView.LoadCompleted += OnLoad;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("로그인에 실패하였습니다.\n인터넷 연결을 확인해주세요.");
                        Close();
                    }
                });
            };
        }

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
         string url,
         string cookieName,
         StringBuilder cookieData,
         ref int size,
         Int32 dwFlags,
         IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        /// <summary>
        /// Gets the URI cookie container.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            return cookieContainer;
        }
        public static CookieContainer GetUriCookieContainerForOnce(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 32;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookieString = cookieData.ToString().Replace(';', ',');
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        bool IsLoggedIn = false;
        bool isLoginSuccess = false;

        private void OnLoad(object sender, NavigationEventArgs e)
        {
            Dispatcher.InvokeAsync(async() =>
            {

                int INTERNET_COOKIE_HTTPONLY = 0x00002000;
                StringBuilder cookie = new StringBuilder();
                int size = 256;

                InternetGetCookieEx("https://story.kakao.com/", "_karmt", cookie, ref size,
                            INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
                if (cookie.ToString().Contains("_karmt"))
                {
                    cookieContainer = GetUriCookieContainerForOnce(new Uri("https://story.kakao.com/"));
                    isLoginSuccess = true;
                    MainWindow.Instance.StartTimer();
                    Dispose();
                    Close();
                }
                else if (!IsLoggedIn)
                {
                    try
                    {
                        WebBrowserView.InvokeScript("eval", new string[] { $"document.getElementById(\"id_email_2\").value = \"{MainWindow.Instance.TBX_Email.Text}\";" });
                        WebBrowserView.InvokeScript("eval", new string[] { $"document.getElementById(\"id_password_3\").value = \"{MainWindow.Instance.TBX_Password.Password}\";" });
                        WebBrowserView.InvokeScript("eval", new string[] { "document.getElementsByClassName(\"ico_account ico_check\")[0].click();" });
                        WebBrowserView.InvokeScript("eval", new string[] { "document.getElementsByClassName(\"btn_g btn_confirm submit\")[0].click();" });

                        await Task.Delay(7000);
                        if (!isLoginSuccess)
                        {
                            MessageBox.Show("로그인 실패");
                            Show();
                        }
                        IsLoggedIn = true;
                    }
                    catch (Exception) { };
                }
            });
        }

        private void RevertLogin()
        {
            MainWindow.Instance.BT_Login.IsEnabled = true;
        }

        public void Dispose()
        {
            WebBrowserView.Dispose();
            WebBrowserView = null;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (!isLoginSuccess)
            {
                RevertLogin();
            }
        }
    }
}
