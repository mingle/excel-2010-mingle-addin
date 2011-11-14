@ECHO OFF
IF NOT "%~f0" == "~f0" GOTO :WinNT
@"ir.exe" "C:/code/solutions_office/tools/ironruby/bin/update_rubygems" %1 %2 %3 %4 %5 %6 %7 %8 %9
GOTO :EOF
:WinNT
@"ir.exe" "%~dpn0" %*
