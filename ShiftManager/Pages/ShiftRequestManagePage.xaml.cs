using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Reactive.Bindings;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ShiftRequestManagePage.xaml
  /// </summary>
  public partial class ShiftRequestManagePage : Page, IContainsApiHolder, IContainsIsProcessing
  {
    const int DayPerPage = 28;
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    ScheduledShiftManagePageViewModel VM = new();
    public ShiftRequestManagePage()
    {
      InitializeComponent();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

    /*******************************************
* specification ;
* name = DatePicker_SelectedDateChanged ;
* Function = 選択する日付が変更された時にシフト希望の内容を更新する ;
* note = 補足説明 ;
* date = 07/03/2021 ;
* author = 佐藤真通 ;
* History = 更新履歴 ;
* input = 保存ボタンが押されたことを知らせるイベントハンドラ ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async void Save_Click(object sender, RoutedEventArgs e)
    {
      bool isSuccess = true;

      if (IsProcessing is not null)
        IsProcessing.Value = true;

      List<Task<ApiResult>> list = new();

      for (int i = 0; i < DayPerPage; i++)
        list.Add(ApiHolder.Api.AddShiftRequestAsync(VM.ShiftRequestArray[i]));

      var results = await Task.WhenAll(list);
      List<ApiResultCodes> ErrorCodes = new();
      foreach (var i in results)
      {
        isSuccess &= i.IsSuccess;
        if (!ErrorCodes.Contains(i.ResultCode))
          ErrorCodes.Add(i.ResultCode);
      }

      if (!isSuccess)
      {
        StringBuilder ErrorCodesString = new();
        foreach (var i in ErrorCodes)
          _ = ErrorCodesString.Append(i).AppendLine();

        _ = MessageBox.Show("送信に失敗しました\n" + ErrorCodesString.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      if (IsProcessing is not null)
        IsProcessing.Value = false;
    }

    /*******************************************
    * specification ;
    * name = DatePicker_SelectedDateChanged ;
    * Function = 選択する日付が変更された時にシフト希望の内容を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 選択する日付が変わったことを知らせるイベントハンドラ ;
    * output = N/A ;
    * end of specification ;
    *******************************************/
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => OnLoaded(null, null);

    bool DataLoadingCompleted { get; set; } = true;
    public ReactivePropertySlim<bool> IsProcessing { get; set; }

    /*******************************************
    * specification ;
    * name = OnLoaded ;
    * Function = 画面がロードされた時に予定シフト表の内容を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 画面がロードされたことを知らせるイベントハンドラ ;
    * output = N/A ;
    * end of specification ;
    *******************************************/
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (!DataLoadingCompleted)
        return;
      DataLoadingCompleted = false;

      if (IsProcessing is not null)
        IsProcessing.Value = true;

      VM.ShiftRequestArray.Clear();
      for (int i = 0; i < DayPerPage; i++)
      {
        DateTime targetDate = VM.TargetDate.Date.AddDays(i);
        VM.ShiftRequestArray.Add(new SingleShiftData(ApiHolder.CurrentUserID, targetDate, false, targetDate, targetDate, new()));
      }

      List<Task<ApiResult<SingleShiftData>>> list = new();

      for (int i = 0; i < DayPerPage; i++)
      {
        DateTime targetDate = VM.TargetDate.Date.AddDays(i);
        list.Add(ApiHolder.Api.GetShiftRequestByDateAsync(targetDate));
      }

      var results = await Task.WhenAll(list);

      for (int i = 0; i < DayPerPage; i++)
        if (results[i].ReturnData is not null)
          VM.ShiftRequestArray[i] = results[i].ReturnData;

      DataLoadingCompleted = true;

      if (IsProcessing is not null)
        IsProcessing.Value = false;
    }
  }
}
