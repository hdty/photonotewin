# PhotoNoteWin

## 概要
高齢の利用者（作者の父・後期高齢者）が、デジカメ写真を簡単に印刷するための Windows アプリ。
（旧称 PhotoNote。同名スマホアプリとの区別のため改名。表示名・exe名のみ変更し、
　名前空間 `PhotoNote`・フォルダ名・データファイル `.photonote.json` は据え置き）
Windows7 から Windows11 へ移行できるようにすることが動機。
フォルダ内の写真を一覧表示し、各写真にテキストを書き込み、
用紙サイズ・1枚あたりの写真枚数・レイアウトを選んで、
写真の下に「テキスト＋撮影日」を付けて印刷する。
テキストと印刷設定はフォルダごとの隠しファイル（.photonote.json）に保存する。

- 種別: C# / .NET 8 WPF デスクトップアプリ
- 対象: Windows 10 以上
- サークル: HidéToys（ヒデトイズ）名義で公開
- 最優先方針: とにかく簡単に。高齢者が迷わず使えるUIを最優先する。

## 公開・配布
- GitHub: https://github.com/hdty/photonotewin （公開リポジトリ、アカウント hdty）
- ライセンス: 純粋な MIT（例外なし）。SignPath Foundation のコード署名が
  「OSI 承認ライセンス」を条件とし、GitHub にも MIT と判定させる必要があるため、
  リポジトリには MIT で配れるものだけを置く。
- 配布: GitHub Releases に単一ファイル exe を添付する（リポジトリ本体に exe は入れない）。
- コード署名: SignPath Foundation に申請中。CI（.github/workflows/release.yml）で
  タグ push 時にビルド → 言語パッチ → 署名 → Release 添付まで行う。
  README の「Code signing policy」節は Foundation の要件なので消さない。

## ディレクトリ
- PhotoNote/      : 本体（WPF アプリ）
- PhotoNote.Test/ : 検証プログラム（テストフレームワークではなく実行型。後述）
- assets/         : ロゴの SVG マスタ（非公開。.gitignore 対象でリポジトリには入れない）
- sample/         : 仕様サンプルPDF（photonotesample01.pdf=印刷結果, 02=編集画面）
- testphotos/     : テスト用写真（自動生成）
- testout/        : 出力サンプル・UIスクリーンショット・アイコンの確認用画像
- tools/          : アイコン生成・バージョン情報パッチ等の補助ツール
- docs/           : 詳細仕様。本文からは @docs/SPEC.md で参照

## コマンド
- ビルド : `dotnet build`
- 実行   : `dotnet run --project PhotoNote`
- 検証   : `dotnet run --project PhotoNote.Test`
  （`PhotoNote.Test` は xUnit 等ではなく OutputType=Exe の自作検証。`dotnet test` では走らない）
- 配布   : 単一ファイル exe を作り、バージョン情報の言語を日本語に直す
  ```powershell
  dotnet publish PhotoNote -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
  .\tools\Set-JapaneseVersionLang.ps1 PhotoNote\bin\Release\net8.0-windows\win-x64\publish\PhotoNoteWin.exe
  ```

## ルール
- 第一の利用者は高齢者。UIは簡潔・大きく・迷わない設計を最優先する。
- UIフォントは Meiryo UI を使う（游ゴシック・MS UI ゴシックは使わない）。
- 保存ファイル .photonote.json の仕様を変える時は、実装（PhotoNote/NoteFile.cs）を正とし、
  docs/SPEC.md と必ず整合させる。
- アプリアイコンの形状の唯一の定義は tools/IconBuilder.cs の `PhotoNoteMark`（ベクタ）。
  ブランド色（水色 #A3D8E1 / 薄桜 #E8AFCF / 紺 #13253F）を含め、根拠は @docs/ICON.md にある。
  ラスタ原画は持たない。SVG のマスタも作らない（下の「公開禁止」と紛らわしくなるため）。
- exe のバージョン情報: 著作権は「© HidéToys」、会社名（作者）欄は空にする。
- ビルド成果物（bin/, obj/）は編集・参照しない。
- リポジトリに置く画像は、ビルドに必要な PhotoNote/Assets/PhotoNote.ico だけにする。
  ロゴ画像は置かない（純 MIT を保つため。ロゴは HidéToys のサークル名を表すもので、
  MIT で自由に再配布させたくない）。
- 公開禁止: assets/ の SVG（ロゴのマスタ）と sample/ の PDF（実在の人物が写っている）。
  いずれも .gitignore で除外済み。push 前に `git ls-tree -r origin/main` で混入がないか確認する。
