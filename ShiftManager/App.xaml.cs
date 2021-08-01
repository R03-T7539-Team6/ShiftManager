using System.Windows;

namespace ShiftManager
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static IShiftManagerStartupSettings ShiftManagerStartupSettings { get; private set; }
    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      ShiftManagerStartupSettings settings = new();
      var ret = await settings.SetFromStringArr(e.Args);
      ShiftManagerStartupSettings = settings;
    }
  }

}
