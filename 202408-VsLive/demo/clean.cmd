@echo off
setlocal enabledelayedexpansion

cd..

REM Loop through and delete all solution files in the current directory
for %%i in (*.sln) do (
    del /f /q "%%i" 2>nul
)

REM Loop through all directories in the current directory, excluding /demo and /solution
for /d %%i in (*) do (
    if /i not "%%i"=="demo" if /i not "%%i"=="solution" (
        rmdir /s /q "%%i" 2>nul
    )
)

echo Clean-up complete!
