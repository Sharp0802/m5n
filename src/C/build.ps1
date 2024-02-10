CDIR="$(pwd)"
cd "$(dirname "$0")"
gcc -fPIC -rdynamic -shared -lpython311 export.c -o libpyexport.dll
cd "$CDIR"
