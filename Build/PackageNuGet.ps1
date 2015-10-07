param($scriptRoot)

$ErrorActionPreference = "Stop"

$msBuild = "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$nuGet = "$scriptRoot..\tools\NuGet.exe"
$solution = "$scriptRoot\..\MicroCHAP.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

& $nuGet pack "$scriptRoot\..\src\MicroCHAP\MicroCHAP.csproj" -Symbols -Prop "Configuration=Release"