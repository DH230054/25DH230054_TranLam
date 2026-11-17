@echo off
setlocal
cd /d "%~dp0"
echo Updating MyStore orders on SQL Server (local), Windows Authentication...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0CAPNHAT_DONHANG_1_LAN.ps1" -ServerName "(local)"
if errorlevel 1 (
  echo Failed. If your server is different, run the .ps1 script with -ServerName "YOUR_SERVER".
) else (
  echo Done. Open the Visual Studio solution and press F5.
)
pause
