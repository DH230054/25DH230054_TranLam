@echo off
setlocal
set SCRIPT_DIR=%~dp0
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%CAPNHAT_ANH_SANPHAM_1_LAN.ps1"
echo.
echo Press any key to continue . . .
pause >nul
