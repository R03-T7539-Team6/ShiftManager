namespace ShiftManager.Communication.RestData
{
  public static class RestDataConstants
  {
    public static class Status
    {
      public const string Normal = "Normal";
      public const string InLeaveOfAbsence = "InLeaveOfAbsence";
      public const string Retired = "Retired";
      public const string NotHired = "NotHired";
      public const string Others = "Others";
    }

    public static class Group
    {
      public const string None = "None";
      public const string SystemAdmin = "SystemAdmin";
      public const string SuperUser = "SuperUser";
      public const string NormalUser = "NormalUser";
      public const string ForTimeRecordTerminal = "ForTimeRecordTerminal";
    }

    public static class ShiftStatus
    {
      public const string NoneShift = "None";
      public const string NotStarted = "NotStarted";
      public const string Working = "Working";
      public const string FinalVersion = "FinalVersion";
    }
  }
}
