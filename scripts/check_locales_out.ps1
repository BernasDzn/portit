$root = Join-Path $PSScriptRoot "..\FrontEnd\src\locales"
$refPath = Join-Path $root 'en.json'
$ref = Get-Content $refPath -Raw | ConvertFrom-Json
function Get-Paths($obj, $prefix = '') {
    $paths = @()
    if ($obj -eq $null) { return $paths }
    $props = @()
    try { $props = $obj.psobject.properties.name } catch { return $paths }
    foreach ($p in $props) {
        $val = $obj.$p
        $path = if ($prefix -ne '') { "$prefix.$p" } else { $p }
        if ($val -ne $null -and ($val -is [System.Management.Automation.PSObject] -or $val -is [System.Collections.Hashtable])) {
            $sub = Get-Paths $val $path
            $paths += $sub
        } else {
            $paths += $path
        }
    }
    return $paths
}
$refPaths = Get-Paths $ref
$files = Get-ChildItem -Path $root -Filter '*.json' | Where-Object { $_.Name -ne 'en.json' } | Sort-Object Name
$report = @{ summary = @(); details = @{} }
foreach ($f in $files) {
    $obj = $null
    try { $obj = Get-Content $f.FullName -Raw | ConvertFrom-Json } catch { $report.summary += @{ File = $f.Name; MissingCount = -1 }; continue }
    $paths = Get-Paths $obj
    $missing = $refPaths | Where-Object { $paths -notcontains $_ }
    $report.summary += @{ File = $f.Name; MissingCount = $missing.Count }
    if ($missing.Count -gt 0) { $report.details[$f.Name] = $missing }
}
$outPath = Join-Path $PSScriptRoot 'locales_report.json'
$report | ConvertTo-Json -Depth 10 | Out-File -FilePath $outPath -Encoding UTF8
Write-Host "Wrote report to $outPath"