
@echo off
setlocal ENABLEDELAYEDEXPANSION


:: --- Konfiguracja ---
set BASE_URL=https://localhost:44344/taskflow
set ADMIN_USER=Admin
set ADMIN_PASS=Password
set REGULAR_USER=User
set REGULAR_PASS=Password
set LOG_FILE=api_test.log

:: --- Inicjalizacja logu ---
echo. > %LOG_FILE%
echo [INFO] API Test Run Started: %DATE% %TIME% >> %LOG_FILE%
echo =================================================== >> %LOG_FILE%

:: --- Krok 1: Uwierzytelnianie jako Admin ---
echo [STEP 1] Authenticating as Admin (%ADMIN_USER%)...
curl -k -s -X POST "%BASE_URL%/auth/login" -H "Content-Type: application/json" -d "{\"userName\":\"%ADMIN_USER%\", \"password\":\"%ADMIN_PASS%\"}" > tmp_login_admin.json
for /f "tokens=2 delims=:" %%a in ('findstr "bearer" tmp_login_admin.json') do set "RAW_ADMIN_TOKEN=%%a"
set "ADMIN_TOKEN=%RAW_ADMIN_TOKEN:~1,-2%"
del tmp_login_admin.json

if "!ADMIN_TOKEN!"=="" (
    echo [FAIL] Admin authentication failed. Terminating. >> %LOG_FILE%
    goto :end
)
echo [ OK ] Admin authentication successful. >> %LOG_FILE%
echo. >> %LOG_FILE%

:: --- Krok 2: Operacje jako Admin (tworzenie i odczyt) ---
echo [STEP 2] Admin is creating a new project 'TESTPROJ'...
curl -k -s -o NUL -w "[ OK ] Create project 'TESTPROJ' status: %%{http_code}\n" -X POST "%BASE_URL%/projects/create" -H "Authorization: Bearer !ADMIN_TOKEN!" -H "Content-Type: application/json" -d "{\"projectKey\": \"TESTPROJ\", \"name\": \"Project From Script\", \"userNames\": [\"%REGULAR_USER%\"]}" >> %LOG_FILE%

echo [STEP 2] Admin is fetching details of the new project...
curl -k -s -o NUL -w "[ OK ] Get project 'TESTPROJ' details status: %%{http_code}\n" "%BASE_URL%/projects/testproj" -H "Authorization: Bearer !ADMIN_TOKEN!" >> %LOG_FILE%

echo [STEP 2] Admin is adding a task to 'TESTPROJ'...
curl -k -s -o NUL -w "[ OK ] Add task 'TESTPROJ-1' status: %%{http_code}\n" -X POST "%BASE_URL%/projects/testproj/AddTask" -H "Authorization: Bearer !ADMIN_TOKEN!" -H "Content-Type: application/json" -d "{\"title\": \"Task created by script\", \"description\": \"This task should be completed and then deleted.\", \"assigneeUserName\": \"%REGULAR_USER%\"}" >> %LOG_FILE%
echo. >> %LOG_FILE%

:: --- Krok 3: Uwierzytelnianie jako zwykły Użytkownik ---
echo [STEP 3] Authenticating as Regular User (%REGULAR_USER%)...
curl -k -s -X POST "%BASE_URL%/auth/login" -H "Content-Type: application/json" -d "{\"userName\":\"%REGULAR_USER%\", \"password\":\"%REGULAR_PASS%\"}" > tmp_login_user.json
for /f "tokens=2 delims=:" %%a in ('findstr "bearer" tmp_login_user.json') do set "RAW_USER_TOKEN=%%a"
set "USER_TOKEN=%RAW_USER_TOKEN:~1,-2%"
del tmp_login_user.json

if "!USER_TOKEN!"=="" (
    echo [FAIL] User authentication failed. Terminating. >> %LOG_FILE%
    goto :end
)
echo [ OK ] User authentication successful. >> %LOG_FILE%
echo. >> %LOG_FILE%

:: --- Krok 4: Operacje jako zwykły Użytkownik ---
echo [STEP 4] User is checking their assigned tasks...
curl -k -s -o NUL -w "[ OK ] Get 'mine' tasks status (should contain 'TESTPROJ-1'): %%{http_code}\n" "%BASE_URL%/tasks/mine" -H "Authorization: Bearer !USER_TOKEN!" >> %LOG_FILE%
echo. >> %LOG_FILE%

:: --- Krok 5: Zakończenie cyklu przez Admina ---
echo [STEP 5] Admin is completing the task 'TESTPROJ-1'...
curl -k -s -o NUL -w "[ OK ] Complete task 'TESTPROJ-1' status: %%{http_code}\n" -X PATCH "%BASE_URL%/tasks/testproj-1" -H "Authorization: Bearer !ADMIN_TOKEN!" -H "Content-Type: application/json" -d "{\"operation\": \"complete\"}" >> %LOG_FILE%

echo [STEP 5] Admin is deleting the project 'TESTPROJ' to clean up...
curl -k -s -o NUL -w "[ OK ] Delete project 'TESTPROJ' status: %%{http_code}\n" -X DELETE "%BASE_URL%/projects/testproj/delete" -H "Authorization: Bearer !ADMIN_TOKEN!" >> %LOG_FILE%
echo. >> %LOG_FILE%

:end
echo =================================================== >> %LOG_FILE%
echo [INFO] API Test Run Finished. See %LOG_FILE% for details.
echo.
pause