using MahApps.Metro.Controls;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public WebViewWindow()
        {
            InitializeComponent();
            WebBrwoserView.Loaded += (s, e) =>
            {
                try
                {
                    WebBrwoserView.Navigate("https://story.kakao.com/s/logout");
                    WebBrwoserView.LoadCompleted += OnLoad;
                }
                catch (Exception)
                {
                    MessageBox.Show("로그인에 실패하였습니다.\n인터넷 연결을 확인해주세요.");
                    Close();
                }
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
        private CookieContainer GetUriCookieContainerForOnce(Uri uri)
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
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        bool IsLoggedIn = false;
        bool isLoginSuccess = false;

        private async void OnLoad(object sender, NavigationEventArgs e)
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
                MainWindow.instance.StartTimer();
                mshtml.HTMLDocument document = WebBrwoserView.Document as mshtml.HTMLDocument;
                string html = document.documentElement.innerHTML;
                int indexStart = html.IndexOf(@"boot.parseInitialData(") + @"boot.parseInitialData(".Length;
                int indexEnd = html.IndexOf(@");</script>", indexStart);
                try
                {
                    string rawData = html.Substring(indexStart, indexEnd - indexStart);
                    rawDataNow = rawData;
                }
                finally
                {
                    Dispose();
                    Close();
                }
            }
            else if(!IsLoggedIn)
            {
                try
                {
                    (WebBrwoserView.Document as mshtml.HTMLDocument).getElementById("loginEmail").setAttribute("value", MainWindow.instance.TBX_Email.Text);
                    (WebBrwoserView.Document as mshtml.HTMLDocument).getElementById("loginPw").setAttribute("value", MainWindow.instance.TBX_Password.Password);
                    (WebBrwoserView.Document as mshtml.HTMLDocument).getElementById("staySignedIn").setAttribute("value", "true");
                    mshtml.IHTMLElementCollection buttons = (WebBrwoserView.Document as mshtml.IHTMLDocument3).getElementsByTagName("button");
                    foreach (mshtml.IHTMLElement button in buttons)
                    {
                        if (((string)button.getAttribute("type")).Contains("submit"))
                        {
                            button.click();
                            await Task.Delay(7000);
                            if (!isLoginSuccess)
                            {
                                MessageBox.Show("로그인 실패");
                                Show();
                            }
                            break;
                        }
                    }
                    IsLoggedIn = true;
                }
                catch (Exception) {};
            }
        }

        private void RevertLogin()
        {
            MainWindow.instance.BT_Login.IsEnabled = true;
        }

        public void Dispose()
        {
            WebBrwoserView.Dispose();
            WebBrwoserView = null;
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
