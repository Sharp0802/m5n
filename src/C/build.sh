#!/usr/bin/env bash

CDIR="$(pwd)"
cd "$(dirname "$0")"
gcc -fPIC -rdynamic -shared $(pkg-config --cflags python3) -lpython3.11 export.c -o libpyexport.so
cd "$CDIR"
