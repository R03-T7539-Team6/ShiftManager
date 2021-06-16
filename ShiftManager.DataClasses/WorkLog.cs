using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record WorkLog(UserID UserID, SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary);


}

qwertyuiop