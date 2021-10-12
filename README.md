# AsyncReadManagerMetricsRemote

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/AsyncReadManagerMetricsRemote)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/katsumasa/AsyncReadManagerMetricsRemote)

This package allows you to use AsyncReadManagerMetrics from UnityEditor.

## 概要

[AsyncReadManagerMetrics](https://docs.unity3d.com/ja/2020.3/ScriptReference/Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics.html)で得られる情報をUnityEditor上から取得するパッケージです。

![16cef9ca3ca755d318f282db406eb906](https://user-images.githubusercontent.com/29646672/135828887-cccb2a62-9540-4cf2-a361-2f109e327abb.gif)


## 動作環境

Unity2020.3.18f1 + Andoroid端末(Pixel4XL)で動作確認を行っています。

## Dependencies

[IMGUIExtentions](https://github.com/katsumasa/IMGUIExtentions.git)と[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)に依存している為、合わせて取得して下さい。

## Installing

### using git

```
git clone https://github.com/katsumasa/IMGUIExtentions.git
git clone https://github.com/katsumasa/RemoteConnect.git
git clone https://github.com/katsumasa/AsyncReadManagerMetricsRemote.git
```

### using Unity Package Manager

1. Click the add button in the status bar.
2. The options for adding packages appear.
3. Select Add package from git URL from the add menu. A text box and an Add button appear.
4. Enter https://github.com/katsumasa/IMGUIExtentions.git in the text box and click Add.
5. Enter https://github.com/katsumasa/RemoteConnect.git in the text box and click Add.
6. Enter clone https://github.com/katsumasa/AsyncReadManagerMetricsRemote.git in the text box and click Add.

[Click here for details.](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html)

## 使い方

- Prefab[AsyncReadManagerMetricsRemotePlayer](https://github.com/katsumasa/AsyncReadManagerMetricsRemote/blob/main/Runtime/Prefabs/AsyncReadManagerMetricsRemotePlayer.prefab)をSceneに配置してDevelopmentビルドを行います
- Window > AsyncReadManagerMetricsRemote からEditorWindowを起動させます
- Connect to から端末を選択
- Startボタンを押すことで計測が開始されます。
- Stopボタンで計測を終了します
- Saveボタンで計測結果をCSVファイルへ出力します

## Thats all! Appreciate your comments and feedback!

__Katsumasa Kimura: katsumasa@unity3d.com__

