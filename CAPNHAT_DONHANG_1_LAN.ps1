#Requires -Version 5.1
param([string]$ServerName='(local)')
$ErrorActionPreference='Stop'
$filepath=Join-Path $PSScriptRoot 'Database\04_ThemPhiGiaoHang.sql'
$builder=New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source']=$ServerName
$builder['Initial Catalog']='master'
$builder['Integrated Security']=$true
$builder['TrustServerCertificate']=$true
$builder['Connect Timeout']=20
$conn=New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
try {
    $conn.Open()
    $sql=[IO.File]::ReadAllText($filepath,[Text.Encoding]::UTF8)
    foreach($batch in [Text.RegularExpressions.Regex]::Split($sql,'(?im)^[ \t]*GO[ \t]*(?:--[^\r\n]*)?(?:\r?\n|$)')) {
        if([string]::IsNullOrWhiteSpace($batch)){continue}
        $cmd=$conn.CreateCommand()
        $cmd.CommandText=$batch
        try{[void]$cmd.ExecuteNonQuery()}finally{$cmd.Dispose()}
    }
    $cmd=$conn.CreateCommand()
    $cmd.CommandText="SELECT COUNT(*) FROM [MyStore].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Order' AND COLUMN_NAME='ShippingFee'"
    $count=[int]$cmd.ExecuteScalar();$cmd.Dispose()
    if($count -ne 1){throw 'Chua them duoc cot ShippingFee.'}
    Write-Host "SUCCESS - ShippingFee ready on $ServerName. No old orders removed." -ForegroundColor Green
    exit 0
} catch {
    Write-Host 'DATABASE UPDATE FAILED' -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
} finally {$conn.Dispose()}
