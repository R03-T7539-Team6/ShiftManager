﻿using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record ShiftRequest(UserID UserID, DateTime LastUpdate, Dictionary<DateTime, SingleShiftData> RequestsDictionary);
}