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
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
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
      VM.ScheduledShiftArray.Clear();
      VM.ShiftRequestArray.Clear();
      for (int v = 0; v < 10; v++)
        foreach (var i in ShiftRequests)
        {
          if (!i.RequestsDictionary.TryGetValue(VM.TargetDate, out ISingleShiftData ssd))
            ssd = new SingleShiftData(i.UserID, VM.TargetDate, false, default, default, new());
          VM.ShiftRequestArray.Add(ssd);
          VM.ScheduledShiftArray.Add(new SingleShiftData(ssd));
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
      //VM.ScheduledShiftArray = new();
      for (int v = 0; v < 10; v++)
        foreach (var i in ret.ReturnData.ShiftDictionary.Values)
        {
          VM.ScheduledShiftArray.Add(new SingleShiftData(i));
        }
      
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
      if (srlvScrollProvider.VerticalScrollPercent != scrollPercent)
        srlvScrollProvider.SetScrollPercent(ScrollPatternIdentifiers.NoScroll, scrollPercent);

      var sslvScrollProvider = GetScrollProvider(ScheduledShiftListView);
      if (sslvScrollProvider.VerticalScrollPercent != scrollPercent)
        sslvScrollProvider.SetScrollPercent(ScrollPatternIdentifiers.NoScroll, scrollPercent);
    }
  }

  public partial class ScheduledShiftManagePageViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _ShiftRequestArray;

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _ScheduledShiftArray;

    [AutoNotify]
    private DateTime _TargetDate = DateTime.Now.Date;
  }
}
