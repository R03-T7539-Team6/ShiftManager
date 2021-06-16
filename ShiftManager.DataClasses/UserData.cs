namespace ShiftManager.DataClasses
{
  public record UserID(string Value) : IUserID;
  public record UserData(UserID UserID, HashedPassword HashedPassword, NameData FullName, UserGroup UserGroup, UserState UserState, WorkLog WorkLog, UserSetting UserSetting) : IUserData;

  public record HashedPassword(string Hash, string Salt, int StretchCount) : IHashedPassword;

  /// <summary>氏名情報 (FirstName:名前, LastName:苗字)</summary>
  public record NameData(string FirstName, string LastName) : INameData;

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

  public interface IUserID
  {
    string Value { get; }
  }

  public interface IUserData
  {
    UserID UserID { get; }
    HashedPassword HashedPassword { get; }
    NameData FullName { get; }
    UserGroup UserGroup { get; }
    UserState UserState { get; }
    WorkLog WorkLog { get; }
    UserSetting UserSetting { get; }
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
}
