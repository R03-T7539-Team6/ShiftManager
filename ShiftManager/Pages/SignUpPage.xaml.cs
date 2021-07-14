using System;
using System.Windows;
using System.Windows.Controls;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for SignUpPage.xaml
  /// </summary>
  public partial class SignUpPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    public SignUpPage()
    {
      InitializeComponent();
      USC.SavePushed += OnSavePushed;
    }

    /*******************************************
    * specification ;
    * name = OnSavePushed ;
    * Function = 保存ボタンが押された時に入力された情報をサーバに送る ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 保存ボタンが押されたことを知らせるイベントハンドラ ;
    * output = N/A ;
    * end of specification ;
    *******************************************/
    private async void OnSavePushed(object sender, EventArgs e){
      //TODO: Saveボタンが押されたときの処理を書く.
      if (string.IsNullOrWhiteSpace(USC.UserIDText))
      {
        MessageBox.Show("IDが入力されていません");
      }
      else if (string.IsNullOrWhiteSpace(USC.FirstNameText))
      {
        MessageBox.Show("名前が入力されていません");
      }
      else if (string.IsNullOrWhiteSpace(USC.LastNameText))
      {
        MessageBox.Show("苗字が入力されていません");
      }
      else if (string.IsNullOrWhiteSpace(USC.PasswordText))
      {
        MessageBox.Show("パスワードが入力されていません");
      }
      else
      {
        UserID userID = new(USC.UserIDText);
        string rawPassword = USC.PasswordText;
        string Salt = "eiofjrueahrgheirugha";//Base64
        int StretchCount = 33;//適当な数字
        HashedPassword Hash = HashedPasswordGetter.Get(rawPassword, new HashedPassword(string.Empty, Salt, StretchCount));
        WorkLog wl = new(userID, new());
        UserData userData = new(userID, Hash, new NameData(USC.FirstNameText, USC.LastNameText), ApiHolder.CurrentStoreID, USC.SelectedUserGroup, USC.SelectedUserState, wl, new UserSetting(userID, NotificationPublishTimings.None, new()));
        await ApiHolder.Api.SignUpAsync(userData);
      }
    }
  }
}
