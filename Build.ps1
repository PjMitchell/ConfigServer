cd c:
<#  
.SYNOPSIS
    You can add this to you build script to ensure that psbuild is available before calling
    Invoke-MSBuild. If psbuild is not available locally it will be downloaded automatically.
#>
function EnsurePsbuildInstalled{  
    [cmdletbinding()]
    param(
        [string]$psbuildInstallUri = 'https://raw.githubusercontent.com/ligershark/psbuild/master/src/GetPSBuild.ps1'
    )
    process{
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            'Installing psbuild from [{0}]' -f $psbuildInstallUri | Write-Verbose
            (new-object Net.WebClient).DownloadString($psbuildInstallUri) | iex
        }
        else{
            'psbuild already loaded, skipping download' | Write-Verbose
        }

        # make sure it's loaded and throw if not
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            throw ('Unable to install/load psbuild from [{0}]' -f $psbuildInstallUri)
        }
    }
}

# Taken from psake https://github.com/psake/psake

<#  
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec  
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

function ExecuteGulpTasks
{
	Push-Location "./src/ConfigServer.Gui"  
	Write-Host "npm package restore"	
	& "npm" install
	if ($LastExitCode -ne 0) {
		Write-Error "Npm package restore failed";
		exit 1;
	}
	npm install --global gulp

	Write-Host "gulp build"
	& "gulp" BuildPackageAssets
	if ($LastExitCode -ne 0) {
		Write-Error "gulp asset package failed";
		exit 1;
	}

	Write-Host "gulp TsLint"
	& "gulp" TsLint
	if ($LastExitCode -ne 0) {
		Write-Error "gulp tslint failed";
		exit 1;
	}
	npm run build
	Pop-Location
}

function AssertPath {
	[CmdletBinding()]	
	param(
        [Parameter(Position=0,Mandatory=1)][string]$path
    )
	if( (Test-Path $path) -eq $false) {
		Write-Error "Asset $path not found";
		exit 1;
	}
}

function CopyItemWithAssert {
	[CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][string]$path,
        [Parameter(Position=1,Mandatory=0)][string]$target
    )
	Copy-Item $path $target
	AssertPath $path
}

function CopyAssets {
	$assetPath = '.\src\ConfigServer.Server\Assets'
	$assetLibPath = '.\src\ConfigServer.Server\Assets\lib'
	New-Item -Path $assetPath -type directory
	New-Item -Path $assetLibPath -type directory
	Write-Host "Copying Assets from ConfigServer.Gui to ConfigServer.Server"
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\app.js' $assetPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\styles.css' $assetPath
	Write-Host "Copying Assets/lib from ConfigServer.Gui to ConfigServer.Server"
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\lib\shim.min.js' $assetLibPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\lib\system.js' $assetLibPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\lib\zone.min.js' $assetLibPath
}

function AssertAssets {
	Write-Host "Checking asset have been generated and copied"
	AssertPath '.\src\ConfigServer.Server\Assets\app.js'
	AssertPath '.\src\ConfigServer.Server\Assets\styles.css'	
	AssertPath '.\src\ConfigServer.Server\Assets\lib\shim.min.js' 
	AssertPath '.\src\ConfigServer.Server\Assets\lib\system.js' 
	AssertPath '.\src\ConfigServer.Server\Assets\lib\zone.min.js'
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

EnsurePsbuildInstalled
exec { & dotnet --info }
exec { & dotnet restore }
ExecuteGulpTasks
CopyAssets
AssertAssets

exec { & dotnet build -c Release}

exec { & dotnet test .\test\ConfigServer.Core.Tests\ConfigServer.Core.Tests.csproj -c Release }

exec { & dotnet pack .\src\ConfigServer.Core\ConfigServer.Core.csproj -c Release -o .\artifacts }  
exec { & dotnet pack .\src\ConfigServer.Server\ConfigServer.Server.csproj -c Release -o .\artifacts }  
exec { & dotnet pack .\src\ConfigServer.Client\ConfigServer.Client.csproj -c Release -o .\artifacts }
exec { & dotnet pack .\src\ConfigServer.Client.Builder\ConfigServer.Client.Builder.csproj -c Release -o .\artifacts }  

exec { & dotnet pack .\src\ConfigProviders\ConfigServer.TextProvider.Core\ConfigServer.TextProvider.Core.csproj -c Release -o .\artifacts }  
exec { & dotnet pack .\src\ConfigProviders\ConfigServer.FileProvider\ConfigServer.FileProvider.csproj -c Release -o .\artifacts }  
exec { & dotnet pack .\src\ConfigProviders\ConfigServer.InMemoryProvider\ConfigServer.InMemoryProvider.csproj -c Release -o .\artifacts }
exec { & dotnet pack .\src\ConfigProviders\ConfigServer.AzureBlobStorageProvider\ConfigServer.AzureBlobStorageProvider.csproj -c Release -o .\artifacts }  


