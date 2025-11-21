$root = Join-Path $PSScriptRoot "..\FrontEnd\src\locales"
$refPath = Join-Path $root 'en.json'
$ref = Get-Content $refPath -Raw | ConvertFrom-Json
function Get-PathsAndValues($obj, $prefix = '') {
    $res = @{}
    if ($obj -eq $null) { return $res }
    $props = @()
    try { $props = $obj.psobject.properties.name } catch { return $res }
    foreach ($p in $props) {
        $val = $obj.$p
        $path = if ($prefix -ne '') { "$prefix.$p" } else { $p }
        if ($val -ne $null -and ($val -is [System.Management.Automation.PSObject] -or $val -is [System.Collections.Hashtable])) {
            $sub = Get-PathsAndValues $val $path
            foreach ($k in $sub.Keys) { $res[$k] = $sub[$k] }
        } else {
            $res[$path] = $val
        }
    }
    return $res
}
$refMap = Get-PathsAndValues $ref
$files = Get-ChildItem -Path $root -Filter '*.json' | Where-Object { $_.Name -ne 'en.json' } | Sort-Object Name
$report = @{}
foreach ($f in $files) {
    try { $obj = Get-Content $f.FullName -Raw | ConvertFrom-Json } catch { continue }
    $map = Get-PathsAndValues $obj
    $placeholders = @()
    foreach ($k in $refMap.Keys) {
        if ($map.ContainsKey($k)) {
            $rval = $refMap[$k]
            $tval = $map[$k]
            if ($rval -eq $tval) { $placeholders += $k }
        }
    }
    if ($placeholders.Count -gt 0) { $report[$f.Name] = $placeholders }
}
$out = Join-Path $PSScriptRoot 'placeholders_report.json'
$report | ConvertTo-Json -Depth 10 | Out-File -FilePath $out -Encoding UTF8
Write-Host "Wrote placeholders report to $out"