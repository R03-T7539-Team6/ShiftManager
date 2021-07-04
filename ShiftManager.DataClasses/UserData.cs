using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  #region Records
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record UserID(string Value) : IUserID
  {
    public UserID() : this(string.Empty) { }
    public UserID(IUserID i) : this(i?.Value ?? string.Empty) { }
  }
  public record UserData(IUserID UserID, IHashedPassword HashedPassword, INameData FullName, IStoreID StoreID, UserGroup UserGroup, UserState UserState, IWorkLog WorkLog, IUserSetting UserSetting) : IUserData
  {
    public UserData(IUserData i) : this(
      i?.UserID ?? new UserID(),
      i?.HashedPassword ?? new HashedPassword(),
      i?.FullName ?? new NameData(),
      i?.StoreID ?? new StoreID(),
      i?.UserGroup ?? UserGroup.None,
      i?.UserState ?? UserState.Others,
      i?.WorkLog ?? new WorkLog(i?.UserID ?? new UserID(), new()),
      i?.UserSetting ?? new UserSetting(i?.UserID ?? new UserID(), NotificationPublishTimings.None, new()))
    { }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record HashedPassword(string Hash, string Salt, int StretchCount) : IHashedPassword
  {
    public HashedPassword() : this(string.Empty, string.Empty, 0) { }
    public HashedPassword(IHashedPassword i) : this(i?.Hash ?? string.Empty, i?.Salt ?? string.Empty, i?.StretchCount ?? 0) { }
  }

  /// <summary>氏名情報 (FirstName:名前, LastName:苗字)</summary>
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record NameData(string FirstName, string LastName) : INameData
  {
    public NameData() : this(string.Empty, string.Empty) { }
    public NameData(INameData i) : this(i?.FirstName ?? string.Empty, i?.LastName ?? string.Empty) { }
  }
  #endregion

  #region NotifyPropertuChanged Classes
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public partial class UserID_NotifyPropertyChanged : IUserID, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private string _Value = string.Empty;
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public partial class UserData_NotifyPropertyChanged : IUserData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID();
    [AutoNotify]
    private IHashedPassword _HashedPassword = new HashedPassword();
    [AutoNotify]
    private INameData _FullName = new NameData();
    [AutoNotify]
    private IStoreID _StoreID = new StoreID();
    [AutoNotify]
    private UserGroup _UserGroup;
    [AutoNotify]
    private UserState _UserState;
    [AutoNotify]
    private IWorkLog _WorkLog = new WorkLog(new UserID(), new());
    [AutoNotify]
    private IUserSetting _UserSetting = new UserSetting(new UserID(), NotificationPublishTimings.None, new());
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public partial class HashedPassword_NotifyPropertyChanged : IHashedPassword, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private string _Hash = string.Empty;
    [AutoNotify]
    private string _Salt = string.Empty;
    [AutoNotify]
    private int _StretchCount;
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public partial class NameData_PropertyChanged : INameData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public NameData_PropertyChanged() { }
    public NameData_PropertyChanged(INameData i) { FirstName = i?.FirstName ?? string.Empty; LastName = i?.LastName ?? string.Empty; }

    [AutoNotify]
    private string _FirstName = string.Empty;
    [AutoNotify]
    private string _LastName = string.Empty;
  }
  #endregion

  #region enums
  /// <summary>ユーザの種別を表す</summary>
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
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
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
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
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface IUserID
  {
    string Value { get; }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface IUserData
  {
    IUserID UserID { get; }
    IHashedPassword HashedPassword { get; }
    INameData FullName { get; }
    IStoreID StoreID { get; }
    UserGroup UserGroup { get; }
    UserState UserState { get; }
    IWorkLog WorkLog { get; }
    IUserSetting UserSetting { get; }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface IHashedPassword
  {
    string Hash { get; }
    string Salt { get; }
    int StretchCount { get; }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface INameData
  {
    string FirstName { get; }
    string LastName { get; }
  }
  #endregion
}
