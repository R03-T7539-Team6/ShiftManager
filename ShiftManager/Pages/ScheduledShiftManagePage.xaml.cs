using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
      VM.ShiftRequestArray = new();
      foreach (var i in ShiftRequests)
      {
        if (!i.RequestsDictionary.TryGetValue(VM.TargetDate, out ISingleShiftData ssd))
          ssd = new SingleShiftData(i.UserID, VM.TargetDate, false, default, default, new());
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
      VM.ScheduledShiftArray = new();
      foreach (var i in ret.ReturnData.ShiftDictionary.Values)
        VM.ScheduledShiftArray.Add(new SingleShiftData(i));
      
    }
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
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
