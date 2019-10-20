Framework '4.5.1x86'

properties {
	$base_dir = resolve-path .
	$source_dir = "$base_dir\src"
	$result_dir = "$base_dir\results"
	$artifacts_dir = "$base_dir\artifacts"
	$global:config = "debug"
}


task default -depends local
task local -depends init, compile, test
task ci -depends clean, release, local

task clean {
	Remove-Item "$artifacts_dir" -recurse -force  -ErrorAction SilentlyContinue | out-null
	Remove-Item "$result_dir" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task init {

	# Make sure per-user dotnet is installed

	Install-Dotnet
}

task release {

	$global:config = "release"

}

task compile -depends clean {

	$tag = $(git tag -l --points-at HEAD)
	$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
	$suffix = @{ $true = ""; $false = "ci-$revision"}[$tag -ne $NULL -and $revision -ne "local"]
	$commitHash = $(git rev-parse --short HEAD)
	$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

	$buildParam = @{ $true = ""; $false = "--version-suffix=$buildSuffix"}[$tag -ne $NULL -and $revision -ne "local"]
	$packageParam = @{ $true = ""; $false = "--version-suffix=$suffix"}[$tag -ne $NULL -and $revision -ne "local"]

	Write-Output "build: Tag is $tag"
	Write-Output "build: Package version suffix is $suffix"
	Write-Output "build: Build version suffix is $buildSuffix"

	# restore all project references (creating project.assets.json for each project)
	exec { dotnet restore $base_dir\AutoMapper.Collection.EFCore.sln /nologo }

	exec { dotnet build $base_dir\AutoMapper.Collection.EFCore.sln -c $config $buildParam --no-restore /nologo }

	exec { dotnet pack $base_dir\AutoMapper.Collection.EFCore.sln -c $config --include-symbols --no-build --no-restore --output $artifacts_dir $packageParam /nologo}

}

task test {

	exec { dotnet test $source_dir\AutoMapper.Collection.EntityFrameworkCore.Tests -c $config --no-build --no-restore --results-directory $result_dir --logger trx /nologo }

}

function Install-Dotnet
{
    $dotnetcli = Get-CommandLocation -command 'dotnet'

    if ($null -eq $dotnetcli)
    {
		$dotnetPath = "$pwd\.dotnet"
		$dotnetCliVersion = if ($null -eq $env:DOTNET_CLI_VERSION) { 'Latest' } else { $env:DOTNET_CLI_VERSION }
		[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; 
		&([scriptblock]::Create((Invoke-WebRequest -useb 'https://dot.net/v1/dotnet-install.ps1'))) -Channel "LTS" -version $dotnetCliVersion -InstallDir $dotnetPath -NoPath
		$env:Path = "$dotnetPath;$env:Path"
	}
}

function Get-CommandLocation {
	param ($command)
    (Get-ChildItem env:\path).Value.split(';') | `
        Where-Object { $_ } | `
        ForEach-Object{ [System.Environment]::ExpandEnvironmentVariables($_) } | `
        Where-Object { test-path $_ } |`
        ForEach-Object{ Get-ChildItem "$_\*" -include *.bat,*.exe,*.cmd } | `
        ForEach-Object{  $file = $_.Name; `
            if ($file -and ($file -eq $command -or `
			    $file -eq ($command + '.exe') -or  `
			    $file -eq ($command + '.bat') -or  `
			    $file -eq ($command + '.cmd'))) `
            { `
                $_.FullName `
            } `
        } | `
        Select-Object -unique
}