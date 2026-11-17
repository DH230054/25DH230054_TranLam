@echo off
setlocal EnableExtensions
cd /d "%~dp0"
echo ==============================================
echo     MyStore - Choose a SQL Server instance
echo ==============================================
echo 1. School: (local)  [default]
echo 2. Home:   (localdb)\MSSQLLocalDB
echo 3. SQL Express: .\SQLEXPRESS
echo 4. Enter another server name
set /p CHOICE=Select 1-4 [1]: 
if "%CHOICE%"=="" set CHOICE=1
if "%CHOICE%"=="1" set "SERVER=(local)"
if "%CHOICE%"=="2" set "SERVER=(localdb)\MSSQLLocalDB"
if "%CHOICE%"=="3" set "SERVER=.\SQLEXPRESS"
if "%CHOICE%"=="4" set /p SERVER=Server name: 
if not defined SERVER (
 echo Invalid choice.
 pause
 exit /b 1
)
echo.
echo Target SQL Server: %SERVER%
echo Using Windows Authentication of current user.
echo This may require CREATE DATABASE permission.
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0CAIDAT_DATABASE_1_LAN.ps1" -ServerName "%SERVER%"
if errorlevel 1 (
 echo Failed. Check the message above and SQL Server connection.
 pause
 exit /b 1
)
echo.
echo Database and Web.config are now set to the SAME server.
pause
