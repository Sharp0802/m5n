CDIR="$(pwd)"
cd "$(dirname "$0")"
gcc -fPIC -shared -I"C:\Program Files\Python311\include" -L"C:\Program Files\Python311" -lpython311 export.c -o pyexport.dll
cd "$CDIR"
