@echo off
chcp 65001 >nul
cd /d "%~dp0"
title Statlocker filter
echo Starting. Leave this window open. Close it when you are done.
runtime\node.exe app\launcher.mjs
if errorlevel 1 (
  echo.
  echo The filter did not start. See the message above.
  pause
)
