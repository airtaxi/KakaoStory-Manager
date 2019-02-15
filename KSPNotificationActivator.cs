// The GUID CLSID must be unique to your app. Create a new GUID if copying this code.
using DesktopNotifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using static KSP_WPF.CommentData;

namespace KSP_WPF
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("0c6af8c8-51d6-4e07-9c45-C173E6ADF0C3"), ComVisible(true)]
    public class KSPNotificationActivator : NotificationActivator
    {
        public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
        {
            ActivateHandler(invokedArgs, userInput);
        }

        public static void ActivateHandler(string invokedArgs, NotificationUserInput userInput)
        {
            Application.Current.Dispatcher.Invoke(async delegate
            {
                if (!invokedArgs.Contains("default null string"))
                {
                    try
                    {
                        if (invokedArgs.Contains("LIKE!@#$%"))
                        {
                            string text = invokedArgs.Substring(0, invokedArgs.IndexOf("LIKE!@#$%"));
                            string activityID = text.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                            string commentID = invokedArgs.Split(new string[] { "LIKE!@#$%" }, StringSplitOptions.None)[1];
                            await KakaoRequestClass.LikeComment(activityID, commentID, false);
                        }
                        else if (invokedArgs.Contains("REPLY!@#$%"))
                        {
                            string text = invokedArgs.Substring(0, invokedArgs.IndexOf("REPLY!@#$%"));
                            string[] datas = invokedArgs.Split(new string[] { "R!@=!!" }, StringSplitOptions.None);
                            string id = datas[1];
                            string name = datas[2];
                            string writer = datas[3];
                            string identity = datas[4];
                            string msg = userInput["tbReply"];
                            string activityID = text.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                            await KakaoRequestClass.ReplyToFeed(activityID, msg, id, name);
                        }
                        else
                        {
                            //MessageBox.Show(invokedArgs);
                            string text = invokedArgs.Split(new string[] { "!" }, StringSplitOptions.None)[1];
                            string activityID = text.Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                            string url = invokedArgs.Split(new string[] { "!" }, StringSplitOptions.None)[0];
                            try
                            {
                                PostData data = await KakaoRequestClass.GetPost(activityID);
                                if (data != null)
                                {
                                    await MainWindow.Instance.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                                    {
                                        PostWindow.ShowPostWindow(data, activityID);
                                    }));
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("접근할 수 없는 포스트입니다.");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            string id = invokedArgs.Replace("https://story.kakao.com/", "");
                            if (id.Length > 0)
                            {
                                TimeLineWindow tlw = new TimeLineWindow(id);
                                tlw.Show();
                                tlw.Activate();
                            }
                            else
                                throw new InvalidDataException();
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Process.Start(invokedArgs.Split(new string[] { "!" }, StringSplitOptions.None)[0]);
                        }
                    }
                }
            });
        }
    }
}