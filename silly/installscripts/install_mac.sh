#!/bin/bash

abort()
{
    echo "******* ABORTING *******"
    echo "$1"

    exit 1
}

set -e

echo "------------------------------------------------------------------------"
echo "WELCOME to Silly Widgets v0.1!"
echo "This script will install Silly Widgets to your /usr/local/lib directory."
echo "This will be straightforward and hopefully not too painful. Thanks!"
echo "Let's get started..."
echo "------------------------------------------------------------------------"

INSTALL_DIR="/usr/local/lib/sillywidgets"
EXEC_DIR="/usr/local/bin"
PAYLOAD="silly"

if [ ! -f "$PAYLOAD" ]; then
    abort "silly executable not found in current directory and I don't know where it's at. Try running install from the directory you unzipped the downloaded archive. Sorry."
fi

if [ -d "$INSTALL_DIR" ]; then
    if [ -L "$INSTALL_DIR" ]; then
        abort "Cannot install because $INSTALL_DIR is a symlink, which is very weird. You'll have to manually remove the symlink before installing."
    else
        echo "Cleaning existing installation, $INSTALL_DIR ..."
        rm -rf "$INSTALL_DIR"
        rm -f "$EXEC_DIR/silly"
    fi
fi

echo "Creating install directory, $INSTALL_DIR ..."
mkdir "$INSTALL_DIR"

echo "Installing components to $INSTALL_DIR ..."
rsync -av --exclude="install.sh" ./ "$INSTALL_DIR"

echo "Creating link to silly in $EXEC_DIR ..."
ln -s "$INSTALL_DIR/silly" "$EXEC_DIR/silly"

echo "Testing install ..."
if silly ; then
    echo ""
    echo "We all good!!"
else
    echo "Oh, fuck, something's screwed up."
fi

echo "------------------------------------------------------------------------"
echo "Finished!"
echo ""