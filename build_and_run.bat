@echo off
echo =================================
echo Vault Redirector GUI Launcher
echo =================================

cd vault-redirector-gui

if not exist "node_modules" (
    echo Installing dependencies...
    call npm install
)

echo Starting application...
call npm start

echo.
echo Application finished.
pause
