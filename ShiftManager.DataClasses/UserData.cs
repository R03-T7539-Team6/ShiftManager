using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
	#region Records
	public record UserID(string Value) : IUserID;
  public record UserData(IUserID UserID, IHashedPassword HashedPassword, INameData FullName, UserGroup UserGroup, UserState UserState, IWorkLog WorkLog, IUserSetting UserSetting) : IUserData;

  public record HashedPassword(string Hash, string Salt, int StretchCount) : IHashedPassword;

  /// <summary>氏名情報 (FirstName:名前, LastName:苗字)</summary>
  public record NameData(string FirstName, string LastName) : INameData;
  #endregion

  #region NotifyPropertuChanged Classes
  public partial class UserID_NotifyPropertyChanged : IUserID, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private string _Value;
  }

  public partial class UserData_NotifyPropertyChanged : IUserData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private IUserID _UserID;
    [AutoNotify]
    private IHashedPassword _HashedPassword;
    [AutoNotify]
    private INameData _FullName;
    [AutoNotify]
    private UserGroup _UserGroup;
    [AutoNotify]
    private UserState _UserState;
    [AutoNotify]
    private IWorkLog _WorkLog;
    [AutoNotify]
    private IUserSetting _UserSetting;
  }

  public partial class HashedPassword_NotifyPropertyChanged : IHashedPassword, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private string _Hash;
    [AutoNotify]
    private string _Salt;
    [AutoNotify]
    private int _StretchCount;
  }

  public partial class NameData_PropertyChanged : INameData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private string _FirstName;
    [AutoNotify]
    private string _LastName;
  }
  #endregion

  #region enums
  /// <summary>ユーザの種別を表す</summary>
  public enum UserGroup
  {
    /// <summary>デフォルト値</summary>
    None,

    /// <summary>システム管理者 (データ編集用)</summary>
    SystemAdmin,

    /// <summary>権威ユーザ (店舗責任者)</summary>
    SuperUser,

    /// <summary>一般ユーザ</summary>
    NormalUser,

    /// <summary>勤怠登録/店舗端末用</summary>
    ForTimeRecordTerminal
  }

  /// <summary>ユーザの状態</summary>
  public enum UserState
  {
    /// <summary>通常</summary>
    Normal,

    /// <summary>休職中</summary>
    InLeaveOfAbsence,

    /// <summary>退職済み</summary>
    Retired,

    /// <summary>未採用</summary>
    NotHired,

    /// <summary>その他</summary>
    Others
  }
	#endregion

	#region interfaces
	public interface IUserID
  {
    string Value { get; }
  }

  public interface IUserData
  {
    IUserID UserID { get; }
    IHashedPassword HashedPassword { get; }
    INameData FullName { get; }
    UserGroup UserGroup { get; }
    UserState UserState { get; }
    IWorkLog WorkLog { get; }
    IUserSetting UserSetting { get; }
  }

  public interface IHashedPassword
  {
    string Hash { get; }
    string Salt { get; }
    int StretchCount { get; }
  }

  public interface INameData
  {
    string FirstName { get; }
    string LastName { get; }
  }
	#endregion
}
