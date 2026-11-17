#Requires -Version 5.1
$serverName = '(local)'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile = Join-Path $scriptRoot 'Database\03_CapNhat_Anh_SanPham.sql'
$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source'] = $serverName
$builder['Initial Catalog'] = 'master'
$builder['Integrated Security'] = $true
$builder['Connect Timeout'] = 30
$conn = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
function Invoke-SqlFile([string]$path) {
  $sql = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
  $batches = [System.Text.RegularExpressions.Regex]::Split($sql, '(?im)^\s*GO\s*(?:--[^\n]*)?(?:\r?\n|$)')
  foreach($batch in $batches){
    if([string]::IsNullOrWhiteSpace($batch)){ continue }
    $cmd = $conn.CreateCommand(); $cmd.CommandText = $batch; $cmd.CommandTimeout = 120
    [void]$cmd.ExecuteNonQuery(); $cmd.Dispose()
  }
}
try {
  $conn.Open()
  Invoke-SqlFile $sqlFile
  Write-Host 'SUCCESS - Da cap nhat duong dan anh san pham.' -ForegroundColor Green
  Write-Host 'Hay mo lai website va nhan Ctrl+F5 de thay anh.' -ForegroundColor Green
} catch {
  Write-Host 'UPDATE PRODUCT IMAGES FAILED.' -ForegroundColor Red
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit 1
} finally {
  if($conn){ $conn.Dispose() }
}
