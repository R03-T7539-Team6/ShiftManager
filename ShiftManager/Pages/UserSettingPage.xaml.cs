using System;
using System.Windows;
using System.Windows.Controls;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for UserSettingPage.xaml
  /// </summary>
  public partial class UserSettingPage : Page, IContainsApiHolder
  {
    public event EventHandler UpdateUserDataSuccessed;
    public IApiHolder ApiHolder { get; set; }
    private UserData userData;
    public UserSettingPage() => InitializeComponent();

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      ApiResult<UserData> data = await ApiHolder.Api.GetUserDataByIDAsync(ApiHolder.CurrentUserID);

      if (data.IsSuccess && data.ReturnData is not null)
      {
        userData = data.ReturnData;
        usc.SetData(userData);
      }
      else
        _ = MessageBox.Show("データ取得に失敗しました\nErrorCode:" + data.ResultCode.ToString(), "ShiftManager");
    }

    private async void usc_SavePushed(object sender, System.EventArgs e)
    {
      UserData data = userData with
      {
        FullName = new NameData(usc.FirstNameText, usc.LastNameText),
        UserGroup = usc.SelectedUserGroup,
        UserState = usc.SelectedUserState,
      };

      if (!string.IsNullOrWhiteSpace(usc.PasswordText))
      {
        HashedPassword hpw;
        var _hpw = await ApiHolder.Api.GetPasswordHashingDataAsync(new UserID(usc.UserIDText));

        if (!_hpw.IsSuccess || _hpw.ReturnData is null)
        {
          _ = MessageBox.Show("データ取得に失敗しました\nErrorCode:" + _hpw.ResultCode.ToString(), "ShiftManager");
          return;
        }

        hpw = HashedPasswordGetter.Get(usc.PasswordText, _hpw.ReturnData);
        data = data with { HashedPassword = hpw };

        var res = await ApiHolder.Api.UpdatePasswordAsync(hpw);
        if (!res.IsSuccess)
        {
          _ = MessageBox.Show("パスワード更新に失敗しました\nErrorCode:" + res.ResultCode.ToString(), "ShiftManager");
          return;
        }

        data = data with { HashedPassword = new HashedPassword() };
      }

      if(userData != data) //パスワード以外も変更されているかチェック
      {
        var res = await ApiHolder.Api.UpdateUserDataAsync(data);
        if (!res.IsSuccess)
        {
          _ = MessageBox.Show("ユーザー情報更新に失敗しました\nErrorCode:" + res.ResultCode.ToString(), "ShiftManager");
          return;
        }
      }

      UpdateUserDataSuccessed?.Invoke(this, EventArgs.Empty);
      _ = MessageBox.Show("ユーザー情報更新に成功しました", "ShiftManager");
    }
  }
}
