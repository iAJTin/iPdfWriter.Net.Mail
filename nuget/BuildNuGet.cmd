@ECHO OFF
CLS

..\src\.nuget\nuget Pack iPdfWriter.Net.Mail.1.0.0.nuspec -NoDefaultExcludes -NoPackageAnalysis -OutputDirectory ..\deployment\nuget 

pause

