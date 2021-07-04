using System;
using System.Collections.Generic;
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
    public ScheduledShiftManagePage()
    {
      InitializeComponent();

      VM.ShiftRequestArray = new();
      VM.ScheduledShiftArray = new();
      DataContext = VM;
    }

    ImmutableArray<ShiftRequest> ShiftRequests;
    private void Page_Loaded(object sender, RoutedEventArgs e) => ReloadData();
    private void ReloadData()
    {
      VM.ApiHolder = ApiHolder;
      ReloadShiftRequest();
      ReloadScheduledShift();
    }
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

    private async void ReloadScheduledShift()
    {
      var ret = await ApiHolder.Api.GetScheduledShiftByDateAsync(VM.TargetDate);
      if (!ret.IsSuccess) {
        if (ret.ResultCode == Communication.ApiResultCodes.Target_Date_Not_Found)
          ret = await ApiHolder.Api.GenerateScheduledShiftAsync(VM.TargetDate);
        else
        {
          MessageBox.Show("Error has occured\nErrorCode:" + ret.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }
      }
      VM.ScheduledShiftArray.Clear();

      foreach (var i in ret.ReturnData.ShiftDictionary.Values)
        VM.ScheduledShiftArray.Add(new SingleShiftData(i));
    }

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
    private void UpdateShiftSchedulingState(DateTime targetDate)
    {
      //実装準備状態とする
      /*var stateUpdateResult = await ApiHolder.Api.UpdateShiftSchedulingStateAsync(targetDate, VM.ShiftSchedulingState);

      if (!stateUpdateResult.IsSuccess)
        if (MessageBox.Show("Error has occured\nErrorCode:" + stateUpdateResult.ResultCode.ToString() + "\nRetry?", "ShiftManager", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
          UpdateShiftSchedulingState(targetDate);*/
    }
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ReloadData();

    private void ListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) => ScrollSyncer(sender);

    private void ShiftRequestListView_MouseWheel(object sender, MouseWheelEventArgs e) => ScrollSyncer(sender);

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
