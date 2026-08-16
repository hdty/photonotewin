# PhotoNoteWin

デジカメ写真にひとことメモを付けて、メモと撮影日入りで印刷するWindows用ソフト。
ヒデトイズ HidéToys

> 旧称は「PhotoNote」。同名のスマホアプリと区別するため PhotoNoteWin に改名しました。

## ダウンロード

[Releases](https://github.com/hdty/photonotewin/releases) から `PhotoNoteWin.exe` をダウンロードしてください。
インストール不要で、そのまま実行できます。

> **現在配布している exe はコード署名をしていません。**
> そのため初回起動時に SmartScreen の警告（「WindowsによってPCが保護されました」）が出ます。
> 配布元がこのリポジトリの Releases であることを確認したうえで、
> 「詳細情報」→「実行」で起動してください。
>
> コード署名は [SignPath Foundation](https://signpath.org/) の無償プログラムに申請中です
> （下記「Code signing policy」参照）。署名付きの exe を配布できるようになった時点で、
> この記載を更新します。

## 動作環境

- Windows 10 / 11 (64bit)
- 自己完結型の exe で配布するため、.NET ランタイムのインストールは不要

## 使い方

1. 「📂 フォルダを開く」で写真の入っているフォルダを選ぶ
2. 写真が2列×4行(1画面8枚)で表示される。「◀ 前へ」「次へ ▶」でページを切り替える
3. 並び順は「撮影日時・ファイル名・更新日時」×「昇順・降順」から選べる(既定: 撮影日時の古い順)。
   日時は時刻まで含めて比較する。並び順はフォルダごとに保存され、印刷もこの順で出る
4. それぞれの写真に説明文を書き、印刷したい写真にチェックを付ける
5. 「🖨 印刷する」を押すと同じウィンドウ内で印刷画面に切り替わる
   (「◀ 写真選びに戻る」でいつでも戻れる)
6. 用紙サイズ・向き・1ページの枚数・並べ方を選び、右のプレビューを確認して「印刷する」を押す
7. プリンタドライバの印刷ダイアログが開くので、プリンタ・部数・画質などを選んで印刷する

書いた説明文と印刷設定は、フォルダごとに自動保存される(入力をやめて1.5秒後、フォルダ切替時、終了時)。

## データファイル(.photonote.json)

写真フォルダごとに、固定名の隠しファイル `.photonote.json`(UTF-8、隠し属性)を保存する。

```json
{
  "app": "PhotoNote",
  "version": 1,
  "updatedAt": "2026-06-11T12:34:56+09:00",
  "sortOrder": "DateTakenAsc",
  "printSettings": {
    "paperSize": "A4",
    "orientation": "Portrait",
    "photosPerPage": 2,
    "layout": "Vertical"
  },
  "photos": [
    { "file": "IMG_0001.JPG", "text": "入場シーン", "selected": true },
    { "file": "IMG_0002.JPG", "text": "フェアプレイフラッグ掲出", "selected": false }
  ]
}
```

- `sortOrder`: `DateTakenAsc`(撮影日時 古い順・既定) / `DateTakenDesc` / `FileNameAsc` / `FileNameDesc` / `ModifiedAsc` / `ModifiedDesc`(いずれも時刻まで比較)
- `paperSize`: `A4` / `B5` / `2L` / `L` / `Hagaki`
- `orientation`: `Portrait`(縦) / `Landscape`(横)
- `photosPerPage`: 1 / 2 / 4
- `layout`: 2枚のときの並べ方 `Vertical`(縦に並べる) / `Horizontal`(横に並べる)
- `photos`: ファイル名(フォルダからの相対名)ごとのメモと選択状態。
  フォルダから消えた写真の項目も、メモが書いてあれば消さずに残す。

## 撮影日

EXIF の撮影日時(DateTaken)を使い、取れない場合はファイルの更新日時で代用する。
印刷時の表記はサンプルに合わせて `2025/5/7` 形式。

## 印刷レイアウトの仕様

- 紙端の余白: B5以上(A4・B5)=上下左右10mm、B5未満(2L・L判・はがき)=5mm
  (フチあり印刷の必要余白は EPSON EW-M873T=四辺3mm、Canon PIXUS TS8630=左右5mm/上下3.4mm)
- キャプションのフォント: Meiryo UI
- 文字サイズ固定: B5以上=10pt、B5未満=8pt(1ページの枚数では変えない)
- 写真とキャプションの間隔: 文字サイズの2割(A4で約0.7mm)
- 「B5以上」の判定は用紙の短辺が182mm(B5の短辺)以上かどうか

## ビルド

.NET 8 SDK が必要。

```powershell
cd PhotoNote
dotnet build                 # 開発ビルド
dotnet run                   # 実行
```

配布用(単一exe、ランタイム同梱):

```powershell
dotnet publish PhotoNote -c Release -r win-x64 --self-contained `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

出力: `PhotoNote\bin\Release\net8.0-windows\win-x64\publish\PhotoNoteWin.exe`
（プロジェクトフォルダ名は `PhotoNote` のままだが、exe 名は `PhotoNoteWin.exe`）

## 構成

- `MainWindow` — 1ウィンドウに2画面を持つ
  - 写真選び画面: 2列×4行のページ式一覧・並び順選択・メモ入力・印刷対象の選択
  - 印刷画面: 印刷設定・プレビュー(DocumentViewer、検索バーは非表示化)。
    「印刷する」でプリンタドライバの印刷ダイアログを開いてから印刷する(画質設定のため)
- `PrintDocumentBuilder` — 印刷ページ(FixedDocument)の組み立て。写真の下に左=メモ、右=撮影日
- `NoteFile` — `.photonote.json` の読み書き(隠し属性の付け外しを含む)
- `ImageLoader` — 画像読み込み(EXIF回転の反映、撮影日時の取得)
- アプリアイコンは `tools/IconTool` で生成する。元画像は要らず、形状は
  `tools/IconBuilder.cs` の `PhotoNoteMark` がベクタで持つ(16/24/32/48/64/128/256px)。
  寸法と配色の根拠は [docs/ICON.md](docs/ICON.md)
  ```powershell
  dotnet run --project tools\IconTool
  ```
  確認用に `testout/` へ各サイズの実寸PNGと、明暗のタスクバーに並べた合成画像も出る
- リポジトリに置く画像はビルドに必要な `PhotoNote/Assets/PhotoNote.ico` だけ。
  それも `tools/IconTool` で再生成できる(形状の定義はコード側にある)

## 開発用オプション

```powershell
PhotoNoteWin.exe --uishot <写真フォルダ> <出力フォルダ>
```

起動して写真フォルダを読み込み、写真選び画面と印刷画面をPNGに保存して終了する(UI確認用)。

## リリース手順

```powershell
dotnet publish PhotoNote -c Release -r win-x64 --self-contained `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
# バージョン情報の言語欄を「日本語」にする(.NETの仕様でニュートラルになるため)
.\tools\Set-JapaneseVersionLang.ps1 PhotoNote\bin\Release\net8.0-windows\win-x64\publish\PhotoNoteWin.exe
```

## Code signing policy

**現在の状態: 未署名。** [SignPath Foundation](https://signpath.org/) の無償コード署名
プログラムに申請中で、**承認されるまでのリリース（v0.2.0 を含む）は署名されていません。**

承認後は、以下のポリシーで署名する予定です。

Free code signing **will be** provided by [SignPath.io](https://signpath.io/),
certificate by [SignPath Foundation](https://signpath.org/).

- 署名の対象は GitHub Actions（[`.github/workflows/release.yml`](.github/workflows/release.yml)）で
  ビルドした成果物のみとします。ローカルでビルドした exe は配布しません。
- リリースは、リポジトリ管理者がタグ（`v*`）を付けたときにのみビルド・署名されます。
- 署名付きの配布を開始したら、この節と「ダウンロード」の記載を更新します。

**Roles**

- Committers and reviewers: [hdty](https://github.com/hdty)（単独の開発者。
  すべての変更を本人がレビューし、コミットします）
- Approvers: [hdty](https://github.com/hdty)（すべての署名リクエストを本人が承認します）

**Privacy policy**

このプログラムは利用者の個人情報を一切収集・送信しません。
ネットワーク通信は行わず、データの保存先は利用者が選んだ写真フォルダ内の
`.photonote.json`（キャプションと印刷設定）のみです。

This program will not transfer any information to other networked systems unless
specifically requested by the user or the person installing or operating it.

## ライセンス

[MIT License](LICENSE)。リポジトリの内容はすべて MIT で利用できます。

なお「HidéToys」はサークル名です。ソフトウェアの利用・改変・再配布は MIT の
条件で自由に行えますが、派生物があたかも HidéToys 公式であるかのように
サークル名を表示することはご遠慮ください。
