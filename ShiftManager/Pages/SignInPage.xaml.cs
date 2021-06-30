using System.Windows.Controls;
using System.Threading.Tasks;
using ShiftManager.DataClasses;
using ShiftManager.Communication;
using System.Windows;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for LoginPage.xaml
  /// </summary>
  public partial class SignInPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }

    public SignInPage()
    {
      InitializeComponent();
    }

    private async void ln_Click_2(object sender, System.Windows.RoutedEventArgs e)
    {
      string UID = ID.Text;
      string UPass = Pass.Text;
      var re=await SignInAsyncTest_WithIDAndPassword(UID, UPass);
      MessageBox.Show(re.IsSuccess.ToString() + " * " + re.ResultCode.ToString());
    }

    public async Task<ApiResult> SignInAsyncTest_WithIDAndPassword(string userID, string rawPassword)
    {
      //テスト用にInternalApiのインスタンスをローカル変数に入れていますが, 実際はインスタンス変数として置きます
      InternalApi api = new();

      //ユーザID情報をAPIに渡すために, 型を変換します
      UserID targetUserID = new(userID);

      //パスワードをハッシュ化するために必要な情報を取得します
      ApiResult<HashedPassword> passwordHashingDataRequestResult = await api.GetPasswordHashingDataAsync(targetUserID);

      //もしもID不一致等で実行に失敗した場合は, IsSuccessプロパティに"false"が入ります.
      if (!passwordHashingDataRequestResult.IsSuccess)
        //実行結果の詳細はResultCodeプロパティに入っています
        return new(false, passwordHashingDataRequestResult.ResultCode);

      //ハッシュ化用情報を取得できていた場合のみ, 次に進みます

      //生パスワードを, ハッシュ化するために必要な情報と一緒にHashedPasswordGetterに投げます
      HashedPassword hashedPassword = InternalApi.HashedPasswordGetter(rawPassword, passwordHashingDataRequestResult.ReturnData);

      //ハッシュ化パスワードとともに, サインインを試行します
      //API側で削られる情報ではありますが, 念のためSaltとStretchCountの情報は削除しておきましょう.  削除せずに渡しても動作に問題はありません.
      return await api.SignInAsync(targetUserID, hashedPassword with { Salt = string.Empty, StretchCount = 0 });
    }
  }
}
