using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

using ShiftManager.DataClasses;

namespace ShiftManager.Controls
{
  /// <summary>
  /// Interaction logic for UserSettingControl.xaml
  /// </summary>
  public partial class UserSettingControl : UserControl
  {
    #region 操作の許可/不許可設定用(依存関係)プロパティ群
    public bool CanEditUserID { get => (bool)GetValue(CanEditUserIDProperty); set => SetValue(CanEditUserIDProperty, value); }
    static public DependencyProperty CanEditUserIDProperty = DependencyProperty.Register(nameof(CanEditUserID), typeof(bool), typeof(UserSettingControl));

    public bool CanEditUserName { get => (bool)GetValue(CanEditUserNameProperty); set => SetValue(CanEditUserNameProperty, value); }
    static public DependencyProperty CanEditUserNameProperty = DependencyProperty.Register(nameof(CanEditUserName), typeof(bool), typeof(UserSettingControl));

    public bool CanEditPassword { get => (bool)GetValue(CanEditPasswordProperty); set => SetValue(CanEditPasswordProperty, value); }
    static public DependencyProperty CanEditPasswordProperty = DependencyProperty.Register(nameof(CanEditPassword), typeof(bool), typeof(UserSettingControl));

    public bool CanEditUserGroup { get => (bool)GetValue(CanEditUserGroupProperty); set => SetValue(CanEditUserGroupProperty, value); }
    static public DependencyProperty CanEditUserGroupProperty = DependencyProperty.Register(nameof(CanEditUserGroup), typeof(bool), typeof(UserSettingControl));

    public bool CanEditUserState { get => (bool)GetValue(CanEditUserStateProperty); set => SetValue(CanEditUserStateProperty, value); }
    static public DependencyProperty CanEditUserStateProperty = DependencyProperty.Register(nameof(CanEditUserState), typeof(bool), typeof(UserSettingControl));

    public bool CanEditNotificationSetting { get => (bool)GetValue(CanEditNotificationSettingProperty); set => SetValue(CanEditNotificationSettingProperty, value); }
    static public DependencyProperty CanEditNotificationSettingProperty = DependencyProperty.Register(nameof(CanEditNotificationSetting), typeof(bool), typeof(UserSettingControl));

    public bool CanPrintSettings { get => (bool)GetValue(CanPrintSettingsProperty); set => SetValue(CanPrintSettingsProperty, value); }
    static public DependencyProperty CanPrintSettingsProperty = DependencyProperty.Register(nameof(CanPrintSettings), typeof(bool), typeof(UserSettingControl));

    public bool CanSaveSettings { get => (bool)GetValue(CanSaveSettingsProperty); set => SetValue(CanSaveSettingsProperty, value); }
    static public DependencyProperty CanSaveSettingsProperty = DependencyProperty.Register(nameof(CanSaveSettings), typeof(bool), typeof(UserSettingControl));
    #endregion

    #region 入力内容取得用
    public string UserIDText { get => (string)GetValue(UserIDTextProperty); set => SetValue(UserIDTextProperty, value); }
    static public DependencyProperty UserIDTextProperty = DependencyProperty.Register(nameof(UserIDText), typeof(string), typeof(UserSettingControl));

    public string FirstNameText { get => (string)GetValue(FirstNameTextProperty); set => SetValue(FirstNameTextProperty, value); }
    static public DependencyProperty FirstNameTextProperty = DependencyProperty.Register(nameof(FirstNameText), typeof(string), typeof(UserSettingControl));

    public string LastNameText { get => (string)GetValue(LastNameTextProperty); set => SetValue(LastNameTextProperty, value); }
    static public DependencyProperty LastNameTextProperty = DependencyProperty.Register(nameof(LastNameText), typeof(string), typeof(UserSettingControl));

    public string PasswordText { get => (string)GetValue(PasswordTextProperty); set => SetValue(PasswordTextProperty, value); }
    static public DependencyProperty PasswordTextProperty = DependencyProperty.Register(nameof(PasswordText), typeof(string), typeof(UserSettingControl),
      new(string.Empty, (d, e) => UpdatePWBoxText((d as UserSettingControl)?.PWBox, e.NewValue), PasswordCoerceCallback));

    public bool IsPasswordVisible { get => (bool)GetValue(IsPasswordVisibleProperty); set => SetValue(IsPasswordVisibleProperty, value); }
    static public DependencyProperty IsPasswordVisibleProperty = DependencyProperty.Register(nameof(IsPasswordVisible), typeof(bool), typeof(UserSettingControl));

    public UserGroup SelectedUserGroup { get => (UserGroup)GetValue(SelectedUserGroupProperty); set => SetValue(SelectedUserGroupProperty, value); }
    static public DependencyProperty SelectedUserGroupProperty = DependencyProperty.Register(nameof(SelectedUserGroup), typeof(UserGroup), typeof(UserSettingControl));

    public UserState SelectedUserState { get => (UserState)GetValue(SelectedUserStateProperty); set => SetValue(SelectedUserStateProperty, value); }
    static public DependencyProperty SelectedUserStateProperty = DependencyProperty.Register(nameof(SelectedUserState), typeof(UserState), typeof(UserSettingControl));

    public NotificationPublishTimings NotificationSetting { get => (NotificationPublishTimings)GetValue(NotificationSettingProperty); set => SetValue(NotificationSettingProperty, value); }
    static public DependencyProperty NotificationSettingProperty = DependencyProperty.Register(nameof(NotificationSetting), typeof(NotificationPublishTimings), typeof(UserSettingControl));
    #endregion
    public bool IsReTypedPasswordNotSame { get => (bool)GetValue(IsReTypedPasswordNotSameProperty); protected set => SetValue(IsReTypedPasswordNotSamePropertyKey, value); }
    static private DependencyPropertyKey IsReTypedPasswordNotSamePropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsReTypedPasswordNotSame), typeof(bool), typeof(UserSettingControl), new());
    static public DependencyProperty IsReTypedPasswordNotSameProperty = IsReTypedPasswordNotSamePropertyKey.DependencyProperty;

    public int PasswordMaxLength { get; } = 32;
    UserData RemoteData { get; set; }

    static object PasswordCoerceCallback(DependencyObject d, object baseValue)
    {
      if (d is UserSettingControl c && baseValue is string s && s.Length >= c.PasswordMaxLength)
        return s.Substring(0, c.PasswordMaxLength);
      else
        return baseValue;
    }

    static void UpdatePWBoxText(PasswordBox target, object newValue)
    {
      if (target is not null && newValue is string s)
        if (target.Password != s)
          target.Password = s;
    }

    IUserData InitialUserData { get; set; }
    public void SetData(IUserData userData)
    {
      InitialUserData = userData;
      UserIDText = userData.UserID.Value;
      FirstNameText = userData.FullName.FirstName;
      LastNameText = userData.FullName.LastName;
      PasswordText = string.Empty;
      SelectedUserGroup = userData.UserGroup;
      SelectedUserState = userData.UserState;
      //通知は実装省略
    }

    public UserSettingControl()
    {
      InitializeComponent();

    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
      if (sender is PasswordBox pb)
        PasswordText = pb.Password;
    }

    private void GeneratePW(object sender, RoutedEventArgs e) => GeneratePW();
    private void GeneratePW()
    {
      var r = new RNGCryptoServiceProvider();
      byte[] ba = new byte[32];//生成先のバッファ

      for (int trycount = 0; trycount < 10; trycount++)//さすがに10回もやれば大丈夫なはず
      {
        r.GetNonZeroBytes(ba);//1~255の乱数を32個生成する
        List<char> GeneratedPW = new();//生成されたパスワードを一時的に保管するバッファ

        foreach (var b in ba)
        {
          char c = (char)(b / 2);//ASCII範囲内のみを使用するため (0 ~ 127)
          if (char.IsLetterOrDigit(c) || char.IsSymbol(c))//文字 OR 数字 OR 記号の場合のみ
            GeneratedPW.Add(c);//パスワード候補バッファに追加
        }

        //生成パスワードの文字数は16文字とする
        if (GeneratedPW.Count < 16)
          continue;//16文字未満は再試行

        string generatedPWStr = new(GeneratedPW.GetRange(0, 16).ToArray());//パスワード文字列生成
        if (PasswordStrengthVisualizerControl.GetPasswordStrength(generatedPWStr) < 1.0)
          continue;//強度不足なら再試行

        PasswordText = generatedPWStr;//生成したパスワードを設定
        MessageBox.Show("パスワードを生成しました.  生成されたパスワードは, 以下の通りです.\n\n" + PasswordText, "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Information);
        return;//生成終了
      }

      if (MessageBox.Show("パスワードの生成に失敗しました.  再試行しますか?", "ShiftManager", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        GeneratePW();//再試行
    }

    private void PrintClicked(object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void DiscardChanges(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show("変更内容が破棄されます.  よろしいですか?", "ShiftManager", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
        SetData(InitialUserData);
    }

    private void SaveChanges(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(PasswordText) && PWStrengthVisualizer.PasswordStrength < PasswordStrengthVisualizerControl.PWStrength_Least)
      {
        MessageBox.Show("パスワードの強度が低いです.", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
        (IsPasswordVisible ? PWVisibleBox as UIElement : PWBox).Focus();//パスワードを変更させるため, PWBoxにフォーカスを当てる
        return;
      }

      //TODO: APIを使用して設定を登録する
    }
  }
}
