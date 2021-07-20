using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IApi : InternalApi_ManageData, InternalApi_ScheduledShift, InternalApi_ShiftRequest, InternalApi_WorkLog, IInternalApi_SignIn, IInternalApi_StoreData, IInternalApi_UserData, IInternalApi_Password
  { }

  /// <summary>データ操作に関するメソッド群を規定しています</summary>
  public interface InternalApi_ManageData
  {
    /// <summary>閲覧可能なデータのリストを返します</summary>
    /// <returns>閲覧可能なデータのカテゴリとデータ名(ファイル名)のDictionaryを含む実行結果</returns>
    Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync();

    /// <summary>データカテゴリとデータ名, データ型を指定してデータ内容を取得する</summary>
    /// <typeparam name="T">データの型</typeparam>
    /// <param name="Category">データのカテゴリ</param>
    /// <param name="DataName">データ名</param>
    /// <returns>取得したデータを含む実行結果</returns>
    Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName);

    /// <summary>指定のデータカテゴリとデータ名を指定のデータで更新する</summary>
    /// <typeparam name="T">データ型</typeparam>
    /// <param name="Category">データカテゴリ</param>
    /// <param name="DataName">データ名</param>
    /// <param name="Data">データ</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data);
  }

  /// <summary>パスワードに関するメソッドを規定しています</summary>
  public interface IInternalApi_Password
  {
    /// <summary>指定したユーザのパスワードのハッシュ化に必要な情報を取得する</summary>
    /// <param name="userID">取得するユーザのID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID);
  }

  /// <summary>予定シフトの操作に関するメソッドを規定しています</summary>
  public interface InternalApi_ScheduledShift
  {
    /// <summary>指定した人/指定した日の予定シフトを取得する  存在しなければ新規に作成されて返る</summary>
    /// <param name="targetDate">勤務予定日</param>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, IUserID userID);

    /// <summary>予定シフトの編集状態を更新する</summary>
    /// <param name="targetDate">日付</param>
    /// <param name="shiftSchedulingState">設定する編集状態</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState);

    /// <summary>指定した日に勤務する予定の人の予定シフトをリストで更新する  リストに含まれない人のシフトは更新されない</summary>
    /// <param name="targetDate">勤務予定日</param>
    /// <param name="singleShiftDatas">更新するデータ群</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas);

    /// <summary>指定した日の必要従業員人数リストを更新する</summary>
    /// <param name="targetDate">日付</param>
    /// <param name="singleShiftDatas">必要従業員人数リスト</param>
    /// <returns></returns>
    Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> singleShiftDatas);
  }

  /// <summary>シフト希望の操作に関するメソッド群を規定しています</summary>
  public interface InternalApi_ShiftRequest
  {
    /// <summary>シフト希望を登録する  既に存在した場合はデータが更新されます  (非同期実行)</summary>
    /// <param name="singleShiftData">登録するシフト希望</param>
    /// <returns>実行結果</returns>
    ApiResult AddShiftRequest(ISingleShiftData singleShiftData);

    /// <summary>シフト希望を登録する  既に存在した場合はデータが更新されます</summary>
    /// <param name="singleShiftData">登録するシフト希望</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData);

    /// <summary>現在のユーザの指定の日付のシフト希望を取得する  存在しなければ新規に作成されて返る</summary>
    /// <remarks>内部的には, GetShiftRequestByIDAsyncを実行してそこから日付でピックアップしています</remarks>
    /// <param name="date">希望した日付</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date);

    [Obsolete("Please use AddShiftRequestAsync method")]
    Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData);
  }

  /// <summary>サインイン機能に関するメソッド群を規定しています</summary>
  public interface IInternalApi_SignIn
  {
    /// <summary>現在サインイン中のユーザのデータ</summary>
    IUserData? CurrentUserData { get; }

    /// <summary>サインインを試行します.</summary>
    /// <param name="userID">ユーザID</param>
    /// <param name="HashedPasswordGetter">パスワードのハッシュ化に関する情報を受けてハッシュ化パスワードを返す関数</param>
    /// <returns>試行結果</returns>
    Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword);

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult> SignOutAsync();
  }

  /// <summary>店舗情報を操作するようなメソッド群を規定しています</summary>
  public interface IInternalApi_StoreData
  {
    //自身が所属する店舗に対してのみ操作可能

    /// <summary>指定の日付の予定シフトが確定済みか確認します</summary>
    /// <param name="date">確認する日付</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date);

    /// <summary>指定のユーザのユーザデータを取得します</summary>
    /// <param name="userID">取得するユーザのユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID);

    /// <summary>指定のユーザデータを追加する  既にユーザが存在した場合はエラーが返る</summary>
    /// <param name="userData">追加するユーザデータ</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> SignUpAsync(IUserData userData);

    /// <summary>現在の店舗に所属する全てのユーザのデータを取得する</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync();

    /// <summary>現在の店舗に所属するユーザのうち, 指定のグループに属するユーザ群を取得する</summary>
    /// <param name="userGroup">取得するユーザ群のユーザグループ</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None);

    /// <summary>現在の店舗に所属するユーザのうち, 指定の状態であるユーザ群を取得する</summary>
    /// <param name="userState">取得するユーザ群のユーザグループ</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal);

    /// <summary>指定のユーザのシフト希望を取得する</summary>
    /// <param name="userID">取得するユーザのID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID);

    /// <summary>現在の店舗に所属するすべてのユーザのシフト希望を取得する</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync();

    /// <summary>指定のユーザのシフト希望ファイルを作成します  既に存在する場合は作成済みのものが返ります</summary>
    /// <param name="userID">作成するユーザのID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID);

    /// <summary>指定の日付の予定シフトを取得します</summary>
    /// <param name="dateTime">取得する予定シフトの日付</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime);

    /// <summary>指定の日付の予定シフトを生成します  既に存在する場合は作成済みのものが返ります</summary>
    /// <param name="dateTime">作成する予定シフトの日付</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<ScheduledShift>> GenerateScheduledShiftAsync(DateTime dateTime);

    /// <summary>指定のユーザを削除します</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> DeleteUserDataAsync(IUserID userID);
  }

  /// <summary>ユーザデータに関するメソッド群を規定しています</summary>
  public interface IInternalApi_UserData
  {
    /// <summary>現在サインイン中のユーザのパスワードを更新します</summary>
    /// <param name="hashedPassword">ハッシュ化されたパスワード</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword);

    /// <summary>指定のユーザ名と氏名を持つユーザのパスワードを更新します  (実行にサインイン不要  成功後要サインイン)</summary>
    /// <param name="userID">更新するユーザのID</param>
    /// <param name="nameData">更新するユーザの氏名</param>
    /// <param name="hashedPassword">更新するユーザのハッシュ化パスワード</param>
    /// <returns>実行結果</returns>
    Task<ApiResult> UpdatePasswordAsync(IUserID userID, INameData nameData, IHashedPassword hashedPassword);

    /// <summary>現在サインイン中のユーザの勤務実績を取得します</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult<WorkLog>> GetWorkLogAsync();

    /// <summary>現在サインイン中のユーザのユーザ設定を取得します</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult<UserSetting>> GetUserSettingAsync();

    Task<ApiResult<UserData>> UpdateUserDataAsync(IUserData userData);
  }

  /// <summary>勤怠実績の操作に関するメソッド群を規定しています</summary>
  public interface InternalApi_WorkLog
  {
    /// <summary>出勤打刻を行います</summary>
    /// <param name="userID">打刻を行うユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID);

    /// <summary>出勤打刻を行います</summary>
    /// <param name="userID">打刻を行うユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID);

    /// <summary>休憩開始打刻を行います</summary>
    /// <param name="userID">打刻を行うユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID);

    /// <summary>休憩終了打刻を行います</summary>
    /// <param name="userID">打刻を行うユーザID</param>
    /// <returns>実行結果</returns>
    Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID);
  }
}
