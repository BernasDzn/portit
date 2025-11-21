$path = Join-Path $PSScriptRoot 'placeholders_report.json'
if (-Not (Test-Path $path)) { Write-Error "File not found: $path"; exit 1 }
$data = Get-Content -Raw $path | ConvertFrom-Json
$summary = @{}
foreach ($p in $data.PSObject.Properties) { $summary[$p.Name] = $p.Value.Count }
$summary.GetEnumerator() | Sort-Object -Property Value -Descending | ForEach-Object { Write-Output ("{0}: {1}" -f $_.Name, $_.Value) }
