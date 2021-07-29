# ShiftManager.Communication.InternalApi
ShiftManagerでオフライン動作を実現するための通信ライブラリです.  自動生成されるサンプルデータを用いた動作のみ使用可能であり, ファイルを使用した入出力には対応していません.

## 依存関係
- System.Collections.Immutable (Version 5.0.0)
  - 不変性が保証された配列を返すために使用しています.  別にIReadOnlyListを使っても良かったんですが, ImmutableArrayを使ってみたかったため, 使用しています.
- [ShiftManager.Communication.Common.csproj](../ShiftManager.Communication.Common/README.md)
- [ShiftManager.DataClasses.csproj](../ShiftManager.DataClasses/README.md)

## 使用方法
.NET 5.0 SDKが存在する環境にて, 以下のコマンドを実行すると, 依存関係の解決やビルドが行われます.

```
dotnet build
```

## ライセンス

## 各ファイルの説明
(準備中)
