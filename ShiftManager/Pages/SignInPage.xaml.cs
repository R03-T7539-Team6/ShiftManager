using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for LoginPage.xaml
  /// </summary>
  public partial class SignInPage : UserControl, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();

    public event EventHandler Login;

    public SignInPage()
    {
      InitializeComponent();
    }

/*******************************************
* specification ;
* name = In_Click_2 ;
* Function = ログインボタンが押された時にログインを試行する関数を呼び出す ;
* note = 補足説明 ;
* date = 07/03/2021 ;
* author = 佐藤真通 ;
* History = 更新履歴 ;
* input = ログインボタンが押されたことを知らせるイベントハンドラ ;
* output = N/A ;
* end of specification ;
*******************************************/
    private async void ln_Click_2(object sender, RoutedEventArgs e)
    {
      string UID = ID.Text;
      string UPass = Pass.Password;
      var re = await SignInAsyncTest_WithIDAndPassword(UID, UPass);

      if (re.IsSuccess)
      {
        Login?.Invoke(this, EventArgs.Empty);

        //成功時は入力内容をクリアする
        ID.Text = string.Empty;
        Pass.Password = string.Empty;
      }

      MessageBox.Show(re.IsSuccess.ToString() + " : " + re.ResultCode.ToString());
    }

    /*******************************************
    * specification ;
    * name = SignInAsyncTest_WithIDAndPassword ;
    * Function = ユーザID文字列と生パスワードを受け取り, サインインを試行する ;
    * note = テストプロジェクトからコピーして利用 ;
    * date = 07/03/2021 ;
    * author = 藤田一範, 佐藤真通 ;
    * History = v1.0:新規作成 ;
    * input = ユーザID、生パスワード ;
    * output = 試行結果 ;
    * end of specification ;
    *******************************************/
    public async Task<ApiResult> SignInAsyncTest_WithIDAndPassword(string userID, string rawPassword)
    {
      //ユーザID情報をAPIに渡すために, 型を変換します
      UserID targetUserID = new(userID);

      //パスワードをハッシュ化するために必要な情報を取得します
      ApiResult<HashedPassword> passwordHashingDataRequestResult = await ApiHolder.Api.GetPasswordHashingDataAsync(targetUserID);

      //もしもID不一致等で実行に失敗した場合は, IsSuccessプロパティに"false"が入ります.
      if (!passwordHashingDataRequestResult.IsSuccess)
        //実行結果の詳細はResultCodeプロパティに入っています
        return new(false, passwordHashingDataRequestResult.ResultCode);

      //ハッシュ化用情報を取得できていた場合のみ, 次に進みます

      //生パスワードを, ハッシュ化するために必要な情報と一緒にHashedPasswordGetterに投げます
      HashedPassword hashedPassword = HashedPasswordGetter.Get(rawPassword, passwordHashingDataRequestResult.ReturnData);

      //ハッシュ化パスワードとともに, サインインを試行します
      //API側で削られる情報ではありますが, 念のためSaltとStretchCountの情報は削除しておきましょう.  削除せずに渡しても動作に問題はありません.
      return await ApiHolder.Api.SignInAsync(targetUserID, hashedPassword with { Salt = string.Empty, StretchCount = 0 });
    }
  }
}
