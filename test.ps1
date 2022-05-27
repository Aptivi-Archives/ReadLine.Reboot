dotnet test .\ReadLine.Reboot.Tests\ReadLine.Reboot.Tests.csproj
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }