pushd %~dp0

dotnet build -c Release

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage1Org.AdoNet >log.txt

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage1Mod.AdoNet >>log.txt

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage2Org.AdoNet >>log.txt

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage2Mod.AdoNet >>log.txt

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage3Org.AdoNet >>log.txt

REM sc stop MSSQLServer
REM WAITFOR Nothing /T 5
REM sc start MSSQLServer
REM WAITFOR Nothing /T 5
dotnet run -c Release -- GrainStorage3Mod.AdoNet >>log.txt

echo " - fini -"

popd