$root = Join-Path $PSScriptRoot "..\FrontEnd\src\locales"
$refPath = Join-Path $root 'en.json'
$ref = Get-Content $refPath -Raw | ConvertFrom-Json
function Merge-Missing($target, $source) {
    $sourceProps = @()
    try { $sourceProps = $source.psobject.properties.name } catch { return }
    foreach ($p in $sourceProps) {
        $sval = $source.$p
        $tHas = $false
        try { $tHas = $target.psobject.properties.name -contains $p } catch { $tHas = $false }
        if (-not $tHas) {
            # If source is an object, copy it as-is; else copy scalar
            if ($sval -ne $null -and ($sval -is [System.Management.Automation.PSObject] -or $sval -is [System.Collections.Hashtable])) {
                # Deep clone the source object into a new psobject
                $json = $sval | ConvertTo-Json -Depth 20
                $clone = $json | ConvertFrom-Json
                $target | Add-Member -Force -MemberType NoteProperty -Name $p -Value $clone
            } else {
                $target | Add-Member -Force -MemberType NoteProperty -Name $p -Value $sval
            }
        } else {
            # If both are objects, recurse
            $tval = $target.$p
            if ($sval -ne $null -and $tval -ne $null -and ($sval -is [System.Management.Automation.PSObject] -or $sval -is [System.Collections.Hashtable]) -and ($tval -is [System.Management.Automation.PSObject] -or $tval -is [System.Collections.Hashtable])) {
                Merge-Missing $tval $sval
            }
        }
    }
}

$files = Get-ChildItem -Path $root -Filter '*.json' | Where-Object { $_.Name -ne 'en.json' } | Sort-Object Name
foreach ($f in $files) {
    Write-Host "Processing $($f.Name)"
    try {
        $obj = Get-Content $f.FullName -Raw | ConvertFrom-Json
    } catch {
        Write-Host "  Skipping $($f.Name) - failed to parse JSON" -ForegroundColor Yellow
        continue
    }
    Merge-Missing $obj $ref
    # Write back with depth large enough
    $json = $obj | ConvertTo-Json -Depth 20
    $json | Out-File -FilePath $f.FullName -Encoding UTF8
    Write-Host "  Wrote $($f.Name)"
}
Write-Host "Done merging missing keys (English placeholders)." -ForegroundColor Green
