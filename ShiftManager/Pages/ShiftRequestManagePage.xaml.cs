using System;
using System.Windows;
using System.Windows.Controls;

using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ShiftRequestManagePage.xaml
  /// </summary>
  public partial class ShiftRequestManagePage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    ScheduledShiftManagePageViewModel VM = new();
    public ShiftRequestManagePage()
    {
      InitializeComponent();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

    private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      for (int i = 0; i < 7; i++)
      {
        SingleShiftData ssdata = new(VM.ShiftRequestArray[i]);
        var res = ApiHolder.Api.AddShiftRequestAsync(ssdata);
        if (!res.Result.IsSuccess) { MessageBox.Show("データ送信に失敗しました"); }
      }
    }

    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => OnLoaded(null, null);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      VM.ShiftRequestArray.Clear();
      for (int i = 0; i < 7; i++)
      {
        DateTime targetDate = VM.TargetDate.Date.AddDays(i);
        VM.ShiftRequestArray.Add(new SingleShiftData(ApiHolder.CurrentUserID, targetDate, false, targetDate, targetDate, new()));
      }
    }
  }
}
