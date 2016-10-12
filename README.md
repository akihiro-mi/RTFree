# RTFree

radiko タイムフリー録音ツール  
WindowsとMacで動作確認済み  
Linuxは未検証
## 必要なもの
- [.NET Core SDK](https://www.microsoft.com/net/core#windows)
- swfextract
- ffmpeg

## ビルド
```sh
dotnet restore
dotnet build
```
## 実行
```sh
dotnet run
```

## 引数
| 引数 | 説明 |
|----|----|
|-s |放送局ID (必須)|
|-f |開始時刻(yyyyMMddHHmm) (必須)|
|-t | 終了時刻(yyyyMMddHHmm) (必須)|
|-m|メールアドレス|
|-p|パスワード|
|-n|ファイル名(拡張子無し)　未指定の場合は放送局ID\_開始時刻\_終了時刻|


## 使用例1
```sh
dotnet run -s MBS -f 201610060800 -t 201610061030 -n ありがとう浜村淳です
```

## 使用例2(プレミアムの場合)
```sh
dotnet run -s MBS -f 201610060800 -t 201610061030 -n ありがとう浜村淳です -m hogehoge@hoge.com -p password
```
