using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

using AutoNotify;

using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftManagePage.xaml
  /// </summary>
  public partial class ScheduledShiftManagePage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get => VM.ApiHolder; set => VM.ApiHolder = value; }
    ScheduledShiftManagePageViewModel VM = new();

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
    private void ReloadData()
    {
      VM.ApiHolder = ApiHolder;
      ReloadShiftRequest();
      ReloadScheduledShift();
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
    private async void ReloadShiftRequest()
    {
      var shiftReqs = await ApiHolder.Api.GetAllShiftRequestAsync();
      if (!shiftReqs.IsSuccess)
      {
        _ = MessageBox.Show("Cannot Get ShiftRequest\nErrorCode:" + shiftReqs.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      ShiftRequests = shiftReqs.ReturnData;
      VM.ShiftRequestArray.Clear();
      foreach (var i in ShiftRequests)
      {
        if (!i.RequestsDictionary.TryGetValue(VM.TargetDate, out ISingleShiftData ssd))
          ssd = new SingleShiftData(i.UserID, VM.TargetDate, false, VM.TargetDate, VM.TargetDate, new());
        VM.ShiftRequestArray.Add(ssd);
      }
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
    private async void ReloadScheduledShift()
    {
      var ret = await ApiHolder.Api.GetScheduledShiftByDateAsync(VM.TargetDate);
      if (!ret.IsSuccess)
      {
        if (ret.ResultCode == Communication.ApiResultCodes.Target_Date_Not_Found)
          ret = await ApiHolder.Api.GenerateScheduledShiftAsync(VM.TargetDate);
        else
        {
          MessageBox.Show("Error has occured\nErrorCode:" + ret.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }
      }
      VM.ScheduledShiftArray.Clear();

      if (ret.ReturnData is null)
        return;

      foreach (var i in ret.ReturnData?.ShiftDictionary.Values)
        VM.ScheduledShiftArray.Add(new SingleShiftData(i));
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

      DateTime targetDate = VM.ScheduledShiftArray[0].WorkDate.Date;
      var scheduledShifts = await ApiHolder.Api.UpdateSingleScheduledShiftListAsync(targetDate, VM.ScheduledShiftArray);

      if (!scheduledShifts.IsSuccess)
        if (MessageBox.Show("Error has occured\nErrorCode:" + scheduledShifts.ResultCode.ToString() + "\nRetry?", "ShiftManager", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
          UpdateScheduledShift();

      UpdateShiftSchedulingState(targetDate);
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
