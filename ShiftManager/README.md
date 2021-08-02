# ShiftManager
シフト管理ツールのクライアントです.  動作確認用に, ランダムで生成されたデータを用いたオフライン動作にも対応します.

基本的にWindows環境に依存する機能を実装し, Linux環境でもビルド/動作可能なものに関しては, 可能な限り[ShiftManager.Common.csproj](../ShiftManager.Common/README.md)に実装するようにしています.

## 使用方法
[こちらをご覧ください](./HowToUse.md)

## 依存関係
- MahApps.Metro (Version 2.4.7)
  - ProgressRingの実現に使用
- ReactiveProperty.Core (Version 7.11.0)
  - ReactivePropertySlimを使用するために使用
- zxcvbn-core (Version 7.0.92)
  - [Controls/PasswordStrengthVisualizerControl.xaml.cs](./Controls/PasswordStrengthVisualizerControl.xaml.cs#L60)にて, パスワード強度チェックを行うために使用
- [RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator.csproj](../RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/README.md)
- [ShiftManager.Common.csproj](../ShiftManager.Common/README.md)
- [ShiftManager.Communication.Common.csproj](../ShiftManager.Communication.Common/README.md)
- [ShiftManager.Communication.InternalApi.csproj](../ShiftManager.Communication.InternalApi/README.md)
- [ShiftManager.Communication.RestApiBroker.csproj](../ShiftManager.Communication.RestApiBroker/README.md)
- [ShiftManager.DataClasses.csproj](../ShiftManager.DataClasses/README.md)

## ビルド方法
.NET 5.0 SDKが存在するWindows環境にて, 以下のコマンドを実行すると, 依存関係の解決やビルドが行われます.

```
dotnet build
```

## ライセンス
ライセンスは設定しません.  なお, GitHubの利用規約に基づき, GitHubユーザは, GitHubの機能を通じてコンテンツを使用/表示/実行し, またGitHub上で複製(fork)する権利を有します.  
[GitHub Terms of Service (D. User-Generated Content)](https://docs.github.com/en/github/site-policy/github-terms-of-service#d-user-generated-content)

## 各ファイルの説明
(準備中)
