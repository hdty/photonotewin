# プリンタが Windows 経由で公開している用紙種類・画質を一覧する。
# PhotoNote が写真用紙・高画質を自動設定できるかの確認用。
# 使い方:
#   .\Show-PrinterCapabilities.ps1              # 既定のプリンタを調べる
#   .\Show-PrinterCapabilities.ps1 -Name "EPSON EW-M873T Series"
param([string]$Name)

Add-Type -AssemblyName ReachFramework, System.Printing

if ($Name) {
    $server = New-Object System.Printing.LocalPrintServer
    $queue = $server.GetPrintQueue($Name)
} else {
    $queue = [System.Printing.LocalPrintServer]::GetDefaultPrintQueue()
}

"プリンタ: $($queue.FullName)"
$caps = $queue.GetPrintCapabilities()

"`n[用紙種類 (PageMediaType)]"
$types = @($caps.PageMediaTypeCapability)
if ($types.Count -eq 0) { "  (公開なし)" } else { $types | ForEach-Object { "  - $_" } }

"`n[画質 (OutputQuality)]"
$quals = @($caps.OutputQualityCapability)
if ($quals.Count -eq 0) { "  (公開なし)" } else { $quals | ForEach-Object { "  - $_" } }

# PhotoNote と同じ優先順で判定する
$preferredTypes = @('PhotographicGlossy','PhotographicHighGloss','Photographic',
                    'PhotographicSemiGloss','PhotographicSatin','PhotographicMatte')
$chosenType = $preferredTypes | Where-Object { $types -contains $_ } | Select-Object -First 1
$chosenQual = @('Photographic','High')   | Where-Object { $quals -contains $_ } | Select-Object -First 1

"`n=== PhotoNote が自動設定できる内容 ==="
if ($chosenType) {
    "  用紙種類: $chosenType  → 写真用紙を自動で選べます"
} else {
    "  用紙種類: 設定不可(このドライバは写真用紙を Windows に公開していません)"
    "            → プリンタの「印刷設定」で写真用紙を既定にしてください"
}
if ($chosenQual) { "  画質    : $chosenQual" } else { "  画質    : 設定不可(既定のまま)" }
