@echo off
echo Restoring packages...
dotnet restore

echo Building solution...
dotnet build --no-restore

echo Running application...
dotnet run --project src/UI/VaultRedirector.UI.csproj -- %*

echo.
echo =================================
echo Application finished.
echo If it crashed, please check 'crash_log.txt' and 'debug_engine.log'.
echo =================================
pause
