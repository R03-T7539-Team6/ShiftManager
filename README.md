# ShiftManager
茨城大学情報工学科 2021年度ソフトウェア開発演習にてTeam-6が開発したソフトウェアです.  
サーバーと通信して, シフト希望の提出/閲覧, 予定シフト組み/閲覧, 勤怠打刻/閲覧等を行うことができます.

## 使用方法
[こちらをご覧ください](./ShiftManager/HowToUse.md)

## プロジェクト一覧
- [RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator](./RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/README.md)
- [ShiftManager](./ShiftManager/README.md)
- [ShiftManager.Common](./ShiftManager.Common/README.md)
- [ShiftManager.Common.Test](./ShiftManager.Common.Test/README.md)
- [ShiftManager.Communication.Common](./ShiftManager.Communication.Common/README.md)
- [ShiftManager.Communication.InternalApi](./ShiftManager.Communication.InternalApi/README.md)
- [ShiftManager.Communication.InternalApi.Test](./ShiftManager.Communication.InternalApi.Test/README.md)
- [ShiftManager.Communication.RestApiBroker](./ShiftManager.Communication.RestApiBroker/README.md)
- [ShiftManager.Communication.RestApiBroker.Test](./ShiftManager.Communication.RestApiBroker.Test/README.md)
- [ShiftManager.Communication.ServerIF](./ShiftManager.Communication.ServerIF/README.md)
- [ShiftManager.Communication.ServerIF.Test](./ShiftManager.Communication.ServerIF.Test/README.md)
- [ShiftManager.DataClasses](./ShiftManager.DataClasses/README.md)
- [ShiftManager.PrintHelper](./ShiftManager.PrintHelper/README.md)

依存関係は各プロジェクトの README.md に記載しています.

## 環境構築
WPFを使用しているため, 2021年7月29日現在, 一部プロジェクトについてはWindows環境でのみビルド可能です.  
いずれのプロジェクトについても, PCに.NET 5.0 SDKをインストールしていただくと, プロジェクトをビルド可能になります.

## ライセンス
RoslynSDK.SourceGeneratorSamples.AutoNotifyGeneratorプロジェクトに関しては, RoslynSDKリポジトリに含まれるコードを, RoslynSDKリポジトリに設定されたMITライセンスに基づいて複製利用しているため, MITライセンスが適用されます.  詳細は, [RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/LICENSE.txt](./RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/LICENSE.txt)および[RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/README.md](./RoslynSDK.SourceGeneratorSamples.AutoNotifyGenerator/README.md)をご確認ください.

各ShiftManagerプロジェクトに関しては, ライセンスを設定しません.  但し, GitHubの利用規約に基づき, GitHubユーザは, GitHubの機能を通じてコンテンツを使用/表示/実行し, またGitHub上で複製(fork)する権利を有します.  
[GitHub Terms of Service (D. User-Generated Content)](https://docs.github.com/en/github/site-policy/github-terms-of-service#d-user-generated-content)

