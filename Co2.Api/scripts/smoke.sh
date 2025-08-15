#!/usr/bin/env bash
set -euo pipefail

PORT="${PORT:-5253}"
BASE_URL="http://localhost:${PORT}"
HEALTH_URL="${BASE_URL}/healthz"

# Источник факторов по умолчанию — JSON (можно переопределить извне)
export FACTOR_SOURCE="${FACTOR_SOURCE:-json}"
export FACTOR_JSON_PATH="${FACTOR_JSON_PATH:-$HOME/work/co2/data-pipeline/datasets/factors.json}"
export ASPNETCORE_URLS="${ASPNETCORE_URLS:-$BASE_URL}"

started=0
pid=""

# Проверим, слушает ли кто-то порт уже
if ss -tlnp 2>/dev/null | grep -q ":${PORT} "; then
  echo "[info] порт ${PORT} уже занят — предполагаем, что API запущен отдельно."
else
  echo "[info] стартуем API на ${ASPNETCORE_URLS} ..."
  dotnet run > /tmp/co2api.log 2>&1 &
  pid=$!
  started=1
fi

# Ждём healthz
for i in {1..40}; do
  if curl -sf "${HEALTH_URL}" >/dev/null; then
    break
  fi
  sleep 0.25
done

echo "[ok] healthz: $(curl -s "${HEALTH_URL}")"

# Тестовый расчёт
resp=$(curl -s -X POST "${BASE_URL}/v1/calculations/page" \
  -H "Content-Type: application/json" \
  -d '{"region":"IE","bytesTransferred":250000000,"cacheHitRate":0.3}')

echo "[ok] calc: $resp"

# Останавливаем только то, что запускали сами
if [ "$started" -eq 1 ] && [ -n "${pid}" ]; then
  kill "$pid" >/dev/null 2>&1 || true
  wait "$pid" 2>/dev/null || true
  echo "[info] API, запущенный скриптом, остановлен."
else
  echo "[info] Внешний API оставляем работать."
fi
