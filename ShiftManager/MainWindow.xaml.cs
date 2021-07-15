﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

using Reactive.Bindings;

using ShiftManager.DataClasses;

namespace ShiftManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IContainsApiHolder
  {
    // public IApiHolder ApiHolder { get; set; } = new ApiHolder() { Api = new Communication.RestApiBroker() };
    public IApiHolder ApiHolder { get; set; } = new ApiHolder() { Api = new Communication.InternalApi() };
    private MainWindowViewModel MWVM { get; }

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
      MWVM = new() { MainFramePageChanger = new(MainFrame) };
      DataContext = MWVM;
      SignInPageElem.ApiHolder = this.ApiHolder;
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
      if (e.Content is IContainsApiHolder i)
        i.ApiHolder = ApiHolder;
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
      _ = await ApiHolder.Api.SignOutAsync();
      MWVM.IsSignedIn.Value = false;
      MainFrame.Content = null;
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
  }

  /// <summary>指定のFrameに, CommandParmeterで指定された型のPageを表示する</summary>
  internal class FramePageChanger : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Frame TargetFrame { get; }

    public FramePageChanger(Frame _TargetFrame) => TargetFrame = _TargetFrame;

    public bool CanExecute(object parameter) => TargetFrame is not null && parameter is Type;

    public void Execute(object parameter) => TargetFrame.Content = (parameter as Type)?.GetConstructor(new Type[0]).Invoke(null);
  }
}
