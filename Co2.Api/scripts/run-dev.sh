#!/usr/bin/env bash
set -euo pipefail
# подхватываем .env
if [ -f .env ]; then set -a; . ./.env; set +a; fi
echo "[run] ASPNETCORE_URLS=${ASPNETCORE_URLS:-unset} FACTOR_SOURCE=${FACTOR_SOURCE:-unset}"
dotnet run
