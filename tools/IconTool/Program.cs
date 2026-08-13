// アプリアイコンを生成する。
//
//   dotnet run --project tools\IconTool
//
// 形状の定義は ..\IconBuilder.cs の PhotoNoteMark にある。元画像は要らない。
// 引数で出力先を上書きできる: IconTool [出力ico] [確認用PNGの出力先]
using System;
using System.IO;

string root = FindRepoRoot(Directory.GetCurrentDirectory());
if (root == null) root = FindRepoRoot(AppContext.BaseDirectory);
if (root == null) root = Directory.GetCurrentDirectory();

string ico = args.Length > 0 ? args[0] : Path.Combine(root, "PhotoNote", "Assets", "PhotoNote.ico");
string previewDir = args.Length > 1 ? args[1] : Path.Combine(root, "testout");

IconBuilder.Build(ico, previewDir);
Console.WriteLine("icon written : " + Path.GetFullPath(ico));
Console.WriteLine("previews     : " + Path.GetFullPath(previewDir));

// 実行場所に依存しないよう、PhotoNote\PhotoNote.csproj を目印にリポジトリ直下を探す。
static string FindRepoRoot(string start)
{
    for (var dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
        if (File.Exists(Path.Combine(dir.FullName, "PhotoNote", "PhotoNote.csproj")))
            return dir.FullName;
    return null;
}
