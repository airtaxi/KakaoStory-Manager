using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static KSP_WPF.CommentData;

namespace KSP_WPF
{
    class KakaoRequestClass
    {
        private static DateTime? LastMessageTime = null;
        private const string OfflineStr = " (오프라인)";

        public static async Task<string> GetScrapData(string url)
        {
            string requestURI = "https://story.kakao.com/a/scraper?url=" + Uri.EscapeDataString(url);

            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                return respResult;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                return null;
            }
        }

        public static async Task<ProfileData.ProfileObject> GetProfileFeed(string id, string from)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + "?with=activities";
            if (from != null)
            {
                requestURI += "&since=" + from;
            }
            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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

            var readStream = await webRequest.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();
            ProfileData.ProfileObject obj = JsonConvert.DeserializeObject<ProfileData.ProfileObject>(respResult);
            return obj;
        }

        public static async Task<ProfileRelationshipData.ProfileRelationship> GetProfileRelationship(string id)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + "?profile_only=true";
            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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

            var readStream = await webRequest.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();
            ProfileRelationshipData.ProfileRelationship obj = JsonConvert.DeserializeObject<ProfileRelationshipData.ProfileRelationship>(respResult);
            return obj;
        }

        public static async Task<TimeLineData.TimeLine> GetFeed(string from)
        {
            string requestURI = "https://story.kakao.com/a/feeds";

            if (from != null)
            {
                requestURI += "?since=" + from;
            }

            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                TimeLineData.TimeLine obj = JsonConvert.DeserializeObject<TimeLineData.TimeLine>(respResult);
                return obj;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
        }

        public static async Task<BookmarkData.Bookmarks> GetBookmark(string id, string from)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + "/sections/bookmark";

            if (from != null)
                requestURI += $"?since={from}";
            
            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                BookmarkData.Bookmarks obj = JsonConvert.DeserializeObject<BookmarkData.Bookmarks>(respResult);
                return obj;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
        }

        public static async Task<string> GetFriendData()
        {
            string requestURI = "https://story.kakao.com/a/friends/";

            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                return respResult;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
        }

        private static HttpWebRequest GenerateDefaultProfile(string requestURI, string method)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
            webRequest.Method = method.ToUpper();
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

            return webRequest;
        }

        private static string GetBoolString(bool src)
        {
            return src ? "true" : "false";
        }

        public static async Task<bool> SetActivityProfile(string id, string permission, bool enable_share, bool comment_all_writable, bool is_must_read)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id;

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");
            string postData = $"permission={permission}&enable_share={GetBoolString(enable_share)}&comment_all_writable={GetBoolString(comment_all_writable)}&is_must_read={GetBoolString(is_must_read)}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            try
            {
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                return true;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
        }

        public static async Task<bool> MutePost(string id, bool mute)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/mute_push";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, mute ? "POST" : "DELETE");
            string postData = $"push_mute={mute}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            try
            {
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                return true;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
        }

        public static async Task<List<CommentData.Comment>> GetComment(string id, string since)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/comments?lpp=30&order=desc";
            if(since != null)
            {
                requestURI += "&since=" + since;
            }
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "GET");
            try
            {
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                List<CommentData.Comment> returnVar = JsonConvert.DeserializeObject<List<CommentData.Comment>>(respResult);
                return returnVar;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp, "DOGE");
            }
            return null;
        }

        public static async Task<string> GetProfileData()
        {
            string requestURI = "https://story.kakao.com/a/settings/profile";

            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                return respResult;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
        }

        public static async Task<List<Actor>> GetSpecificFriends(string id)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/specific_friends";
            MessageBox.Show(requestURI);

            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                var readStream = await webRequest.GetResponseAsync();
                var respReader = readStream.GetResponseStream();
                string respResult = await new StreamReader(respReader).ReadToEndAsync();
                respReader.Close();
                readStream.Close();
                List<Actor> obj = JsonConvert.DeserializeObject<List<Actor>>(respResult);
                return obj;
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return null;
            }
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
            readStream.Close();
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
                readStream.Close();
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
                readStream.Close();
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
            string requestURI = "https://story.kakao.com/a/friends/" + id + "/favorite";
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
                readStream.Close();
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
                return false;
            }
            return true;
        }

        public static async Task<bool> PinPost(string id, bool isUnpin)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/bookmark";
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
                readStream.Close();
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

            List<QuoteData> rawContent = GlobalHelper.GetQuoteDataFromString(text);
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
                readStream.Close();
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
                readStream.Close();
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
                readStream.Close();
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
                readStream.Close();
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

            if (id != null && name != null)
                text = text.Insert(0, "{!{{" + "\"id\":\"" + id + "\", \"type\":\"profile\", \"text\":\"" + name + "\"}}!} ");

            List<QuoteData> rawContent = GlobalHelper.GetQuoteDataFromString(text, true);
            string textContent = Uri.EscapeDataString(JsonConvert.SerializeObject(rawContent).Replace("\"id\":null,", ""));

            string postData;
            string imageData = "";
            string imageData2 = "";
            string textNow = GlobalHelper.GetStringFromQuoteData(rawContent, false);

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
                readStream.Close();
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
            if (MainWindow.IsLoggedIn)
            {
                string requestURI = "https://story.kakao.com/a/notifications";

                HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
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
                    var readStream = await webRequest.GetResponseAsync();
                    var respReader = readStream.GetResponseStream();
                    string respResult = await new StreamReader(respReader).ReadToEndAsync();
                    respReader.Close();
                    readStream.Close();
                    List<CSNotification> obj = JsonConvert.DeserializeObject<List<CSNotification>>(respResult);
                    if (isReturn) return obj;
                    int countTemp = 0;
                    for (int count = 0; count < obj.Count; count++)
                    {
                        countTemp = count;
                        DateTime CurrentMessage = obj[count].created_at;

                        if (LastMessageTime != null && GlobalHelper.ToUnixTime(CurrentMessage) > GlobalHelper.ToUnixTime((DateTime)LastMessageTime))
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
                                GlobalHelper.ShowNotification(Message, Content, Uri, obj[count].comment_id, obj[count].actor?.id, obj[count].actor?.display_name, Profile, Identity, obj[count].thumbnail_url ?? obj[count].actor?.profile_thumbnail_url);
                            }
                            try
                            {
                                string activityID = Uri.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                                if (MainWindow.posts.ContainsKey(activityID))
                                {
                                    MainWindow.posts[activityID].Refresh();
                                    MainWindow.posts[activityID].SV_Comment.ScrollToEnd();
                                }
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (MainWindow.isOffline)
                    {
                        if (!Properties.Settings.Default.Disable_Message)
                            GlobalHelper.ShowNotification("안내", "인터넷 연결이 복구됐습니다.", null);
                        MainWindow.instance.Title = MainWindow.instance.Title.Replace(OfflineStr, "");
                        MainWindow.isOffline = false;
                    }
                    if (obj.Count > 0)
                        LastMessageTime = obj[0].created_at;
                }
                catch (Exception)
                {
                    if (!MainWindow.isOffline)
                    {
                        MainWindow.instance.Title = MainWindow.instance.Title.Replace(OfflineStr, "") + OfflineStr;
                        MainWindow.isOffline = true;
                        if (!Properties.Settings.Default.Disable_Message)
                            GlobalHelper.ShowNotification("일반 오류", "인터넷 연결에 문제가 발생했습니다. 자동으로 복구를 시도합니다.", null);
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

        public static async void DeletePost(string id)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id;

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = new CookieContainer();
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

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();
        }

        public static async Task<bool> DeleteComment(string commentID, PostData data)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/comments/" + commentID;

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = new CookieContainer();
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

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();

            return true;
        }

        public static async Task<bool> EditComment(Comment comment, string text, PostData data)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/comments/" + comment.id + "/content";

            List<QuoteData> rawContent = GlobalHelper.GetQuoteDataFromString(text);
            string textContent = Uri.EscapeDataString(JsonConvert.SerializeObject(rawContent).Replace("\"id\":null,", ""));

            string imageData = "";
            string imageData2 = "";
            foreach (QuoteData qdata in comment.decorators)
            {
                if (qdata.media_path != null)
                {
                    imageData2 = "(Image) ";
                    imageData = JsonConvert.SerializeObject(qdata, Formatting.None, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    textContent = textContent.Insert(3, Uri.EscapeDataString(imageData));
                }
            }
            string postData = "text=" + Uri.EscapeDataString(imageData2 + GlobalHelper.GetStringFromQuoteData(rawContent, false));
            postData += "&decorators=" + textContent;

            postData = postData.Replace("%20", "+");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = new CookieContainer();
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
            writeStream.Close();

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();

            return true;
        }

        public static async Task<List<ShareData.Share>> GetShares(bool isUP, PostData data)
        {

            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/shares";
            if (isUP)
                requestURI = "https://story.kakao.com/a/activities/" + data.id + "/sympathies";
            HttpWebRequest NotificationGetRequest = WebRequest.CreateHttp(requestURI);
            NotificationGetRequest.Method = "GET";
            NotificationGetRequest.ContentType = "application/json; charset=utf-8";

            NotificationGetRequest.CookieContainer = new CookieContainer();
            NotificationGetRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com"));

            NotificationGetRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            NotificationGetRequest.Headers["X-Kakao-ApiLevel"] = "45";
            NotificationGetRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            NotificationGetRequest.Headers["X-Kakao-VC"] = "185412afe1da9580e67f";
            NotificationGetRequest.Headers["Cache-Control"] = "max-age=0";

            NotificationGetRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            NotificationGetRequest.Headers["Accept-Language"] = "ko";

            NotificationGetRequest.Headers["DNT"] = "1";

            NotificationGetRequest.Headers["authority"] = "story.kakao.com";
            NotificationGetRequest.Referer = "https://story.kakao.com";
            NotificationGetRequest.KeepAlive = true;
            NotificationGetRequest.UseDefaultCredentials = true;
            NotificationGetRequest.Host = "story.kakao.com";
            NotificationGetRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            NotificationGetRequest.Accept = "application/json";

            NotificationGetRequest.AutomaticDecompression = DecompressionMethods.GZip;
            NotificationGetRequest.Date = DateTime.Now;

            var readStream = await NotificationGetRequest.GetResponseAsync();
            var respReader = readStream.GetResponseStream();
            string respResult = await new StreamReader(respReader).ReadToEndAsync();
            respReader.Close();
            readStream.Close();
            return JsonConvert.DeserializeObject<List<ShareData.Share>>(respResult);
        }
    }
}
