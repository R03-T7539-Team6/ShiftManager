using System;

namespace ShiftManager.Communication
{
  public class SharedFuncs
  {
    /// <summary>休憩時刻の長さを計算します</summary>
    /// <param name="start">休憩開始時刻</param>
    /// <param name="end">休憩終了時刻</param>
    /// <returns>休憩時間長 [min]</returns>
    /*******************************************
  * specification ;
  * name = GetBreakTimeLength ;
  * Function = 休憩時間長を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 開始時間, 終了時間 ;
  * output = 休憩時間長 [分] ;
  * end of specification ;
  *******************************************/
    static public int GetBreakTimeLength(in DateTime start, in DateTime end)
      => (int)(new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 0) - new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0)).TotalMinutes;
  }
}
