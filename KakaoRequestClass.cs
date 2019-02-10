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
        private const int NotificationRefreshTime = 2000;
        private static DateTime? LastMessageTime = null;
        public static bool notShowError = false;
        private const string OfflineStr = " (오프라인)";
        public static async Task<string> GetScrapData(string url)
        {
            string requestURI = "https://story.kakao.com/a/scraper?url=" + Uri.EscapeDataString(url);
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            return await GetResponseFromRequest(webRequest);
        }

        public static async Task<ProfileData.ProfileObject> GetProfileFeed(string id, string from, bool noActivity = false)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + (!noActivity ? "?with=activities" : "");
            if (from != null)
                requestURI += "&since=" + from;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            ProfileData.ProfileObject obj = JsonConvert.DeserializeObject<ProfileData.ProfileObject>(response);
            return obj;
        }

        public static async Task<ProfileData.ProfileObject> GetProfile(string id)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            ProfileData.ProfileObject obj = JsonConvert.DeserializeObject<ProfileData.ProfileObject>(response);
            return obj;
        }

        public static async Task<ProfileRelationshipData.ProfileRelationship> GetProfileRelationship(string id)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + "?profile_only=true";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            ProfileRelationshipData.ProfileRelationship obj = JsonConvert.DeserializeObject<ProfileRelationshipData.ProfileRelationship>(response);
            return obj;
        }

        public static async Task<TimeLineData.TimeLine> GetFeed(string from)
        {
            string requestURI = "https://story.kakao.com/a/feeds";
            if (from != null)
                requestURI += "?since=" + from;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<TimeLineData.TimeLine>(response);
        }

        public static async Task<BookmarkData.Bookmarks> GetBookmark(string id, string from)
        {
            string requestURI = "https://story.kakao.com/a/profiles/" + id + "/sections/bookmark";
            if (from != null)
                requestURI += $"?since={from}";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<BookmarkData.Bookmarks>(response);
        }

        public static async Task<string> GetFriendData()
        {
            string requestURI = "https://story.kakao.com/a/friends/";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            return await GetResponseFromRequest(webRequest);
        }

        private static HttpWebRequest GenerateDefaultProfile(string requestURI, string method = "GET")
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(requestURI);
            webRequest.Method = method.ToUpper();
            webRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com/"));

            webRequest.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            webRequest.Headers["X-Kakao-ApiLevel"] = "45";
            webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            webRequest.Headers["X-Kakao-VC"] = Guid.NewGuid().ToString().ToLower().Substring(0,20);
            webRequest.Headers["Cache-Control"] = "max-age=0";

            webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
            webRequest.Headers["Accept-Language"] = "ko";

            webRequest.Headers["DNT"] = "1";

            //webRequest.Headers["authority"] = "story.kakao.com";
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

        public static async Task SetActivityProfile(string id, string permission, bool enable_share, bool comment_all_writable, bool is_must_read)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id;

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");
            string postData = $"permission={permission}&enable_share={GetBoolString(enable_share)}&comment_all_writable={GetBoolString(comment_all_writable)}&is_must_read={GetBoolString(is_must_read)}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();
            await GetResponseFromRequest(webRequest);
        }

        public static async Task MutePost(string id, bool mute)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/mute_push";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, mute ? "POST" : "DELETE");
            string postData = $"push_mute={mute}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task<List<CommentData.Comment>> GetComment(string id, string since)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/comments?lpp=30&order=desc";
            if(since != null)
                requestURI += "&since=" + since;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<List<CommentData.Comment>>(response);
        }

        public static async Task<string> GetProfileData()
        {
            string requestURI = "https://story.kakao.com/a/settings/profile";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return response;
        }

        public static async Task<List<Actor>> GetSpecificFriends(string id)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/specific_friends";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<List<Actor>>(response);
        }

        public static async Task LikeComment(string FeedID, string commentID, bool isDelete)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/comments/" + commentID + "/likes";
            string method;
            if (isDelete == true)
                method = "DELETE";
            else
                method = "POST";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, method);
            await GetResponseFromRequest(webRequest);
        }

        public static async Task FriendRequest(string id, bool isDelete)
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

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");

            string postData = $"{key}={id}&has_profile=true";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task FriendAccept(string id, bool isDelete)
        {
            string requestURI;
            if (isDelete)
                requestURI = "https://story.kakao.com/a/invitations/ignore";
            else
                requestURI = "https://story.kakao.com/a/invitations/accept";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");

            string postData = $"inviter_id={id}&has_profile=true";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task FavoriteRequest(string id, bool isUnpin)
        {
            string requestURI = "https://story.kakao.com/a/friends/" + id + "/favorite";
            string method;
            if (isUnpin != true)
                method = "POST";
            else
                method = "DELETE";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, method);
            webRequest.Method = method;
            await GetResponseFromRequest(webRequest);
        }

        public static async Task PinPost(string id, bool isUnpin)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id + "/bookmark";
            string method;
            if (isUnpin != true)
                method = "POST";
            else
                method = "DELETE";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, method);
            await GetResponseFromRequest(webRequest);
        }

        public async static Task ShareFeed(string FeedID, string text, string permission, bool commentable, List<string> with_ids, List<string> trust_ids)
        {
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\n");
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/share";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");

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

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public async static Task UPFeed(string FeedID, bool isDelete)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/sympathy";
            string method;
            if (isDelete)
                method = "DELETE";
            else
                method = "POST";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, method);
            await GetResponseFromRequest(webRequest);
        }

        public async static Task<bool> LikeFeed(string FeedID, string emotion)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/like";
            string method;
            if (emotion == null)
                method = "DELETE";
            else
                method = "POST";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, method);

            string postData;
            if (emotion == null)
                postData = "";
            else
                postData = "emotion=" + emotion;

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            return await GetResponseFromRequest(webRequest) != null;
        }

        public static async Task DeleteFriend(string id)
        {
            string requestURI = "https://story.kakao.com/a/friends/" + id;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");
            await GetResponseFromRequest(webRequest);
        }

        public static async Task DeleteLike(string FeedID, string id)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/like";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");

            string postData = $"id={id}";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task DeleteBirthday()
        {
            string requestURI = "https://story.kakao.com/a/agreement/birth";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");
            await GetResponseFromRequest(webRequest);
        }
        public static async Task SetProfileName(string name)
        {
            string requestURI = "https://story.kakao.com/a/settings/profile/name";
            string postData = $"name={Uri.EscapeDataString(name)}";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }
        public static async Task SetBirthday(DateTime date, bool isLunar, bool isLeapType)
        {
            string requestURI = "https://story.kakao.com/a/settings/profile/birthday";
            string postData = $"birth={Uri.EscapeDataString(date.ToString("yyyyMMdd"))}&birth_type={Uri.EscapeDataString(isLeapType == true ? "-" : "+")}&birth_leap_type={isLeapType.ToString().ToLower()}";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }
        public static async Task SetGender(string gender, string permission)
        {
            string requestURI = "https://story.kakao.com/a/settings/profile/gender";
            string postData = $"gender={gender}&permission={permission}";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }
        public static async Task DeleteGender()
        {
            string requestURI = "https://story.kakao.com/a/settings/profile/gender";
            string postData = $"gender=&permission=";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }
        public static async Task SetStatusMessage(string message)
        {
            string requestURI = "https://story.kakao.com/a/settings/profile/status_message";
            string postData = $"status_message={Uri.EscapeDataString(message)}";
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }
        public static async Task SendMail(string content, string id, bool bomb, string imgURI = null)
        {
            string requestURI = "https://story.kakao.com/a/messages?_="+((long) DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds - 32400).ToString()+ "11149";
            string objectStr = $"&object=%7B%22background%22%3A%7B%22type%22%3A%22color%22%2C%22value%22%3A{new Random().Next(10983816, 10983816).ToString()}%7D%7D";

            if (imgURI != null)
                objectStr = "";

            string postData = $"content={Uri.EscapeDataString("[{\"type\":\"text\",\"text\":\""+content+"\"}]")}&bomb={bomb.ToString().ToLower()}" + objectStr + $"&receiver_id%5B%5D={id}&reference_id=";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");
            webRequest.Headers["Origin"] = "https://story.kakao.com";
            webRequest.Headers["Cache-Control"] = "no-cache";
            webRequest.Referer = "https://story.kakao.com/";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task<List<MailData.Mail>> GetMails(string since = null)
        {
            string requestURI = "https://story.kakao.com/a/messages/";
            if (since != null)
                requestURI += $"?since={since}";

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "GET");

            return JsonConvert.DeserializeObject<List<MailData.Mail>>(await GetResponseFromRequest(webRequest)); ;
        }

        public static async Task<MailData.MailDetail> GetMailDetail(string id)
        {
            string requestURI = "https://story.kakao.com/a/messages/" + id;

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "GET");

            return JsonConvert.DeserializeObject<MailData.MailDetail>(await GetResponseFromRequest(webRequest)); ;
        }
        public static async Task DeleteMail(string id)
        {
            string requestURI = "https://story.kakao.com/a/messages/" + id;

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");

            await GetResponseFromRequest(webRequest);
        }

        public async static Task ReplyToFeed(string FeedID, string text, string id, string name, UploadedImageProp img = null)
        {
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\n");
            string requestURI = "https://story.kakao.com/a/activities/" + FeedID + "/comments";

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


            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "POST");

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public async void GetNotification(object sender, EventArgs e1)
        {
            await RequestNotification(false);
        }

        public static async Task<List<CSNotification>> RequestNotification(bool isReturn)
        {
            if (MainWindow.IsLoggedIn)
            {
                string requestURI = "https://story.kakao.com/a/notifications";

                HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
                webRequest.Timeout = NotificationRefreshTime;

                DateTime? lastTime = null;
                try
                {
                    string response = await GetResponseFromRequest(webRequest);
                    List<CSNotification> obj = JsonConvert.DeserializeObject<List<CSNotification>>(response);
                    if (isReturn) return obj;
                    int countTemp = 0;

                    if(obj.Count > 0)
                        lastTime = obj[0].created_at;

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
                                if (Environment.OSVersion.Version.Major == 10)
                                    GlobalHelper.ShowNotification(Message, Content, Uri, obj[count].comment_id, obj[count].actor?.id, obj[count].actor?.display_name, Profile, Identity, obj[count].thumbnail_url ?? obj[count].actor?.profile_thumbnail_url);
                                else
                                    GlobalHelper.ShowNotification(Message, Content, Uri);
                            }
                            try
                            {
                                string activityID = Uri.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                                if (MainWindow.Posts.ContainsKey(activityID))
                                {
                                    MainWindow.Posts[activityID].Refresh();
                                    MainWindow.Posts[activityID].SV_Comment.ScrollToEnd();
                                }
                            }
                            catch (Exception) {}
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (MainWindow.IsOffline)
                    {
                        if (!Properties.Settings.Default.Disable_Message)
                            GlobalHelper.ShowNotification("안내", "인터넷 연결이 복구됐습니다.", null);
                        MainWindow.Instance.Title = MainWindow.Instance.Title.Replace(OfflineStr, "");
                        MainWindow.IsOffline = false;
                    }
                }
                catch (Exception)
                {
                    if (!MainWindow.IsOffline && Environment.OSVersion.Version.Major == 10)
                    {
                        MainWindow.Instance.Title = MainWindow.Instance.Title.Replace(OfflineStr, "") + OfflineStr;
                        MainWindow.IsOffline = true;
                        if (!Properties.Settings.Default.Disable_Message)
                            GlobalHelper.ShowNotification("일반 오류", "인터넷 연결에 문제가 발생했습니다. 자동으로 복구를 시도합니다.", null);
                    }
                }
                finally
                {
                    if (lastTime != null)
                        LastMessageTime = lastTime;
                }

            }
            if (isReturn == false)
            {
                await Task.Delay(NotificationRefreshTime);
                await RequestNotification(isReturn);
            }
            return null;
        }

        public static async Task DeletePost(string id)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + id;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");
            await GetResponseFromRequest(webRequest);
        }

        public static async Task DeleteComment(string commentID, PostData data)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/comments/" + commentID;
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "DELETE");
            await GetResponseFromRequest(webRequest);
        }

        public static async Task EditComment(Comment comment, string text, PostData data)
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

            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI, "PUT");

            Stream writeStream = await webRequest.GetRequestStreamAsync();
            writeStream.Write(byteArray, 0, byteArray.Length);
            writeStream.Close();

            await GetResponseFromRequest(webRequest);
        }

        public static async Task<List<ShareData.Share>> GetShares(bool isUP, PostData data, string from)
        {

            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/shares/";
            if (isUP)
                requestURI = "https://story.kakao.com/a/activities/" + data.id + "/sympathies/";

            if (from != null)
                requestURI += $"?since={from}";
            
            HttpWebRequest webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<List<ShareData.Share>>(response);
        }
        public async static Task<string> GetResponseFromRequest(WebRequest webRequest, int count = 0)
        {
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
                if (count < 5)
                    return await GetResponseFromRequest(webRequest, ++count);
                else
                {
                    if(!notShowError)
                        MessageBox.Show(new StreamReader(e.Response.GetResponseStream()).ReadToEnd(), e.Message);
                    return null;
                }
            }
        }

        public static async Task<List<ShareData.Share>> GetLikes(PostData data, string from)
        {
            string requestURI = "https://story.kakao.com/a/activities/" + data.id + "/likes/";
            if(from != null)
                requestURI += "?since=" + from;
            var webRequest = GenerateDefaultProfile(requestURI);
            string response = await GetResponseFromRequest(webRequest);
            return JsonConvert.DeserializeObject<List<ShareData.Share>>(response);
        }
    }
}
