using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record ShiftRequest(IUserID UserID, DateTime LastUpdate, Dictionary<DateTime, ISingleShiftData> RequestsDictionary) : IShiftRequest
  {
    public ShiftRequest(IShiftRequest i) : this(i?.UserID ?? new UserID(), i?.LastUpdate ?? new(), i?.RequestsDictionary ?? new()) { }
  }

  public partial class ShiftRequest_NotifyPropertuChanged : IShiftRequest, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID();
    [AutoNotify]
    private DateTime _LastUpdate = new();
    [AutoNotify]
    private Dictionary<DateTime, ISingleShiftData> _RequestsDictionary = new();
  }

  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface IShiftRequest
  {
    IUserID UserID { get; }
    DateTime LastUpdate { get; }
    Dictionary<DateTime, ISingleShiftData> RequestsDictionary { get; }
  }
}
