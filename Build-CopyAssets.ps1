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
	CopyItemWithAssert '.\src\ConfigServer.Gui\wwwroot\Assets\lib\deeppurple-amber.css' $assetLibPath
}

function AssertAssets {
	Write-Host "Checking asset have been generated and copied"
	AssertPath '.\src\ConfigServer.Server\Assets\app.js'
	AssertPath '.\src\ConfigServer.Server\Assets\styles.css'	
	AssertPath '.\src\ConfigServer.Server\Assets\lib\shim.min.js' 
	AssertPath '.\src\ConfigServer.Server\Assets\lib\system.js' 
	AssertPath '.\src\ConfigServer.Server\Assets\lib\zone.min.js'
	AssertPath '.\src\ConfigServer.Server\Assets\lib\deeppurple-amber.css'
}

CopyAssets
AssertAssets


