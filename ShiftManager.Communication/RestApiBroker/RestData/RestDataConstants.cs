namespace ShiftManager.Communication.RestApiBroker.RestData
{
  public static class RestDataConstants
  {
    public static class Status
    {
      public static readonly string Normal = "Normal";
      public static readonly string InLeaveOfAbsence = "InLeaveOfAbsence";
      public static readonly string Retired = "Retired";
      public static readonly string NotHired = "NotHired";
      public static readonly string Others = "Others";
    }

    public static class Group
    {
      public static readonly string None = "None";
      public static readonly string SystemAdmin = "SystemAdmin";
      public static readonly string SuperUser = "SuperUser";
      public static readonly string NormalUser = "NormalUser";
      public static readonly string ForTimeRecordTerminal = "ForTimeRecordTerminal";
    }

    public static class ShiftStatus
    {
      public static readonly string NoneShift = "None";
      public static readonly string NotStarted = "NotStarted";
      public static readonly string Working = "Working";
      public static readonly string FinalVersion = "FinalVersion";
    }
  }
}
