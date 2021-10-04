# AsyncReadManagerMetricsRemote

This package allows you to use AsyncReadManagerMetrics from UnityEditor.

## 概要

[AsyncReadManagerMetrics](https://docs.unity3d.com/ja/2020.3/ScriptReference/Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics.html)で得られる情報をUnityEditor上から取得するパッケージです。

![16cef9ca3ca755d318f282db406eb906](https://user-images.githubusercontent.com/29646672/135828887-cccb2a62-9540-4cf2-a361-2f109e327abb.gif)


## 動作環境

Unity2020.3.18f1 + Andoroid端末(Pixel4XL)で動作確認を行っています。

## Dependencies

[EditorGUILayoutExt](https://github.com/katsumasa/EditorGUILayoutExt.git)と[RemoteConnect](https://github.com/katsumasa/RemoteConnect.git)に依存している為、合わせて取得して下さい。

## 使い方

- Prefab[AsyncReadManagerMetricsRemotePlayer](https://github.com/katsumasa/AsyncReadManagerMetricsRemote/blob/main/Runtime/Prefabs/AsyncReadManagerMetricsRemotePlayer.prefab)をSceneに配置してDevelopmentビルドを行います
- Window > AsyncReadManagerMetricsRemote からEditorWindowを起動させます
- Connect to から端末を選択
- Startボタンを押すことで計測が開始されます。
- Stopボタンで計測を終了します
- Saveボタンで計測結果をCSVファイルへ出力します



