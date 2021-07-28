using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

using Reactive.Bindings;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IContainsGetOnlyIsProcessing
  {
    public ApiHolder ApiHolder { get; set; } = new ApiHolder() { Api = new RestApiBroker() };

    private MainWindowViewModel MWVM { get; }
    public ReactivePropertySlim<bool> IsProcessing => MWVM?.IsProcessing;

    static readonly double BlurRadiusWhenSignOut = 10;

    static readonly double BlurRadiusWhenSignOut = 10;

    enum MenuState
    {
      Opened,
      Closing,
      Closed,
      Opening
    }

    MenuState CurrentMenuState { get; set; } = MenuState.Opened;
    Storyboard OpenMenuStoryboard { get; set; }
    Storyboard CloseMenuStoryboard { get; set; }

    /*******************************************
* specification ;
* name = MainWindow ;
* Function = MainWindowのインスタンスを初期化します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = ListViewインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    public MainWindow()
    {
      InitializeComponent();

#if DEBUG
      if (TryFindResource("WindowStyleToDebug") is Style s)
        this.Style = s;
#endif

      MWVM = new() { MainFramePageChanger = new(MainFrame) { IsProcessingInstance = this } };
      DataContext = MWVM;
      SignInPageElem.ApiHolder = this.ApiHolder;
      SignInPageElem.IsProcessing = IsProcessing;

      MWVM.BlurRadius.Value = MWVM.IsSignedIn.Value ? 0 : BlurRadiusWhenSignOut;
    }

    /// <summary>SignInがSuccessした際に実行される</summary>
    /// <param name="sender">呼び出し元</param>
    /// <param name="e">イベント引数</param>
    /*******************************************
* specification ;
* name = SignInPageElem_Login ;
* Function = サインイン成功時に実行され, 表示の更新を行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = SignInコントロールインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void SignInPageElem_Login(object sender, EventArgs e)
    {
      MWVM.IsSignedIn.Value = true;
      MWVM.UserName.Value = new(ApiHolder.CurrentUserName);
      MWVM.BlurRadius.Value = 0;
    }

    /*******************************************
* specification ;
* name = MainFrame_Navigating ;
* Function = ページ移動時に呼び出され, API参照用インスタンスをセットします ;
* note = N/A ;
* date = 06/29/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Frameのインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      if (MWVM?.IsSignedIn is not null)
        MWVM.IsProcessing.Value = true;

      if (e.Content is IContainsApiHolder i)
        i.ApiHolder = ApiHolder;


      if (MWVM?.IsProcessing is not null && e.Content is IContainsIsProcessing isProcessing)
        isProcessing.IsProcessing = MWVM.IsProcessing; //ページから処理中表示を出すため
    }
    private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
      if (MWVM?.IsProcessing is not null && e.Content is not IContainsIsProcessing)
        MWVM.IsProcessing.Value = false; //向こうに処理完了を通知する実装が無い場合のみ, ここで処理完了したことにする
    }

    /*******************************************
* specification ;
* name = SignOutClicked ;
* Function = サインアウトボタン押下時に実行され, サインアウト処理を実行します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Buttonのインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async void SignOutClicked(object sender, RoutedEventArgs e)
    {
      MWVM.IsProcessing.Value = true;

      _ = await ApiHolder.Api.SignOutAsync();
      MWVM.IsSignedIn.Value = false;

      MWVM.BlurRadius.Value = BlurRadiusWhenSignOut;

      MWVM.IsProcessing.Value = false;
    }

    /*******************************************
    * specification ;
    * name = LicenseClicked ;
    * Function = ライセンスボタンがクリックされた時にライセンス情報のテキストフォルダを読み込んでサブウィンドウに渡す ;
    * note = 補足説明 ;
    * date = 07/04/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = ライセンスボタンが押されたことを知らせるイベントハンドラ ;
    * output = N/A ;
    * end of specification ;
    *******************************************/
    private void LicenseClicked(object sender, RoutedEventArgs e)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "ShiftManager.Resources.ThirdPartyLicense.md";
      string manualFileContent;
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream != null)
        {
          using (var sr = new StreamReader(stream))
          {
            manualFileContent = sr.ReadToEnd();
            var window = new Window1(manualFileContent);
            window.Show();
          }
        }
      }
    }

    private void MenuOpenCloseClicked(object sender, RoutedEventArgs e)
    {
      switch (CurrentMenuState)
      {
        case MenuState.Opened:
          CurrentMenuState = MenuState.Closing;
          CloseMenuStoryboard?.Begin();
          break;

        case MenuState.Closing:
          CloseMenuStoryboard?.Stop();

          CurrentMenuState = MenuState.Opening;
          OpenMenuStoryboard?.Begin();
          break;

        case MenuState.Closed:
          MenuGrid.Visibility = Visibility.Visible;
          CurrentMenuState = MenuState.Opening;
          OpenMenuStoryboard?.Begin();
          break;

        case MenuState.Opening:
          OpenMenuStoryboard?.Stop();

          CurrentMenuState = MenuState.Closing;
          CloseMenuStoryboard?.Begin();
          break;
      }
    }

    private void TitleStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (CloseMenuStoryboard is not null && sender is FrameworkElement elem)
        foreach (var i in CloseMenuStoryboard?.Children)
          if (i is ThicknessAnimation ta)
            ta.To = new(-1 * elem.ActualWidth - 20, 0, 0, 0);
    }

    private void CloseMenu_Completed(object sender, EventArgs e)
    {
      MenuGrid.Visibility = Visibility.Collapsed;
      CurrentMenuState = MenuState.Closed;
    }
    private void OpenMenu_Completed(object sender, EventArgs e) => CurrentMenuState = MenuState.Opened;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (FindResource(nameof(OpenMenuStoryboard) + "Key") is Storyboard oms)
        OpenMenuStoryboard = oms;

      if (FindResource(nameof(CloseMenuStoryboard) + "Key") is Storyboard cms)
        CloseMenuStoryboard = cms;

      TitleStackPanel_SizeChanged(TitleStackPanel, null);
    }

    private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    WindowState lastWindowState = WindowState.Normal;
    DateTime lastF12KeyDown = default;
    int F12KeyDownCount = 0;
    const int F12KeyDownMax = 5;
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.F11: //全画面/解除
          if(WindowStyle == WindowStyle.None) //おそらく全画面状態である
          {
            //全画面解除
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = lastWindowState;
          }
          else //おそらく通常表示状態である
          {
            lastWindowState = WindowState;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
          }
          break;

        case Key.F12:
          /* lastDown : 00:00:00.000 (+0.5 => 00:00:00.500)
           * Current1 : 00:00:00.300 => OK ( < 00:00:00.500)
           * Current2 : 00:00:01.000 => Out ( > 00:00:00.500)
           */
          if (lastF12KeyDown.AddMilliseconds(500) < DateTime.Now)
          {
            F12KeyDownCount = 0;
            Title = Title.TrimEnd('.'); //回数確認用に付加した末尾の「.」を除去する
          }
          lastF12KeyDown = DateTime.Now;

          F12KeyDownCount++;

          //押下回数が規定解数を超えたら, APIモード切替を行う
          if (F12KeyDownCount >= F12KeyDownMax)
          {
            //現在のモードがオンライン動作モードである
            if (ApiHolder.Api is RestApiBroker)
            {
              //切替確認
              if (MessageBox.Show("開発者機能 : 動作モード切替\n現在オンライン動作モードで動作中です.  オフライン動作モードに切り替えますか?", "ShiftManager (Developer Function)", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
              {
                ApiHolder.Api = new InternalApi();
                Title = "ShiftManager (Offline Mode)";
              }
              else
                return;
            }
            else if (ApiHolder.Api is InternalApi)
            {
              //切替確認
              if (MessageBox.Show("開発者機能 : 動作モード切替\n現在オフライン動作モードで動作中です.  オンライン動作モードに切り替えますか?", "ShiftManager (Developer Function)", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
              {
                ApiHolder.Api = new RestApiBroker();
                Title = "ShiftManager (Online Mode)";
              }
              else
                return;
            }
            else
              return;

            SignOutClicked(this, null); //モード切替後は再度サインインが必要
          }
          else
          {
            Title += "."; //回数チェック用
          }

          break;
      }
    }
  }

  internal class MainWindowViewModel
  {
    public FramePageChanger MainFramePageChanger { get; init; }
    public ReactivePropertySlim<bool> IsMenuExpanded { get; } = new(true);
    public ReactivePropertySlim<bool> IsSignedIn { get; }
#if DEBUG
      = new(true);
#else
      = new(false);
#endif
    public ReactivePropertySlim<NameData_PropertyChanged> UserName { get; } = new(new());
    public ReactivePropertySlim<double> BlurRadius { get; } = new(0);

    public ReactivePropertySlim<bool> IsProcessing { get; } = new(false);
  }

  internal interface IContainsIsProcessing
  {
    public ReactivePropertySlim<bool> IsProcessing { get; set; }
  }
  internal interface IContainsGetOnlyIsProcessing
  {
    public ReactivePropertySlim<bool> IsProcessing { get; }
  }

  /// <summary>指定のFrameに, CommandParmeterで指定された型のPageを表示する</summary>
  internal class FramePageChanger : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Frame TargetFrame { get; }
    public IContainsGetOnlyIsProcessing IsProcessingInstance { get; init; }

    public FramePageChanger(Frame _TargetFrame) => TargetFrame = _TargetFrame;

    public bool CanExecute(object parameter) => TargetFrame is not null && parameter is Type;

    public void Execute(object parameter)
    {
      if(IsProcessingInstance is not null)
        IsProcessingInstance.IsProcessing.Value = true;

      TargetFrame.Content = (parameter as Type)?.GetConstructor(new Type[0]).Invoke(null);
    }
  }
}
