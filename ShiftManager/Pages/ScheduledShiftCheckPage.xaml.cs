using System.Windows.Controls;
using System;
using ShiftManager.DataClasses;
using ShiftManager.Communication;
using System.Windows;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftCheckPage.xaml
  /// </summary>
  public partial class ScheduledShiftCheckPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    ScheduledShiftManagePageViewModel VM = new();
    public ScheduledShiftCheckPage()
    {
      InitializeComponent();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

    public async void hoge()
    {
      DateTime selectday = VM.TargetDate.Date;
      for (int i = 0; i < 7; i++)
      {
        ApiResult<SingleShiftData> res = await ApiHolder.Api.GetScheduledShiftByIDAsync(selectday.AddDays(i), ApiHolder.Api.CurrentUserData?.UserID);
        if (!res.IsSuccess)
        {
          break;
        }
        else
        {
          VM.ShiftRequestArray.Add(res.ReturnData);
        }
      }
    }

    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => OnLoaded(null, null);

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
      VM.ShiftRequestArray.Clear();
      hoge();
    }
  }
}
