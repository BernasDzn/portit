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
$summary = @()
$details = @{}
foreach ($f in $files) {
    $obj = $null
    try { $obj = Get-Content $f.FullName -Raw | ConvertFrom-Json } catch { $summary += [PSCustomObject]@{ File=$f.Name; MissingCount = -1 }; continue }
    $paths = Get-Paths $obj
    $missing = $refPaths | Where-Object { $paths -notcontains $_ }
    $summary += [PSCustomObject]@{ File=$f.Name; MissingCount = $missing.Count }
    if ($missing.Count -gt 0) { $details[$f.Name] = $missing }
}
Write-Host "Locale parity summary (file : missing count):"
$summary | ForEach-Object { Write-Host " - $($_.File) : $($_.MissingCount)" }
if ($details.Count -gt 0) {
    Write-Host "`nDetailed missing keys (only files with missing keys):`n"
    foreach ($k in $details.Keys) {
        Write-Host "--- $k : $($details[$k].Count) missing ---"
        $details[$k] | ForEach-Object { Write-Host "  $_" }
    }
} else {
    Write-Host "All locales are up to date."
}
