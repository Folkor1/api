#!/usr/bin/env bash
set -euo pipefail

export FACTOR_SOURCE=${FACTOR_SOURCE:-json}
export FACTOR_JSON_PATH=${FACTOR_JSON_PATH:-$HOME/work/co2/data-pipeline/datasets/factors.json}
export ASPNETCORE_URLS=${ASPNETCORE_URLS:-http://localhost:5253}

dotnet run > /tmp/co2api.log 2>&1 &
PID=$!

# ждём живости
for i in {1..40}; do
  if curl -sf "${ASPNETCORE_URLS}/healthz" >/dev/null; then break; fi
  sleep 0.25
done

echo "[ok] healthz: $(curl -s ${ASPNETCORE_URLS}/healthz)"

# тестовый расчёт (пример входа)
resp=$(curl -s -X POST "${ASPNETCORE_URLS}/v1/calculations/page" \
  -H "Content-Type: application/json" \
  -d '{"region":"IE","bytesTransferred":250000000,"cacheHitRate":0.3}')
echo "[ok] calc: $resp"

kill $PID >/dev/null 2>&1 || true
wait $PID 2>/dev/null || true
