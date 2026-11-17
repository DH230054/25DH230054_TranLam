#Requires -Version 5.1
param(
    [Parameter(Mandatory=$false)]
    [string]$ServerName = '(local)'
)
# SQL Server MyStore setup. Uses current Windows account. Does not delete existing database or rows.
$ErrorActionPreference = 'Stop'
$dbFolder = Join-Path $PSScriptRoot 'Database'
$configFile = Join-Path $PSScriptRoot 'MyStore.Web\Web.config'
$files = @('01_Create_MyStore.sql', '02_Seed_SampleData.sql', '03_CapNhat_Anh_SanPham.sql', '04_ThemPhiGiaoHang.sql')
$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source'] = $ServerName
$builder['Initial Catalog'] = 'master'
$builder['Integrated Security'] = $true
$builder['Connect Timeout'] = 20
$builder['TrustServerCertificate'] = $true
$connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)

function Invoke-SqlBatches([string]$filePath) {
    $sql = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
    # GO is an SSMS batch separator, not SQL syntax.
    $batches = [System.Text.RegularExpressions.Regex]::Split(
        $sql,
        '(?im)^[ \t]*GO[ \t]*(?:--[^\r\n]*)?(?:\r?\n|$)'
    )
    $batchNumber = 0
    foreach ($batch in $batches) {
        if ([string]::IsNullOrWhiteSpace($batch)) { continue }
        $batchNumber++
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = $batch
        $cmd.CommandTimeout = 120
        try {
            [void]$cmd.ExecuteNonQuery()
        } catch {
            throw "SQL ERROR in $([IO.Path]::GetFileName($filePath)) batch $batchNumber : $($_.Exception.Message)"
        } finally {
            $cmd.Dispose()
        }
    }
    Write-Host "OK - $([IO.Path]::GetFileName($filePath)) ($batchNumber batches)" -ForegroundColor Green
}

function Set-WebConfigServer([string]$server) {
    # Use the SQL connection string builder to correctly escape unusual instance names.
    [xml]$xml = Get-Content -LiteralPath $configFile -Raw -Encoding UTF8
    $node = $xml.configuration.connectionStrings.add | Where-Object { $_.name -eq 'MyStoreEntities' } | Select-Object -First 1
    if ($null -eq $node) { throw 'Cannot find MyStoreEntities in MyStore.Web/Web.config' }
    $webBuilder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new([string]$node.connectionString)
    $webBuilder['Data Source'] = $server
    $node.SetAttribute('connectionString', $webBuilder.ConnectionString)
    # No backup of the existing password is needed: Windows Authentication only.
    $xml.Save($configFile)
    Write-Host "Web.config updated for SQL Server: $server" -ForegroundColor Green
}

try {
    if ([string]::IsNullOrWhiteSpace($ServerName)) { throw 'Please enter a server name.' }
    Write-Host "Connecting to SQL Server: $ServerName"
    Write-Host 'Authentication: Windows Authentication'
    $connection.Open()
    Write-Host 'Connected.' -ForegroundColor Green
    foreach($file in $files) {
        $fullPath = Join-Path $dbFolder $file
        if (-not (Test-Path -LiteralPath $fullPath)) { throw "Cannot find: $fullPath" }
        Write-Host "Running: $file"
        Invoke-SqlBatches $fullPath
    }
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = 'SELECT COUNT(*) FROM [MyStore].dbo.Product;'
    $count = [int]$cmd.ExecuteScalar()
    $cmd.Dispose()
    Set-WebConfigServer $ServerName
    Write-Host "SUCCESS - MyStore database ready. Products: $count" -ForegroundColor Green
    Write-Host 'Open MyStore_MVC5_Full.sln using Visual Studio 2022, rebuild and press F5.'
    exit 0
} catch {
    Write-Host 'DATABASE SETUP FAILED:' -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host 'If permission denied: use SSMS with an authorized Windows account.'
    Write-Host 'If connection refused: verify Server Name and SQL Server service.'
    exit 1
} finally {
    if ($null -ne $connection) { $connection.Dispose() }
}
