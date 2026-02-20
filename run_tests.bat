@echo off
echo =================================
echo Vault Redirector Test Runner
echo =================================

echo Running Legacy C# Tests...
dotnet test

echo.
echo =================================
echo Running Electron GUI Tests...
echo =================================
cd vault-redirector-gui

if not exist "node_modules" (
    echo Installing dependencies...
    call npm install
)

call npm test

echo.
echo All tests finished.
pause
