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
$files = Get-ChildItem -Path $root -Filter '*.json' | Where-Object { $_.Name -ne 'en.json' }
$result = @()
foreach ($f in $files) {
    $obj = $null
    try { $obj = Get-Content $f.FullName -Raw | ConvertFrom-Json } catch { Write-Host "Could not parse $($f.Name)"; continue }
    $paths = Get-Paths $obj
    $missing = $refPaths | Where-Object { $paths -notcontains $_ }
    if ($missing.Count -gt 0) {
        $result += [PSCustomObject]@{
            File = $f.Name
            MissingCount = $missing.Count
            Missing = ($missing -join '; ')
        }
    }
}
if ($result.Count -eq 0) {
    Write-Host "All locales contain the reference keys."
} else {
    foreach ($r in $result) {
        Write-Host "--- $($r.File) : $($r.MissingCount) missing ---"
        $r.Missing -split '; ' | ForEach-Object { Write-Host "  $_" }
    }
}
