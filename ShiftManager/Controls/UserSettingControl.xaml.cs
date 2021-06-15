using System.Windows;
using System.Windows.Controls;

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

    public UserSettingControl() => InitializeComponent();
  }
}
