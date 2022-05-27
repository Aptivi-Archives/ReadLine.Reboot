#!/usr/bin/env bash

if [[ !$1 ]]; then
    CONFIGURATION="Release"
fi

if [[ $1 ]]; then
    CONFIGURATION=$1
fi

dotnet restore
dotnet build ./ReadLine.Reboot -c $CONFIGURATION
dotnet build ./ReadLine.Reboot.Demo -c $CONFIGURATION
dotnet build ./ReadLine.Reboot.Tests -c $CONFIGURATION
