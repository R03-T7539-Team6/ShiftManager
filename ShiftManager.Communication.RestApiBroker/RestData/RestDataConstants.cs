using ShiftManager.DataClasses;

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

      public static UserState ConvToValue(in string value) => value switch
      {
        Normal => UserState.Normal,
        InLeaveOfAbsence => UserState.InLeaveOfAbsence,
        Retired => UserState.Retired,
        NotHired => UserState.NotHired,
        _ => UserState.Others
      };
    }

    public static class Group
    {
      public const string None = "None";
      public const string SystemAdmin = "SystemAdmin";
      public const string SuperUser = "SuperUser";
      public const string NormalUser = "NormalUser";
      public const string ForTimeRecordTerminal = "ForTimeRecordTerminal";

      public static UserGroup ConvToValue(in string value) => value switch
      {
        SystemAdmin => UserGroup.SystemAdmin,
        SuperUser => UserGroup.SuperUser,
        NormalUser => UserGroup.NormalUser,
        ForTimeRecordTerminal => UserGroup.ForTimeRecordTerminal,
        _ => UserGroup.None
      };
    }

    public static class ShiftStatus
    {
      public const string NoneShift = "None";
      public const string NotStarted = "NotStarted";
      public const string Working = "Working";
      public const string FinalVersion = "FinalVersion";

      public static ShiftSchedulingState ConvToValue(in string value) => value switch
      {
        NotStarted => ShiftSchedulingState.NotStarted,
        Working => ShiftSchedulingState.Working,
        FinalVersion => ShiftSchedulingState.FinalVersion,
        _ => ShiftSchedulingState.None
      };
    }
  }
}
