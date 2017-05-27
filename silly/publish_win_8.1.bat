:: This script requires 7 zip to create the final archive.
:: If you don't have 7 zip and don't care about creating the archive,
:: comment out lines 14 to 15.

echo OFF
echo -----------------------------------------------------
echo Publishing for Windows 8.1 x64
echo ***** Resolving dependencies
dotnet restore
echo ***** Building deployment package
dotnet publish -c Release -r win81-x64
echo ***** Copying install script
copy .\installscripts\INSTALL_WIN.TXT .\bin\Release\netcoreapp1.1\win81-x64\INSTALL.TXT
echo ***** Creating archive
7z a -t7z .\bin\Release\netcoreapp1.1\win81-x64\sillywidgets-v0.1.7z .\bin\Release\netcoreapp1.1\win81-x64\*
echo ***** Complete
echo -----------------------------------------------------