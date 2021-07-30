# ShiftManager.Communication.RestApiBroker
ShiftManagerでサーバーに接続しての動作を実現するための通信ライブラリです.

## 依存関係
- System.Collections.Immutable (Version 5.0.0)
  - 不変性が保証された配列を返すために使用しています.  別にIReadOnlyListを使っても良かったんですが, ImmutableArrayを使ってみたかったため, 使用しています.
- [ShiftManager.Communication.Common.csproj](../ShiftManager.Communication.Common/README.md)
- [ShiftManager.Communication.ServerIF.csproj](../ShiftManager.Communication.ServerIF/README.md)
- [ShiftManager.DataClasses.csproj](../ShiftManager.DataClasses/README.md)

## 使用方法
.NET 5.0 SDKが存在する環境にて, 以下のコマンドを実行すると, 依存関係の解決やビルドが行われます.

```
dotnet build
```

## ライセンス
ライセンスは設定しません.  なお, GitHubの利用規約に基づき, GitHubユーザは, GitHubの機能を通じてコンテンツを使用/表示/実行し, またGitHub上で複製(fork)する権利を有します.  
[GitHub Terms of Service (D. User-Generated Content)](https://docs.github.com/en/github/site-policy/github-terms-of-service#d-user-generated-content)

## 各ファイルの説明
(準備中)
