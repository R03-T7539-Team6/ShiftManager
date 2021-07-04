using System.Windows.Controls;
using System;
using System.Windows.Threading;
using ShiftManager.DataClasses;
using ShiftManager.Communication;
using System.Windows;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for WorkTimeRecordPage.xaml
  /// </summary>
  public partial class WorkTimeRecordPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    ScheduledShiftManagePageViewModel VM = new();
    public WorkTimeRecordPage()
    {
      InitializeComponent();
      DispatcherTimer timer = new DispatcherTimer();
      timer.Tick += timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 1);
      timer.Start();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

    /*******************************************
    * specification ;
    * name = timer_Tick ;
    * Function = 時間を更新し続ける ;
    * note = 補足説明 ;
    * date = 06/26/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 計測が始まることを知らせるイベントハンドラ ;
    * output = 現在時刻 ;
    * end of specification ;
    *******************************************/
    private void timer_Tick(object sender, EventArgs e)
    {
      DateTime d = DateTime.Now;
      time.Text = string.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);
    }

    /*******************************************
    * specification ;
    * name = syukkin_Click ;
    * Function = 出勤ボタンが押された時に出勤時間を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 出勤ボタンが押されたことを知らせるイベントハンドラ ;
    * output = 実行結果 ;
    * end of specification ;
    *******************************************/
    private async void syukkin_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        string userID = UID.Text;
        UserID targetUserID = new(userID);
        ApiResult<DateTime> res = await ApiHolder.Api.DoWorkStartTimeLoggingAsync(targetUserID);
        if (res.ResultCode == ApiResultCodes.Work_Not_Ended)
          MessageBox.Show("まだ勤務中です");
        if (res.ResultCode == ApiResultCodes.Success)
        {
          MessageBox.Show("出勤登録完了");
          VM.ShiftRequestArray.Clear();
          main(targetUserID);
        }
        if (res.ResultCode == ApiResultCodes.UserID_Not_Found)
          MessageBox.Show("IDが違います");
      }
    }

    /*******************************************
    * specification ;
    * name = kyunyu_Click ;
    * Function = 休入ボタンが押された時に休入時間を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 休入ボタンが押されたことを知らせるイベントハンドラ ;
    * output = 実行結果 ;
    * end of specification ;
    *******************************************/
    private async void kyunyu_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        string userID = UID.Text;
        UserID targetUserID = new(userID);
        ApiResult<DateTime> res = await ApiHolder.Api.DoBreakTimeStartLoggingAsync(targetUserID);
        if (res.ResultCode == ApiResultCodes.UserID_Not_Found)
          MessageBox.Show("IDが違います");
        if (res.ResultCode == ApiResultCodes.Work_Not_Started)
          MessageBox.Show("出勤していません");
        if (res.ResultCode == ApiResultCodes.BreakTime_Not_Ended)
          MessageBox.Show("休憩中です");
        if (res.ResultCode == ApiResultCodes.Success)
        {
          MessageBox.Show("休憩開始");
          VM.ShiftRequestArray.Clear();
          main(targetUserID);
        }
        }
    }

    /*******************************************
    * specification ;
    * name = kyusyutu_Click_1 ;
    * Function = 休出ボタンが押された時に休出時間を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 休出ボタンが押されたことを知らせるイベントハンドラ ;
    * output = 実行結果 ;
    * end of specification ;
    *******************************************/
    private async void kyusyutu_Click_1(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        string userID = UID.Text;
        UserID targetUserID = new(userID);
        ApiResult<DateTime> res =  await ApiHolder.Api.DoBreakTimeEndLoggingAsync(targetUserID);
        if(res.ResultCode == ApiResultCodes.Work_Not_Started)
          MessageBox.Show("出勤記録がありません");
        if (res.ResultCode == ApiResultCodes.BreakTime_Not_Started)
          MessageBox.Show("休憩記録がありません");
        if (res.ResultCode == ApiResultCodes.BreakTimeLen_Zero_Or_Less)
          MessageBox.Show("休憩時間が短すぎます");
        if (res.ResultCode == ApiResultCodes.Success)
        {
          MessageBox.Show("休憩時間終了");
          VM.ShiftRequestArray.Clear();
          main(targetUserID);
        }
        if (res.ResultCode == ApiResultCodes.UserID_Not_Found)
          MessageBox.Show("IDが違います");
      }
    }

    /*******************************************
    * specification ;
    * name = taikin_Click_1 ;
    * Function = 退勤ボタンが押された時に退勤時間を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 退勤ボタンが押されたことを知らせるイベントハンドラ ;
    * output = 実行結果 ;
    * end of specification ;
    *******************************************/
    private async void taikin_Click_1(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        string userID = UID.Text;
        UserID targetUserID = new(userID);
        ApiResult<DateTime> res = await ApiHolder.Api.DoWorkEndTimeLoggingAsync(targetUserID);
        if (res.ResultCode == ApiResultCodes.UserID_Not_Found)
          MessageBox.Show("IDが違います");
        if (res.ResultCode == ApiResultCodes.Work_Not_Started)
          MessageBox.Show("出勤していません");
        if (res.ResultCode == ApiResultCodes.BreakTime_Not_Ended)
          MessageBox.Show("休憩中です");
        if (res.ResultCode == ApiResultCodes.Success)
        {
          MessageBox.Show("退勤登録完了");
          VM.ShiftRequestArray.Clear();
          main(targetUserID);
        }
      }
    }

    /*******************************************
    * specification ;
    * name = main ;
    * Function = ボタンが押された時に勤怠実績を表示する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = ユーザID ;
    * output = 勤怠実績 ;
    * end of specification ;
    *******************************************/
    public async void main(UserID userID)
    {
      DateTime selectday = DateTime.Today;
      ApiResult<SingleShiftData> res = await ApiHolder.Api.GetScheduledShiftByIDAsync(selectday, userID);
        if (!res.IsSuccess)
        {
          MessageBox.Show("データ取得に失敗しました");
        }
        else
        {
          VM.ShiftRequestArray.Add(res.ReturnData);
        }
      
    }
  }
}
