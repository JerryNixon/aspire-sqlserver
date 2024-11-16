@echo off

sqlcmd delete --force

SET SQLCMD_ACCEPT_EULA=YES
sqlcmd.exe create mssql --use mssql.sql --user-database trek --port 1234 --add-on dab --add-on-use dab-config.json --cached
sqlcmd.exe query "CREATE LOGIN Student WITH PASSWORD = 'P@ssw0rd!';"
sqlcmd.exe query "ALTER SERVER ROLE sysadmin ADD MEMBER Student;"
REM echo 127.0.0.1,1234