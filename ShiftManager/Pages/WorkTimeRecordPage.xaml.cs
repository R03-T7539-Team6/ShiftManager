using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

using AutoNotify;
using Reactive.Bindings;
using System.Collections.Generic;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for WorkTimeRecordPage.xaml
  /// </summary>
  public partial class WorkTimeRecordPage : Page, IContainsApiHolder, IContainsIsProcessing
  {
    public IApiHolder ApiHolder { get; set; }
    public ReactivePropertySlim<bool> IsProcessing { get; set; }
    void TurnOnProcessingSW()
    {
      if (IsProcessing is not null)
        IsProcessing.Value = true;
    }
    void TurnOffProcessingSW()
    {
      if (IsProcessing is not null)
        IsProcessing.Value = false;
    }

    WorkTimeRecordPageViewModel VM = new();
    public WorkTimeRecordPage()
    {
      InitializeComponent();

      DispatcherTimer timer = new DispatcherTimer();
      timer.Tick += timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 1);
      timer.Start();
      
      DataContext = VM;
      timer_Tick(default, EventArgs.Empty);
    }

    private void ClearIDBox() => UID.Text = string.Empty;

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
    private void timer_Tick(object sender, EventArgs e) => time.Text = $"{DateTime.Now:HH:mm:ss}";
    

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
    private async void syukkin_Click(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        TurnOnProcessingSW();

        string userID = UID.Text;
        UserID targetUserID = new(userID);

        ApiResult<DateTime> res = await ApiHolder.Api.DoWorkStartTimeLoggingAsync(targetUserID);
        
        switch (res.ResultCode)
        {
          case ApiResultCodes.Work_Not_Ended:
            _ = MessageBox.Show("まだ勤務中です");
            break;

          case ApiResultCodes.UserID_Not_Found:
            _ = MessageBox.Show("IDが違います");
            break;

          case ApiResultCodes.Success:
            _ = MessageBox.Show("出勤登録完了");
            ClearIDBox();

            main(targetUserID);

            WorkLogSetter(userID, DateTime.Today, v => v with { AttendanceTime = res.ReturnData });
            break;

          default:
            _ = MessageBox.Show("不明なエラーが発生しました\n" + res.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
            break;
        }

        TurnOffProcessingSW();
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
    private async void kyunyu_Click(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        TurnOnProcessingSW();

        string userID = UID.Text;
        UserID targetUserID = new(userID);

        ApiResult<DateTime> res = await ApiHolder.Api.DoBreakTimeStartLoggingAsync(targetUserID);

        switch (res.ResultCode)
        {
          case ApiResultCodes.UserID_Not_Found:
            _ = MessageBox.Show("IDが違います");
            break;

          case ApiResultCodes.Work_Not_Started:
            _ = MessageBox.Show("出勤していません");
            break;

          case ApiResultCodes.BreakTime_Not_Ended:
            _ = MessageBox.Show("休憩中です");
            break;

          case ApiResultCodes.Success:
            _ = MessageBox.Show("休憩開始");
            ClearIDBox();

            main(targetUserID);

            WorkLogSetter(userID, DateTime.Today, (v) =>
            {
              v.BreakTimeDictionary[res.ReturnData] = 0; //これでも追加できるらしいので
              return new SingleShiftData(v);
            });
            break;

          default:
            _ = MessageBox.Show("不明なエラーが発生しました\n" + res.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
            break;
        }

        TurnOffProcessingSW();
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
    private async void kyusyutu_Click_1(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        TurnOnProcessingSW();

        string userID = UID.Text;
        UserID targetUserID = new(userID);

        ApiResult<DateTime> res = await ApiHolder.Api.DoBreakTimeEndLoggingAsync(targetUserID);

        switch (res.ResultCode)
        {
          case ApiResultCodes.UserID_Not_Found:
            _ = MessageBox.Show("IDが違います");
            break;

          case ApiResultCodes.Work_Not_Started:
            _ = MessageBox.Show("出勤していません");
            break;

          case ApiResultCodes.BreakTime_Not_Started:
            _ = MessageBox.Show("休憩記録がありません");
            break;

          case ApiResultCodes.BreakTimeLen_Zero_Or_Less:
            _ = MessageBox.Show("休憩時間が短すぎます");
            break;

          case ApiResultCodes.Success:
            MessageBox.Show("休憩時間終了");
            ClearIDBox();

            main(targetUserID);

            WorkLogSetter(userID, DateTime.Today, (v) =>
            {
              DateTime break_in_time = v.BreakTimeDictionary.LastOrDefault().Key;
              if(break_in_time == default)
              {
                _ = MessageBox.Show("休憩開始時刻の取得に失敗しました.\nサーバ上のデータは正常に更新されていますが, 画面上には休憩時間が反映されません.", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
                return v;
              }
              v.BreakTimeDictionary[break_in_time] = (int)(res.ReturnData - break_in_time).TotalMinutes;
              return new SingleShiftData(v);
            });
            break;

          default:
            _ = MessageBox.Show("不明なエラーが発生しました\n" + res.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
            break;
        }
      }

      TurnOffProcessingSW();
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
    private async void taikin_Click_1(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(UID.Text))
      {
        TurnOnProcessingSW();

        string userID = UID.Text;
        UserID targetUserID = new(userID);

        ApiResult<DateTime> res = await ApiHolder.Api.DoWorkEndTimeLoggingAsync(targetUserID);

        switch (res.ResultCode)
        {
          case ApiResultCodes.UserID_Not_Found:
            _ = MessageBox.Show("IDが違います");
            break;

          case ApiResultCodes.Work_Not_Started:
            _ = MessageBox.Show("出勤していません");
            break;

          case ApiResultCodes.BreakTime_Not_Ended:
            _ = MessageBox.Show("休憩中です");
            break;

          case ApiResultCodes.Success:
            _ = MessageBox.Show("退勤登録完了");
            ClearIDBox();

            main(targetUserID);

            WorkLogSetter(userID, DateTime.Today, v => v with { LeavingTime = res.ReturnData });
            break;

          default:
            _ = MessageBox.Show("不明なエラーが発生しました\n" + res.ResultCode.ToString(), "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
            break;
        }

        TurnOffProcessingSW();
      }
    }

    public void main(UserID userID)
    {
      /*
      DateTime selectday = DateTime.Today;

      ApiResult<SingleShiftData> res = await ApiHolder.Api.GetScheduledShiftByIDAsync(selectday, userID);

      if (!res.IsSuccess)
        MessageBox.Show("データ取得に失敗しました");
      else
        VM.ScheduledShiftArray.Add(res.ReturnData);
      */
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      TurnOnProcessingSW();
      DateTime today = DateTime.Today;

      //ユーザーリスト取得
      await ApiHolder.Api.GetAllUserAsync(); //(こっちでは使用しない)

      //今日の予定シフトを表示する

      var res = await ApiHolder.Api.GetScheduledShiftByDateAsync(today);

      if(res.ReturnData is not null)
      {
        VM.ScheduledShiftArray.Clear();
        List<ISingleShiftData> list = new();
        foreach (var i in res.ReturnData.ShiftDictionary.Values)
          list.Add(i);

        list.Sort((i1, i2) => DateTime.Compare(i1.AttendanceTime, i2.AttendanceTime));

        foreach (var i in list)
          VM.ScheduledShiftArray.Add(i);
      }

      //今日の勤怠実績があれば表示する => 未対応
      
      TurnOffProcessingSW();
    }

    private bool WorkLogSetter(string userID, DateTime targetDate, Func<SingleShiftData, ISingleShiftData> updater)
    {
      if (string.IsNullOrWhiteSpace(userID) || targetDate == default || updater is null)
        return false; //無効値処理

      var arr = VM.WorkLogArray; //VM.WorkLogArrって長いので

      if (arr.Count > 0)
        for (int i = 0; i < arr.Count; i++)
        {
          if(arr[i].UserID.Value == userID) //UserIDが一致した場合のみ, 値の更新を行う  一日に2回の打刻は考慮しない
          {
            arr[i] = updater.Invoke(new(arr[i]));
            return true;
          }
        }

      // WorkLogArrayが空だったか, あるいは指定のユーザのデータが存在しなかった
      SingleShiftData ssd = new(new UserID(userID), targetDate, false, targetDate, targetDate, new());
      arr.Add(updater.Invoke(ssd));
      return true;
    }
  }

  public partial class WorkTimeRecordPageViewModel : INotifyPropertyChanged, IContainsApiHolder
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _ScheduledShiftArray = new();

    [AutoNotify]
    private ObservableCollection<ISingleShiftData> _WorkLogArray = new();

    public IApiHolder ApiHolder { get; set; }
  }
}
