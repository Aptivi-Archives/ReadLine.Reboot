param(
    [string]$p1 = "Release"
)

dotnet restore
dotnet build ".\ReadLine.Reboot" -c $p1
dotnet build ".\ReadLine.Reboot.Demo" -c $p1
dotnet build ".\ReadLine.Reboot.Tests" -c $p1
