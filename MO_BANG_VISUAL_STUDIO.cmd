@echo off
cd /d "%~dp0"
if not exist "MyStore_MVC5_Full.sln" (
  echo Khong tim thay file MyStore_MVC5_Full.sln.
  pause
  exit /b 1
)
start "" "MyStore_MVC5_Full.sln"
