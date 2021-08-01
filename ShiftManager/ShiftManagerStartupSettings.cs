using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace ShiftManager
{
  public interface IShiftManagerStartupSettings
  {
    /// <summary>ID入力フォームの初期値</summary>
    string ID { get; }

    /// <summary>PW入力フォームの初期値</summary>
    string PW { get; }

    /// <summary>サインアウトボタンを押下できるかどうか</summary>
    bool CanSignOut { get; }

    /// <summary>メニューの開閉を行うことができるかどうか</summary>
    bool CanOpenCloseMenu { get; }

    /// <summary>初期状態でメニューを開いた状態にしておくかどうか (true:開いた状態, false:閉じた状態)</summary>
    bool DefaultMenuState { get; }

    /// <summary>初期状態で表示するページ (未サインイン状態では, この上にサインインフォームが表示されます)</summary>
    PageList InitPage { get; }

    /// <summary>フルスクリーンに変更できるかどうか</summary>
    bool CanChangeToFullScreen { get; }

    /// <summary>フルスクリーンで起動するかどうか</summary>
    bool InitWithFullScreen { get; }

    /// <summary>動作モードを実行中に変更できるかどうか</summary>
    bool CanChangeRunningMode { get; }

    /// <summary>オフラインモードで起動するかどうか</summary>
    bool OfflineStart { get; }

    /// <summary>UIデバッグモードで起動するかどうか</summary>
    bool IsUIDebugMode { get; }
  }

  /// <summary>実行時引数で設定する引数の一覧</summary>
  public class ShiftManagerStartupSettings : IShiftManagerStartupSettings
  {
    /// <summary>ID入力フォームの初期値</summary>
    public string ID { get; private set; } = string.Empty;

    /// <summary>PW入力フォームの初期値</summary>
    public string PW { get; private set; } = string.Empty;

    /// <summary>サインアウトボタンを押下できるかどうか</summary>
    public bool CanSignOut { get; private set; } = true;

    /// <summary>メニューの開閉を行うことができるかどうか</summary>
    public bool CanOpenCloseMenu { get; private set; } = true;

    /// <summary>初期状態でメニューを開いた状態にしておくかどうか (true:開いた状態, false:閉じた状態)</summary>
    public bool DefaultMenuState { get; private set; } = true;

    /// <summary>初期状態で表示するページ (未サインイン状態では, この上にサインインフォームが表示されます)</summary>
    public PageList InitPage { get; private set; } = PageList.Home;

    /// <summary>フルスクリーンに変更できるかどうか</summary>
    public bool CanChangeToFullScreen { get; private set; } = true;

    /// <summary>フルスクリーンで起動するかどうか</summary>
    public bool InitWithFullScreen { get; private set; } = false;

    /// <summary>動作モードを実行中に変更できるかどうか</summary>
    public bool CanChangeRunningMode { get; private set; } = false;

    /// <summary>オフラインモードで起動するかどうか</summary>
    public bool OfflineStart { get; private set; } = false;

    /// <summary>UIデバッグモードで起動するかどうか</summary>
    public bool IsUIDebugMode { get; set; } = false;

    protected RootCommand rootCommand { get; } = new("Register your workday request, and create and update your work schedule and attendance log.")
    {
      GenOpt(nameof(ID), string.Empty, "Initial value of ID input form"),
      GenOpt(nameof(PW), string.Empty, "Initial value of Password input form"),
      GenOpt(nameof(CanSignOut), true, "Whether you can press the SignOut button."),
      GenOpt(nameof(CanOpenCloseMenu), true, "Whether you can open and close the menu."),
      GenOpt(nameof(DefaultMenuState), true, "Whether the menu is open initially (true: open, false: closed)"),
      GenOpt(nameof(InitPage), PageList.Home, "The page that will be displayed initially (the sign-in form will be displayed on top of this page when you are not signed in)."),
      GenOpt(nameof(CanChangeToFullScreen), true, "Whether you can change widnow state to full screen"),
      GenOpt(nameof(InitWithFullScreen), false, "Whether to boot in full-screen mode."),
      GenOpt(nameof(CanChangeRunningMode), false, "Whether you can change running mode betweeen offline and online."),
      GenOpt(nameof(OfflineStart), false, "Whether to boot in offline-mode."),
      GenOpt(nameof(IsUIDebugMode), false, "Whether to boot in ui-debug-mode."),
    };
    static Option<T> GenOpt<T>(in string propName, T defaultValue, in string description) => new("--" + propName, () => defaultValue, description);
    static Option<T> GenOpt<T>(in string[] propNameArr, T defaultValue, in string description)
    {
      List<string> slist = new();

      foreach (var s in propNameArr)
      {
        if (string.IsNullOrWhiteSpace(s))
          continue;

        slist.Add((s.Length == 1 ? "-" : "--") + s);
      }

      return new(slist.ToArray(), () => defaultValue, description);
    }

    /// <summary>文字列からプロパティを設定する</summary>
    /// <param name="str">実行時引数入力</param>
    public Task<int> SetFromString(string str)
    {
      rootCommand.Handler ??= CommandHandler.Create<string, string, bool, bool, bool, PageList, bool, bool, bool, bool, bool>(Set);
      return rootCommand.InvokeAsync(str);
    }

    /// <summary>文字列からプロパティを設定する</summary>
    /// <param name="str">実行時引数入力</param>
    public Task<int> SetFromStringArr(string[] str_arr)
    {
      rootCommand.Handler ??= CommandHandler.Create<string, string, bool, bool, bool, PageList, bool, bool, bool, bool, bool>(Set);
      return rootCommand.InvokeAsync(str_arr);
    }

    public void Set(string ID, string PW, bool CanSignOut, bool CanOpenCloseMenu, bool DefaultMenuState, PageList InitPage, bool CanChangeToFullScreen, bool InitWithFullScreen, bool CanChangeRunningMode, bool OfflineStart, bool IsUIDebugMode)
    {
      this.ID = ID;
      this.PW = PW;
      this.CanSignOut = CanSignOut;
      this.CanOpenCloseMenu = CanOpenCloseMenu;
      this.DefaultMenuState = DefaultMenuState;
      this.InitPage = InitPage;
      this.CanChangeToFullScreen = CanChangeToFullScreen;
      this.InitWithFullScreen = InitWithFullScreen;
      this.CanChangeRunningMode = CanChangeRunningMode;
      this.OfflineStart = OfflineStart;
      this.IsUIDebugMode = IsUIDebugMode;
    }
  }

  /// <summary>表示できるページの一覧</summary>
  public enum PageList
  {
    None,
    DataManage,
    Home,
    ScheduledShiftCheck,
    ScheduledShiftManage,
    ShiftPrint,
    ShiftRequestManage,

    // SignIn, //サインイン画面はUserControlなのでページではない

    SignUp,
    UserSetting,
    WorkLogCheck,
    WorkTimeRecord
  }
}
