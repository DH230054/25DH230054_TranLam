@echo off
setlocal
cd /d "%~dp0"
echo MyStore - Installing database on SCHOOL SQL Server: (local)
echo Auth: Windows Authentication
 echo Nothing will be deleted from an existing MyStore database.
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0CAIDAT_DATABASE_1_LAN.ps1" -ServerName "(local)"
if errorlevel 1 (
 echo.
 echo Failed. Take a picture of the error message.
 echo If server name differs, run CHON_SERVER_SQL.cmd instead.
 pause
 exit /b 1
)
echo.
echo Ready. Open MyStore_MVC5_Full.sln and run with Visual Studio 2022.
pause
