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
    private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      for (int i = 0; i < 7; i++)
      {
        SingleShiftData ssdata = new(VM.ShiftRequestArray[i]);
        var res = ApiHolder.Api.AddShiftRequestAsync(ssdata);
        if (!res.Result.IsSuccess) { MessageBox.Show("データ送信に失敗しました"); }
      }
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
