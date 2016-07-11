if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

EnsurePsbuildInstalled

exec { & dotnet restore }

Invoke-MSBuild

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$revision = "{0:D4}" -f [convert]::ToInt32($revision, 10)

exec { & dotnet test .\test\ConfigServer.Core.Test -c Release }

exec { & dotnet pack .\src\ConfigServer.Core -c Release -o .\artifacts --version-suffix=$revision }  
exec { & dotnet pack .\src\ConfigServer.FileProvider -c Release -o .\artifacts --version-suffix=$revision }  
exec { & dotnet pack .\src\ConfigServer.InMemoryProvider -c Release -o .\artifacts --version-suffix=$revision }  
exec { & dotnet pack .\src\ConfigServer.Configurator -c Release -o .\artifacts --version-suffix=$revision }

