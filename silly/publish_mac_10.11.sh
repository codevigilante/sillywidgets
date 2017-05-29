#!/bin/bash

set -e

BUILD_CMD="dotnet publish -c Release -r osx.10.11-x64"
MAC_INSTALL="cp ./installscripts/install_mac.sh ./bin/Release/netcoreapp1.1/osx.10.11-x64/install.sh"
CD_CMD="cd bin/Release/netcoreapp1.1/osx.10.11-x64"
ARCHIVE_CMD="zip -r -X ../../sillywidgets-v0.2-osx10.11.zip *"

echo "-----------------------------------------------------"
echo "$BUILD_CMD"
$BUILD_CMD
echo "$MAC_INSTALL"
$MAC_INSTALL
echo "$CD_CMD"
$CD_CMD
echo "$ARCHIVE_CMD"
$ARCHIVE_CMD
echo "-----------------------------------------------------"
echo "done"
