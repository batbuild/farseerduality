REM @echo off
cls
REM ".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"

"packages\FAKE\tools\Fake.exe" Build.fsx %* 
