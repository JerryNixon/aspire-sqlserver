@echo off
setlocal

set PROJECT=AlphaDynamics.SqlRepository
set STARTUP=AlphaDynamics.WebApi

for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format \"MMdd_HHmmss\""') do set MIG=Update_%%I

echo Creating migration: %MIG%
dotnet ef migrations add %MIG% --project %PROJECT% --startup-project %STARTUP%

echo Applying migration...
dotnet ef database update --project %PROJECT% --startup-project %STARTUP%

echo ✅ Migration created and applied.
endlocal
pause
