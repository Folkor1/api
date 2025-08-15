#!/usr/bin/env bash
set -e
PORT="${1:-5253}"
pids=$(lsof -t -i :$PORT || true)
[ -z "$pids" ] && { echo "no process on :$PORT"; exit 0; }
echo "killing: $pids"; kill -9 $pids
