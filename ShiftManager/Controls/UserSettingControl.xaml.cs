using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

    public string PasswordText { get => (string)GetValue(PasswordTextProperty); protected set => SetValue(PasswordTextPropertyKey, value); }
    static private DependencyPropertyKey PasswordTextPropertyKey = DependencyProperty.RegisterReadOnly(nameof(PasswordText), typeof(string), typeof(UserSettingControl), new());
    static public DependencyProperty PasswordTextProperty = PasswordTextPropertyKey.DependencyProperty;

    public string PasswordReTypedText { get => (string)GetValue(PasswordReTypedTextProperty); protected set => SetValue(PasswordReTypedTextPropertyKey, value); }
    static private DependencyPropertyKey PasswordReTypedTextPropertyKey = DependencyProperty.RegisterReadOnly(nameof(PasswordReTypedText), typeof(string), typeof(UserSettingControl), new());
    static public DependencyProperty PasswordReTypedTextProperty = PasswordReTypedTextPropertyKey.DependencyProperty;

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

    public UserSettingControl()
    {
      InitializeComponent();

      PWStrengthVisualizer.SetBinding(PasswordStrengthVisualizerControl.PasswordTextProperty, new Binding(nameof(PasswordText)) { Source = this });
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
      IsReTypedPasswordNotSame = PWBox.Password != PWBoxToReType.Password;

      if(sender is PasswordBox pb)
        switch(pb.Name)
        {
          case nameof(PWBox):
            PasswordText = pb.Password;
            break;
          case nameof(PWBoxToReType):
            PasswordReTypedText = pb.Password;
            break;
        }
    }

    private void GeneratePW(object sender, RoutedEventArgs e)
    {
      PasswordText = PasswordReTypedText = PWBox.Password = PWBoxToReType.Password = "AutoGeneratedPassword";//仮実装
    }
  }
}
