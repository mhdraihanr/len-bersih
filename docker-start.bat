@echo off
REM filepath: d:\a-csharp\Len-Bersih\docker-start.bat
echo ========================================
echo  Len Bersih MariaDB Docker Setup
echo ========================================
echo.

echo [1/3] Starting MariaDB container...
docker-compose up -d mariadb
timeout /t 10 /nobreak > nul

echo [2/3] Waiting for database initialization...
docker-compose logs mariadb | findstr "ready for connections"
timeout /t 5 /nobreak > nul

echo [3/3] Starting phpMyAdmin...
docker-compose up -d phpmyadmin

echo.
echo ========================================
echo  Setup Complete!
echo ========================================
echo.
echo MariaDB:     localhost:3306
echo Database:    lenbersih
echo Username:    lenbersih_user
echo Password:    LenBersih_Pass123!
echo Root Pass:   LenBersih2025!
echo.
echo phpMyAdmin:  http://localhost:8080
echo.
echo Press any key to view logs...
pause > nul
docker-compose logs -f