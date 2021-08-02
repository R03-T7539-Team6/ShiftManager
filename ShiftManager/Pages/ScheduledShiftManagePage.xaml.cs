using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

using AutoNotify;

using Reactive.Bindings;

using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftManagePage.xaml
  /// </summary>
  public partial class ScheduledShiftManagePage : Page, IContainsApiHolder, IContainsIsProcessing
  {
    public IApiHolder ApiHolder { get => VM.ApiHolder; set => VM.ApiHolder = value; }
    ScheduledShiftManagePageViewModel VM = new();

    DateTime LastShowingDate { get; set; }
    public ReactivePropertySlim<bool> IsProcessing { get; set; }

    /*******************************************
* specification ;
* name = ScheduledShiftManagePage ;
* Function = インスタンスを初期化します ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public ScheduledShiftManagePage()
    {
      InitializeComponent();

      VM.ShiftRequestArray = new();
      VM.ScheduledShiftArray = new();
      DataContext = VM;
      LastShowingDate = VM.TargetDate;
    }

    ImmutableArray<ShiftRequest> ShiftRequests;

    /*******************************************
* specification ;
* name = Page_Loaded ;
* Function = ページが表示された際に実行され, 表示するデータの読み込みを行います ;
* note = 本PageのLoadedイベントをハンドリングしています ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Pageのインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void Page_Loaded(object sender, RoutedEventArgs e) => ReloadData();

    /*******************************************
* specification ;
* name = ReloadData ;
* Function = 表示するデータの再読み込みを行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async void ReloadData()
    {
      if (IsProcessing is not null)
        IsProcessing.Value = true;

      VM.ApiHolder = ApiHolder;

      var userIDArr = await ReloadShiftRequest();
      _ = await ReloadScheduledShift(userIDArr);

      LastShowingDate = VM.TargetDate;

      if (IsProcessing is not null)
        IsProcessing.Value = false;
    }

    /*******************************************
* specification ;
* name = ReloadShiftRequest ;
* Function = 希望シフトリストを更新します ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async Task<UserID[]> ReloadShiftRequest(UserID[] IDArr = null)
    {
      DateTime targetDate = VM.TargetDate.Date;
      List<UserID> additionalUserID = new();

      if (!(IDArr?.Length > 0)) //NULLであるか, あるいは長さが0以下の場合
        IDArr = (await ApiHolder.Api.GetUsersByUserStateAsync(UserState.Normal)).ReturnData.Select(v => new UserID(v.UserID)).ToArray();

      VM.ShiftRequestArray.Clear();

      for (int i = 0; i < IDArr.Length; i++)
        VM.ShiftRequestArray.Add(new SingleShiftData(IDArr[i], targetDate, false, targetDate, targetDate, new()));


      var shiftReqs = await ApiHolder.Api.GetAllShiftRequestAsync();
      if (!shiftReqs.IsSuccess)
      {
        _ = MessageBox.Show("Cannot Get ShiftRequest\nErrorCode:" + shiftReqs.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
        return IDArr;
      }

      ShiftRequests = shiftReqs.ReturnData;
      foreach (var i in ShiftRequests)
      {
        bool IDFound = false;

        for (int index = 0; index < IDArr.Length; index++)
          if (IDArr[index] == new UserID(i.UserID))
          {
            if (!i.RequestsDictionary.TryGetValue(targetDate, out ISingleShiftData ssd))
              ssd = new SingleShiftData(i.UserID, targetDate, false, targetDate, targetDate, new());

            VM.ShiftRequestArray[index] = ssd;
            IDFound = true;
            break;
          }

        if (!IDFound)
        {
          VM.ShiftRequestArray.Add(new SingleShiftData(i.UserID, targetDate, false, targetDate, targetDate, new()));
          additionalUserID.Add(new(i.UserID));
        }
      }

      return IDArr.Concat(additionalUserID).ToArray();
    }

    /*******************************************
* specification ;
* name = ReloadScleduledShift ;
* Function = 予定シフトリストを更新します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async Task<UserID[]> ReloadScheduledShift(UserID[] IDArr = null)
    {
      DateTime targetDate = VM.TargetDate.Date;
      List<UserID> additionalUserID = new();

      if (!(IDArr?.Length > 0)) //NULLであるか, あるいは長さが0以下の場合
        IDArr = (await ApiHolder.Api.GetUsersByUserStateAsync(UserState.Normal)).ReturnData.Select(v => new UserID(v.UserID)).ToArray();

      VM.ScheduledShiftArray.Clear();

      for (int i = 0; i < IDArr.Length; i++)
        VM.ScheduledShiftArray.Add(new SingleShiftData(IDArr[i], targetDate, false, targetDate, targetDate, new()));

      var ret = await ApiHolder.Api.GetScheduledShiftByDateAsync(targetDate);
      if (!ret.IsSuccess)
      {
        if (ret.ResultCode == Communication.ApiResultCodes.Target_Date_Not_Found)
          ret = await ApiHolder.Api.GenerateScheduledShiftAsync(targetDate);
        else
        {
          _ = MessageBox.Show("Error has occured\nErrorCode:" + ret.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return IDArr;
        }
      }

      if (ret.ReturnData is null)
        return IDArr;

      Dictionary<UserID, ISingleShiftData> shiftDic = new(ret.ReturnData.ShiftDictionary);

      for (int index = 0; index < IDArr.Length; index++)
      {
        if (shiftDic.TryGetValue(IDArr[index], out var value))
        {
          VM.ScheduledShiftArray[index] = new SingleShiftData(value);
          shiftDic.Remove(IDArr[index]);
        }
      }

      foreach (var item in shiftDic.Values)
      {
        VM.ScheduledShiftArray.Add(new SingleShiftData(item));
        VM.ShiftRequestArray.Add(new SingleShiftData(item.UserID, targetDate, false, targetDate, targetDate, new())); //スクロール位置同期が崩れないように, シフト希望にも空要素を追加しておく
        additionalUserID.Add(new(item.UserID));
      }

      return IDArr.Concat(additionalUserID).ToArray();
    }

    /*******************************************
* specification ;
* name = UpdateScheduledShift ;
* Function = 予定シフトの更新要求をサーバに送り, 情報更新を試行します ;
* note = 更新に失敗した場合, MessageBoxが表示されます ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async void UpdateScheduledShift()
    {
      //シフトを登録
      if (VM.ScheduledShiftArray.Count <= 0)
        return;

      if (IsProcessing is not null)
        IsProcessing.Value = true;

      DateTime targetDate = LastShowingDate;
      var scheduledShifts = await ApiHolder.Api.UpdateSingleScheduledShiftListAsync(targetDate, VM.ScheduledShiftArray);

      if (!scheduledShifts.IsSuccess)
        if (MessageBox.Show("Error has occured\nErrorCode:" + scheduledShifts.ResultCode.ToString() + "\nRetry?", "ShiftManager", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
          UpdateScheduledShift();

      UpdateShiftSchedulingState(targetDate);

      if (IsProcessing is not null)
        IsProcessing.Value = false;
    }

    /*******************************************
* specification ;
* name = UpdateShiftSchedulingState ;
* Function = 指定の日付のシフト編集状態を更新します ;
* note = v1.0では未実装機能です ;
* date = 07/05/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 対象となる日付 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void UpdateShiftSchedulingState(DateTime targetDate)
    {
      //実装準備状態とする
      /*var stateUpdateResult = await ApiHolder.Api.UpdateShiftSchedulingStateAsync(targetDate, VM.ShiftSchedulingState);

      if (!stateUpdateResult.IsSuccess)
        if (MessageBox.Show("Error has occured\nErrorCode:" + stateUpdateResult.ResultCode.ToString() + "\nRetry?", "ShiftManager", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
          UpdateShiftSchedulingState(targetDate);*/
    }

    /*******************************************
* specification ;
* name = DataPicker_SelectedDateChanged ;
* Function = 日付選択コントロールで日付の選択が更新された際に実行され, 表示中データの再読み込みを行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = DataPickerインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ReloadData();

    /*******************************************
* specification ;
* name = ListView_Scroll ;
* Function = ListViewでスクロールが行われた際に実行され, 表示位置の同期を行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = ListViewインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void ListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) => ScrollSyncer(sender);

    /*******************************************
* specification ;
* name = ShiftRequestListView_MouseWheel ;
* Function = ListViewでマウスホイールによる操作が行われた際に実行され, 表示位置の同期を行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = ListViewインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void ShiftRequestListView_MouseWheel(object sender, MouseWheelEventArgs e) => ScrollSyncer(sender);

    /*******************************************
* specification ;
* name = ScrollSyncer ;
* Function = 2つのListView間で表示位置の同期を行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 変更のあったListViewインスタンス ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void ScrollSyncer(object sender)
    {
      static IScrollProvider GetScrollProvider(in UIElement uie)
        => UIElementAutomationPeer.CreatePeerForElement(uie)?.GetPattern(PatternInterface.Scroll) as IScrollProvider;

      if (sender is not ListView lv)
        return;

      //ref : https://blog.okazuki.jp/entry/20120105/1325771139
      var scrollProvider = GetScrollProvider(lv);

      if (scrollProvider is null)
        return;

      double scrollPercent = scrollProvider.VerticalScrollPercent;
      scrollPercent =
        scrollPercent < 0 ? 0 :
        scrollPercent > 100 ? 100 :
        scrollPercent;

      var srlvScrollProvider = GetScrollProvider(ShiftRequestListView);
      if (srlvScrollProvider.VerticallyScrollable && srlvScrollProvider.VerticalScrollPercent != scrollPercent)
        srlvScrollProvider.SetScrollPercent(ScrollPatternIdentifiers.NoScroll, scrollPercent);

      var sslvScrollProvider = GetScrollProvider(ScheduledShiftListView);
      if (sslvScrollProvider.VerticallyScrollable && sslvScrollProvider.VerticalScrollPercent != scrollPercent)
        sslvScrollProvider.SetScrollPercent(ScrollPatternIdentifiers.NoScroll, scrollPercent);
    }

    public static readonly ShiftSchedulingState[] ShiftSchedulingStateLabels = (ShiftSchedulingState[])Enum.GetValues(typeof(ShiftSchedulingState));

    /*******************************************
* specification ;
* name = Page_Unloaded ;
* Function = 本Pageから他のページに移動した際に実行され, サーバーに変更を送信します ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Pageインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void Page_Unloaded(object sender, RoutedEventArgs e) => UpdateScheduledShift();

    private void SaveButtonClocked(object sender, RoutedEventArgs e) => UpdateScheduledShift();

  }

  public partial class ScheduledShiftManagePageViewModel : INotifyPropertyChanged, IContainsApiHolder
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _ShiftRequestArray;

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _ScheduledShiftArray;

    [AutoNotify]
    private DateTime _TargetDate = DateTime.Now.Date;

    [AutoNotify]
    private ShiftSchedulingState _ShiftSchedulingState = ShiftSchedulingState.NotStarted;

    [AutoNotify]
    private IApiHolder _ApiHolder;
  }
}
