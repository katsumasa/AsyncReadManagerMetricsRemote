# AsyncReadManagerMetricsRemote

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/AsyncReadManagerMetricsRemote)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/katsumasa/AsyncReadManagerMetricsRemote)

## 概要

[AsyncReadManagerMetrics](https://docs.unity3d.com/ja/2020.3/ScriptReference/Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics.html)で得られる情報をUnityEditor上から取得するパッケージです。
Unityの非同期読み込みに関する各種情報を計測し、グラフで表示することが可能です。計測したデータはCSV形式で出力することも可能です。

![16cef9ca3ca755d318f282db406eb906](https://user-images.githubusercontent.com/29646672/135828887-cccb2a62-9540-4cf2-a361-2f109e327abb.gif)


## 動作環境

Unity2020.3.33f1 + Andoroid端末(Pixel4XL)で動作確認を行っています。

## Dependencies

[IMGUIExtentions](https://github.com/katsumasa/IMGUIExtentions.git)と[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)に依存している為、合わせて取得して下さい。

## Installing

### コンソールから取得する場合

コンソールから下記のコマンドを実行してください。

```
git clone https://github.com/katsumasa/IMGUIExtentions.git
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/AsyncReadManagerMetricsRemote.git
```

### Unity Package Managerから取得する場合

下記の手順

1. Window > Package ManagerでPackage Managerを開く
2. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
3. ダイアログへ`https://github.com/katsumasa/IMGUIExtentions.git`を設定し、Addボタンを押す
4. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
5. ダイアログへ `https://github.com/katsumasa/RemoteConnect.git`を設定し、Addボタンを押す
6. Package Manager左上の+のプルダウンメニューからAdd package form git URL...を選択する
7. ダイアログへ `https://github.com/katsumasa/AsyncReadManagerMetricsRemote.git`を設定し、Addボタンを押す

[UPMの詳細はこちら](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html)

## 使い方

- Prefab[AsyncReadManagerMetricsRemotePlayer](https://github.com/katsumasa/AsyncReadManagerMetricsRemote/blob/main/Runtime/Prefabs/AsyncReadManagerMetricsRemotePlayer.prefab)をSceneに配置してDevelopmentビルドを行います
- Window > UTJ > AsyncReadManagerMetricsRemote からEditorWindowを起動させます
- Connect to から端末を選択
- Startボタンを押すことで計測が開始されます。
- Stopボタンで計測を終了します
- Saveボタンで計測結果をCSVファイルへ出力します

## 注意事項

`AsyncReadManagerMetrics.StartCollectingMetrics()`と`AsyncReadManagerMetrics.StopCollectingMetrics()`の間（つまりデータ記録中)に、[AseetBundle.Unload](https://docs.unity3d.com/ja/current/ScriptReference/AssetBundle.Unload.html)と[Resources.UnloadUnusedAssets](https://docs.unity3d.com/ja/current/ScriptReference/Resources.UnloadUnusedAssets.html)を組み合わせて実行するとクラッシュが発生することを確認していますのでご注意下さい。

## サンプルコード

本プラグインのサンプルプロジェクトは[こちら](https://github.com/katsumasa/AsyncReadManagerMetricsRemoteSample)です。

## その他

質問・バグ報告は[Issues](https://github.com/katsumasa/AsyncReadManagerMetricsRemote/issues)からお願いします。対応の約束は出来かねますが可能な限り対応します。

以上
