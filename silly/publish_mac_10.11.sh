#!/bin/bash

set -e

BUILD_CMD="dotnet publish -c Release -r osx.10.11-x64"
MAC_INSTALL="cp ./installscripts/install_mac.sh ./bin/Release/netcoreapp1.1/osx.10.11-x64/install.sh"
ARCHIVE_CMD="zip -r ./bin/Release/netcoreapp1.1/osx.10.11-x64/sillywidgets-v0.2.zip ./bin/Release/netcoreapp1.1/osx.10.11-x64/"

echo "-----------------------------------------------------"
echo "$BUILD_CMD"
$BUILD_CMD
echo "$MAC_INSTALL"
$MAC_INSTALL
echo "$ARCHIVE_CMD"
$ARCHIVE_CMD
echo "-----------------------------------------------------"
echo "done"