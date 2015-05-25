properties {
	$base_directory = Resolve-Path . 
	$src_directory = "$base_directory\source"
	$output_directory = "$base_directory\build"
	$dist_directory = "$base_directory\distribution"
	$sln_file = "$src_directory\IdentityServer3.SiteFinity.sln"
	$target_config = "Release"
	$framework_version = "v4.5"
	$nunit_path = "$src_directory\packages\NUnit.Runners.2.6.4\tools\nunit.exe"
	$ilmerge_path = "$src_directory\packages\ilmerge.2.14.1208\tools\ILMerge.exe"
	$nuget_path = "$src_directory\source\.nuget\Nugetnuget.exe"
	
	$buildNumber = 0;
	$version = "1.0.0.0"
	$preRelease = $null
}

task default -depends Clean, CreateNuGetPackage
task appVeyor -depends Clean, CreateNuGetPackage

task Clean {
	rmdir $output_directory -ea SilentlyContinue -recurse
	rmdir $dist_directory -ea SilentlyContinue -recurse
	exec { msbuild /nologo /verbosity:quiet $sln_file /p:Configuration=$target_config /t:Clean }
}

task Compile -depends UpdateVersion {
	exec { msbuild /nologo /verbosity:q $sln_file /p:Configuration=$target_config /p:TargetFrameworkVersion=v4.5 }
}

task UpdateVersion {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$assemblyFileVersion =  "$major.$minor.$patch.$buildNumber"
	$assemblyVersion = "$major.$minor.0.0"
	$versionAssemblyInfoFile = "$src_directory/VersionAssemblyInfo.cs"
	"using System.Reflection;" > $versionAssemblyInfoFile
	"" >> $versionAssemblyInfoFile
	"[assembly: AssemblyVersion(""$assemblyVersion"")]" >> $versionAssemblyInfoFile
	"[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]" >> $versionAssemblyInfoFile
}

task ILMerge -depends Compile {
	$input_dlls = "$output_directory\IdentityServer3.SiteFinity.dll"

	Get-ChildItem -Path $output_directory -Filter *.dll |
		foreach-object {
			if ("$_" -ne "IdentityServer3.SiteFinity.dll" -and
				"$_" -ne "Thinktecture.IdentityServer3.dll" -and 
			    "$_" -ne "Owin.dll") {
				$input_dlls = "$input_dlls $output_directory\$_"
			}
	}

	New-Item $dist_directory\lib\net45 -Type Directory
	Invoke-Expression "$ilmerge_path /targetplatform:v4 /internalize /allowDup /target:library /out:$dist_directory\lib\net45\IdentityServer3.SiteFinity.dll $input_dlls"
}

task CreateNuGetPackage -depends ILMerge {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$packageVersion =  "$major.$minor.$patch"
	if($preRelease){
		$packageVersion = "$packageVersion-$preRelease" 
	}

	if ($buildNumber -ne 0){
		$packageVersion = $packageVersion + "-build" + $buildNumber.ToString().PadLeft(5,'0')
	}

	copy-item $src_directory\IdentityServer3.SiteFinity.nuspec $dist_directory
	copy-item $output_directory\IdentityServer3.SiteFinity.xml $dist_directory\lib\net45\
	exec { . $nuget_path pack $dist_directory\IdentityServer3.SiteFinity.nuspec -BasePath $dist_directory -o $dist_directory -version $packageVersion }
}
