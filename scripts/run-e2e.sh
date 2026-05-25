#!/usr/bin/env bash
set -u

export NPM_CONFIG_PREFIX="${NPM_CONFIG_PREFIX:-$HOME/.npm-global}"
export PATH="$NPM_CONFIG_PREFIX/bin:$PATH"

if ! command -v bru >/dev/null 2>&1; then
  mkdir -p "$NPM_CONFIG_PREFIX"
  npm install -g @usebruno/cli
  INSTALL_EXIT=$?

  if [ "$INSTALL_EXIT" -ne 0 ]; then
    exit "$INSTALL_EXIT"
  fi
fi

docker compose up -d --build
COMPOSE_EXIT=$?

if [ "$COMPOSE_EXIT" -ne 0 ]; then
  docker compose down
  exit "$COMPOSE_EXIT"
fi

sleep 15

(
  cd bruno-collection || exit 1
  bru run --output ../bruno-report.json
)
BRU_EXIT=$?

docker compose down

exit "$BRU_EXIT"
