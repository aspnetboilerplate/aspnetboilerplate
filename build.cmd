:; set -eo pipefail
:; ./build.sh "$@"
:; exit $?

@ECHO OFF
powershell -ExecutionPolicy ByPass -NoProfile %0\..\build.ps1 %*
