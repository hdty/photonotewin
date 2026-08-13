using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

/// <summary>
/// PhotoNoteWin のアプリアイコン。<b>形状の唯一の定義</b>。
///
/// 図案の考え方（ヒエラルキー）:
/// このアプリは写真を撮らない。既にある写真に「ひとことメモ」を付けて印刷する。
/// だから道具（カメラ・ペン）ではなく成果物を描く。主役は大きな写真カード、
/// メモはその右下に重なる小さなバッジとして従属させる。
///
/// 座標は 96x96 のグリッドで定義する（Material Symbols の 24px グリッド x4）。
/// 寸法の根拠と全座標は docs/ICON.md にある。
/// </summary>
public static class PhotoNoteMark
{
    /// <summary>設計グリッドの一辺。</summary>
    public const float Grid = 96f;

    // ブランドカラー（HidéToys）。ロゴの原色をそのまま使う。
    public static readonly Color Ink = Color.FromArgb(0x13, 0x25, 0x3F);        // インクネイビー
    public static readonly Color Water = Color.FromArgb(0xA3, 0xD8, 0xE1);      // 淡い水色
    public static readonly Color PaleSakura = Color.FromArgb(0xE8, 0xAF, 0xCF); // 薄桜色

    // 紺プレートの縁（水色を alpha 62% で）。暗いタスクバーは影も下地も付けないので、
    // これが無いとプレートの角丸が背景に溶けて輪郭が消える。
    static readonly Color Rim = Color.FromArgb(0x9E, 0xA3, 0xD8, 0xE1);
    const float PlateRadius = 22f;
    const float RimInset = 1.2f;
    const float RimRadius = 20.8f;
    const float RimWidth = 2.4f;

    // 写真カード（主役）。
    static readonly RectangleF Card = new RectangleF(19f, 18f, 58f, 46f);
    const float CardRadius = 6f;
    static readonly PointF SunCenter = new PointF(63f, 29f);
    const float SunRadius = 4.5f;

    // メモのバッジ（従）。カードの右下から対角にはみ出す。
    // Gap は地色で塗る「逃げ」。バッジをカードから浮かせて重なりを濁らせない。
    // プレートが不透明なので、合成モードを使わず地色でそのまま塗ればよい。
    static readonly RectangleF BadgeGap = new RectangleF(42f, 47f, 42f, 30f);
    const float BadgeGapRadius = 7f;
    static readonly RectangleF Badge = new RectangleF(45f, 50f, 36f, 24f);
    const float BadgeRadius = 5f;
    const float MemoLineWidth = 3.5f;

    /// <summary>角丸矩形のパス。</summary>
    public static GraphicsPath RoundedRect(RectangleF r, float radius)
    {
        float d = radius * 2f;
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// 山。底辺 y=58 はカード下辺 y=64 から 6 浮かせてある。
    /// 下辺まで届かせると、山（インク）と地（同じインク）が繋がって
    /// カードの底が抜け、写真に見えなくなる。
    /// </summary>
    static GraphicsPath Mountain()
    {
        var path = new GraphicsPath();
        path.AddPolygon(new[]
        {
            new PointF(25f, 58f),
            new PointF(39f, 36f),
            new PointF(49f, 48f),
            new PointF(56f, 40f),
            new PointF(71f, 58f),
        });
        return path;
    }

    /// <summary>
    /// マークを描く。呼び出し側で 96x96 の座標系に変換しておくこと。
    /// 描画順は docs/ICON.md と対応している。
    /// </summary>
    public static void Draw(Graphics g)
    {
        using (var ink = new SolidBrush(Ink))
        using (var water = new SolidBrush(Water))
        using (var sakura = new SolidBrush(PaleSakura))
        {
            // 1. プレート
            using (var path = RoundedRect(new RectangleF(0f, 0f, Grid, Grid), PlateRadius))
                g.FillPath(ink, path);

            // 2. 縁のキーライン
            using (var path = RoundedRect(
                new RectangleF(RimInset, RimInset, Grid - RimInset * 2f, Grid - RimInset * 2f),
                RimRadius))
            using (var pen = new Pen(Rim, RimWidth))
                g.DrawPath(pen, path);

            // 3. 写真カード
            using (var path = RoundedRect(Card, CardRadius))
                g.FillPath(water, path);

            // 4. 太陽
            g.FillEllipse(ink,
                SunCenter.X - SunRadius, SunCenter.Y - SunRadius,
                SunRadius * 2f, SunRadius * 2f);

            // 5. 山
            using (var path = Mountain())
                g.FillPath(ink, path);

            // 6. バッジの逃げ（地色）
            using (var path = RoundedRect(BadgeGap, BadgeGapRadius))
                g.FillPath(ink, path);

            // 7. メモのバッジ
            using (var path = RoundedRect(Badge, BadgeRadius))
                g.FillPath(sakura, path);

            // 8. メモの行
            using (var pen = new Pen(Ink, MemoLineWidth)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
            })
            {
                g.DrawLine(pen, 51f, 58f, 75f, 58f);
                g.DrawLine(pen, 51f, 66f, 67f, 66f);
            }
        }
    }
}

/// <summary>
/// <see cref="PhotoNoteMark"/> から .ico と確認用 PNG を書き出す。
/// 描画そのものは持たない。
/// </summary>
public static class IconBuilder
{
    /// <summary>
    /// Windows のシェルが引くサイズ。
    /// 512 は ICO の仕様上表現できない（ICONDIRENTRY の幅・高さは 1 バイトで、0 が 256 を意味する）。
    /// </summary>
    static readonly int[] Sizes = { 16, 24, 32, 48, 64, 128, 256 };

    /// <summary>確認用の合成画像に並べるサイズ。実寸で並べて目視する。</summary>
    static readonly int[] StripSizes = { 48, 32, 24, 16 };

    /// <summary>各サイズを 96 グリッドから直接ラスタライズする（縮小ではない）。</summary>
    static Bitmap Render(int size)
    {
        var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.ScaleTransform(size / PhotoNoteMark.Grid, size / PhotoNoteMark.Grid);
            PhotoNoteMark.Draw(g);
        }
        return bmp;
    }

    /// <summary>
    /// タスクバー相当の地に実寸で並べた確認用画像。
    /// 「16px で読めるか」と「暗い地で縁が溶けないか」は、目視しないと分からない。
    /// </summary>
    static void WriteStrip(string path, Color background, Dictionary<int, Bitmap> rendered)
    {
        const int pad = 24, gap = 28, height = 90;
        int width = pad * 2;
        foreach (var s in StripSizes) width += s + gap;
        width -= gap;

        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(background);
            int x = pad;
            foreach (var s in StripSizes)
            {
                g.DrawImageUnscaled(rendered[s], x, (height - s) / 2);
                x += s + gap;
            }
        }
        bmp.Save(path, ImageFormat.Png);
    }

    static void WriteIco(string icoPath, List<byte[]> pngs)
    {
        using var fs = File.Create(icoPath);
        using var bw = new BinaryWriter(fs);

        // ICONDIR
        bw.Write((ushort)0);              // reserved
        bw.Write((ushort)1);              // type: 1 = icon
        bw.Write((ushort)Sizes.Length);

        // ICONDIRENTRY x n（256 は 1 バイトに収まらないので 0 で表す）
        int offset = 6 + 16 * Sizes.Length;
        for (int i = 0; i < Sizes.Length; i++)
        {
            int s = Sizes[i];
            bw.Write((byte)(s >= 256 ? 0 : s));  // width
            bw.Write((byte)(s >= 256 ? 0 : s));  // height
            bw.Write((byte)0);                   // パレット色数（true color なので 0）
            bw.Write((byte)0);                   // reserved
            bw.Write((ushort)1);                 // color planes
            bw.Write((ushort)32);                // bits per pixel
            bw.Write((uint)pngs[i].Length);
            bw.Write((uint)offset);
            offset += pngs[i].Length;
        }

        foreach (var b in pngs) bw.Write(b);
    }

    /// <summary>
    /// .ico を <paramref name="icoPath"/> に、確認用 PNG を <paramref name="previewDir"/> に書き出す。
    /// </summary>
    public static void Build(string icoPath, string previewDir)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(icoPath))!);
        Directory.CreateDirectory(previewDir);

        var rendered = new Dictionary<int, Bitmap>();
        var pngs = new List<byte[]>();
        try
        {
            foreach (var size in Sizes)
            {
                var bmp = Render(size);
                rendered[size] = bmp;

                using var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                var bytes = ms.ToArray();
                pngs.Add(bytes);
                File.WriteAllBytes(Path.Combine(previewDir, $"icon_{size}.png"), bytes);
            }

            WriteIco(icoPath, pngs);

            // 暗い / 明るいタスクバー相当。
            WriteStrip(Path.Combine(previewDir, "icon_on_dark.png"),
                Color.FromArgb(0x1B, 0x1B, 0x1B), rendered);
            WriteStrip(Path.Combine(previewDir, "icon_on_light.png"),
                Color.FromArgb(0xE9, 0xE9, 0xEA), rendered);
        }
        finally
        {
            foreach (var bmp in rendered.Values) bmp.Dispose();
        }
    }
}
