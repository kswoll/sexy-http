REM "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" SexyReact.Fody.sln

cd Nuget

..\packages\NugetUtilities.1.0.8\UpdateVersion.exe SexyHttp.nuspec -Increment

mkdir build
cd build

copy ..\SexyHttp.nuspec .

mkdir lib
mkdir lib\net45

copy ..\..\SexyHttp.Net45\bin\Debug\SexyHttp.* lib\net45

..\nuget pack SexyHttp.nuspec

copy *.nupkg ..

cd ..
rmdir build /S /Q
cd ..