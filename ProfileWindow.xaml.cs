using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace KSP_WPF
{
    /// <summary>
    /// MailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProfileWindow : MetroWindow
    {
        private bool isGenderExists;
        private bool isBirthdayExists;
        private bool isDescExists;
        private readonly SolidColorBrush ButtonActivatedBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102));
        private readonly SolidColorBrush ButtonUnActivatedBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        public ProfileWindow()
        {
            InitializeComponent();
            //if (!Properties.Settings.Default.HideScrollBar)
                //SV_Main.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            RC_Mask.Visibility = Visibility.Visible;
            PR_Mask.Visibility = Visibility.Visible;
            Dispatcher.InvokeAsync(async () =>
            {
                await MainWindow.UpdateProfile();
                RC_Mask.Visibility = Visibility.Collapsed;
                PR_Mask.Visibility = Visibility.Collapsed;

                TB_Name.Text = MainWindow.UserProfile.display_name;
                TBX_Name.Text = MainWindow.UserProfile.display_name;
                TB_Desc.Text = MainWindow.UserProfile.status_objects?[0]?.message ?? "한줄 소개 없음";
                TBX_Desc.Text = MainWindow.UserProfile.status_objects?[0]?.message ?? "";
                if (MainWindow.UserProfile.status_objects?[0]?.message == null)
                {
                    isDescExists = false;
                    IC_DescEdit.Foreground = ButtonUnActivatedBrush;
                    IC_DescDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                }
                else
                {
                    isDescExists = true;
                    IC_DescEdit.Foreground = ButtonActivatedBrush;
                    IC_DescDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                    MainWindow.SetClickObject(IC_DescEdit);
                }

                string uri = MainWindow.UserProfile.profile_video_url_square_small ?? MainWindow.UserProfile.profile_thumbnail_url;
                GlobalHelper.AssignImage(IMG_Profile, uri);
                GlobalHelper.AssignImage(IMG_ProfileBG, MainWindow.UserProfile.bg_image_url);

                MainWindow.SetClickObject(EL_ProfileEdit);
                MainWindow.SetClickObject(IC_BGEdit);
                MainWindow.SetClickObject(IC_NameEdit);
                MainWindow.SetClickObject(IC_DescDelete);
                MainWindow.SetClickObject(IC_BirthdayDelete);
                MainWindow.SetClickObject(IC_SexDelete);

                if (MainWindow.UserProfile.birth_type == null)
                {
                    isBirthdayExists = false;
                    IC_BirthdayDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                    CB_SunDate.IsEnabled = false;
                    DP_Birthday.IsEnabled = false;
                }
                else
                {
                    isBirthdayExists = true;
                    IC_BirthdayDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                    CB_SunDate.IsChecked = MainWindow.UserProfile.birth_type.Equals("+");
                    CB_LeapType.IsChecked = MainWindow.UserProfile.birth_leap_type;
                    DP_Birthday.SelectedDate = DateTime.ParseExact(MainWindow.UserProfile.birth, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }

                if(MainWindow.UserProfile.gender_permission.Length > 0)
                {
                    isGenderExists = true;

                    CB_PublicSex.IsChecked = MainWindow.UserProfile.gender_permission.Equals("A");

                    if(MainWindow.UserProfile.gender.Equals("M"))
                        RB_Male.IsChecked = true;
                    else if(MainWindow.UserProfile.gender.Equals("F"))
                        RB_Female.IsChecked = true;
                    else
                        RB_Alien.IsChecked = true;

                    IC_SexDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                }
                else
                {
                    isGenderExists = false;
                    RB_Male.IsEnabled = false;
                    RB_Female.IsEnabled = false;
                    RB_Alien.IsEnabled = false;
                    CB_PublicSex.IsEnabled = false;
                    IC_SexDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                }
                DP_Birthday.SelectedDateChanged += DP_Birthday_SelectedDateChanged;
            });
        }

        private string GetCurrentGenderString()
        {
            string currentGenderString = null;
            if (isGenderExists)
            {
                if (RB_Male.IsChecked == true)
                    currentGenderString = "M";
                else if (RB_Female.IsChecked == true)
                    currentGenderString = "F";
                else if (RB_Alien.IsChecked == true)
                    currentGenderString =  "";
            }
            return currentGenderString;
        }

        private bool IsBirthdayChanged()
        {
            bool isChanged = false;
            //생일이 있으나 삭제돼 프로필이 변경됨
            if (MainWindow.UserProfile.birth_type != null && !isBirthdayExists)
                isChanged = true;
            //생일이 없으나 새로 생겨 프로필이 변경됨
            else if (MainWindow.UserProfile.birth_type == null && isBirthdayExists)
                isChanged = true;
            //생일이 변경되어 프로필이 변경됨
            else if (isBirthdayExists && !MainWindow.UserProfile.birth.Equals(((DateTime)DP_Birthday.SelectedDate).ToString("yyyyMMdd")))
                isChanged = true;
            //생일 양/음력이 변경되어 프로필이 변경됨
            else if (isBirthdayExists && !MainWindow.UserProfile.birth_type.Equals(CB_SunDate.IsChecked == true ? "+" : "-"))
                isChanged = true;
            //생일 윤달이 변경되어 프로필이 변경됨
            else if (isBirthdayExists && !MainWindow.UserProfile.birth_leap_type.Equals(CB_LeapType.IsChecked == true))
                isChanged = true;
            return isChanged;
        }
        private bool IsNameChanged()
        {
            bool isChanged = false;

            //이름이 변경되어 프로필이 변경됨
            if (!MainWindow.UserProfile.display_name.Equals(TBX_Name.Text))
                isChanged = true;

            return isChanged;
        }
        private bool IsStatusMessageChanged()
        {
            bool isChanged = false;

            //한줄소개가 있으나 삭제돼 프로필이 변경됨
            if (MainWindow.UserProfile.status_objects?[0]?.message != null && !isDescExists)
                isChanged = true;
            //한줄소개가 없으나 새로 생겨 프로필이 변경됨
            else if (MainWindow.UserProfile.status_objects?[0]?.message == null && isDescExists)
                isChanged = true;
            //한줄소개가 변경되어 프로필이 변경됨
            else if (isDescExists && MainWindow.UserProfile.status_objects?[0]?.message != null && !TBX_Desc.Text.Equals(MainWindow.UserProfile.status_objects?[0]?.message))
                isChanged = true;

            return isChanged;
        }
        private bool IsGenderChanged()
        {
            bool isChanged = false;

            //성별이 있으나 삭제돼 프로필이 변경됨
            if (MainWindow.UserProfile.gender != null && !isGenderExists)
                isChanged = true;
            //성별이 없으나 새로 생겨 프로필이 변경됨
            else if (MainWindow.UserProfile.gender == null && isGenderExists)
                isChanged = true;
            //성별이 변경되어 프로필이 변경됨
            else if (isGenderExists && MainWindow.UserProfile.gender != null && !MainWindow.UserProfile.gender.Equals(GetCurrentGenderString()))
                isChanged = true;
            //성별 공개 범위가 변경되어 프로필이 변경됨
            else if (isGenderExists && MainWindow.UserProfile.gender_permission != null && !MainWindow.UserProfile.gender_permission.Equals(CB_PublicSex.IsChecked == true ? "A" : "F"))
                isChanged = true;

            return isChanged;
        }
        private bool ValidateProfileChange()
        {
            bool isChanged = false;

            if(IsNameChanged())
                isChanged = true;
            else if(IsStatusMessageChanged())
                isChanged = true;
            else if(IsBirthdayChanged())
                isChanged = true;
            else if(IsGenderChanged())
                isChanged = true;
            
            if (isChanged)
                BT_Submit.IsEnabled = true;
            else
                BT_Submit.IsEnabled = false;

            return isChanged;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.ProfileWindow = null;
        }

        private async void BT_Submit_Click(object sender, RoutedEventArgs e)
        {
            BT_Submit.IsEnabled = false;
            BT_Submit.Content = "업로드중...";

            if (IsNameChanged())
            {
                await KakaoRequestClass.SetProfileName(TBX_Name.Text);
                MainWindow.Instance.TB_Name.Text = TBX_Name.Text;
            }

            if (IsStatusMessageChanged())
                await KakaoRequestClass.SetStatusMessage(TBX_Desc.Text);

            if (IsBirthdayChanged())
            {
                if (!isBirthdayExists)
                    await KakaoRequestClass.DeleteBirthday();
                else
                    await KakaoRequestClass.SetBirthday((DateTime) DP_Birthday.SelectedDate, CB_SunDate.IsChecked == false, CB_LeapType.IsChecked == true);
            }
            if (IsGenderChanged())
            {
                if (!isGenderExists)
                    await KakaoRequestClass.DeleteGender();
                else
                    await KakaoRequestClass.SetGender(GetCurrentGenderString(), CB_PublicSex.IsChecked == true ? "A" : "F");
            }

            string profileRawData = await KakaoRequestClass.GetProfileData();
            MainWindow.UserProfile = JsonConvert.DeserializeObject<UserProfile.ProfileData>(profileRawData);
            Close();
        }

        private void CB_SunDate_Click(object sender, RoutedEventArgs e)
        {
            ValidateProfileChange();
        }

        private void CB_PublicSex_Click(object sender, RoutedEventArgs e)
        {
            ValidateProfileChange();
        }

        private void RB_Sex_Click(object sender, RoutedEventArgs e)
        {
            ValidateProfileChange();
        }

        private void EL_ProfileEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void IC_NameEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(IC_NameEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.Edit)
            {
                IC_NameEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                {
                    TBX_Name.IsEnabled = true;
                    TBX_Name.SelectionStart = TBX_Name.Text.Length;
                    TBX_Name.SelectionLength = 0;
                    TBX_Name.Focus();
                }));
            }
            else
            {
                if (TBX_Name.Text.Length <= 10)
                {
                    IC_NameEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                    TBX_Name.IsEnabled = false;
                }
                else
                    MessageBox.Show("이름은 10자 이내로 입력해주세요.", "오류");
            }
            ValidateProfileChange();
        }

        private void IC_DescDelete_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //한줄소개 삭제가 가능한경우
            if (IC_DescDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.Delete 
                && IC_DescEdit.Kind != MaterialDesignThemes.Wpf.PackIconKind.Check)
            {
                isDescExists = false;
                MainWindow.UnSetClickObject(IC_DescEdit);
                IC_DescEdit.Foreground = ButtonUnActivatedBrush;
                IC_DescDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
                TBX_Desc.Text = "";
            }
            else if(IC_DescEdit.Kind != MaterialDesignThemes.Wpf.PackIconKind.Check)
            {
                isDescExists = true;
                MainWindow.SetClickObject(IC_DescEdit);
                IC_DescEdit.Foreground = ButtonActivatedBrush;
                IC_DescDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                IC_DescEdit_PreviewMouseLeftButtonDown(IC_DescEdit, null);
            }
            ValidateProfileChange();
        }

        private void IC_DescEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(IC_DescDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.Delete)
            {
                if(IC_DescEdit.Kind == MaterialDesignThemes.Wpf.PackIconKind.Edit)
                {
                    IC_DescEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                    {
                        TBX_Desc.IsEnabled = true;
                        TBX_Desc.SelectionStart = TBX_Desc.Text.Length;
                        TBX_Desc.SelectionLength = 0;
                        TBX_Desc.Focus();
                    }));
                    IC_DescDelete.Foreground = ButtonUnActivatedBrush;
                    MainWindow.UnSetClickObject(IC_DescDelete);
                }
                else
                {
                    if(TBX_Desc.Text.Length > 20)
                    {
                        MessageBox.Show("한줄소개를 20자 이내로 작성해주세요.", "오류");
                    }
                    else if(TBX_Desc.Text.Length > 0)
                    {
                        IC_DescEdit.Kind = MaterialDesignThemes.Wpf.PackIconKind.Edit;
                        TBX_Desc.IsEnabled = false;
                        IC_DescDelete.Foreground = ButtonActivatedBrush;
                        MainWindow.SetClickObject(IC_DescDelete);
                        ValidateProfileChange();
                    }
                    else if(TBX_Desc.Text.Length == 0)
                    {
                        MessageBox.Show("한줄소개를 입력해주세요.", "오류");
                    }
                }
            }
            ValidateProfileChange();
        }

        private void IC_BirthdayDelete_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(IC_BirthdayDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.Delete)
            {
                isBirthdayExists = false;
                DP_Birthday.IsEnabled = false;
                CB_SunDate.IsEnabled = false;
                IC_BirthdayDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
            }
            else
            {
                isBirthdayExists = true;
                DP_Birthday.IsEnabled = true;
                CB_SunDate.IsEnabled = true;
                IC_BirthdayDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
            }
            ValidateProfileChange();
        }

        private void DP_Birthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateProfileChange();
        }

        private void IC_SexDelete_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IC_SexDelete.Kind == MaterialDesignThemes.Wpf.PackIconKind.Delete)
            {
                isGenderExists = false;
                RB_Male.IsEnabled = false;
                RB_Female.IsEnabled = false;
                RB_Alien.IsEnabled = false;
                CB_PublicSex.IsEnabled = false;
                IC_SexDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add;
            }
            else
            {
                isGenderExists = true;
                RB_Male.IsEnabled = true;
                RB_Female.IsEnabled = true;
                RB_Alien.IsEnabled = true;
                CB_PublicSex.IsEnabled = true;
                IC_SexDelete.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
            }
            ValidateProfileChange();
        }

        private void CB_LeapType_Click(object sender, RoutedEventArgs e)
        {
            ValidateProfileChange();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
