#!/bin/bash
set -euo pipefail

MSG_FILE="${1:-}"
MSG="$(cat "$MSG_FILE")"

# Regex chuẩn Conventional Commits
REGEX='^(feat|fix|chore|docs|refactor|test|build|ci)(\([a-zA-Z0-9_-]+\))?: .+'

# Bỏ qua nếu là Merge hoặc Revert
if echo "$MSG" | grep -E '^(Merge |Revert )' >/dev/null 2>&1; then
  exit 0
fi

# Check Regex
if ! echo "$MSG" | grep -E "$REGEX" >/dev/null 2>&1; then
  echo "[Husky] Lỗi: Commit message không hợp lệ"
  echo "Cú pháp chuẩn: type(scope): subject"
  echo "Ví dụ: feat(open-api): add swagger docs"
  exit 1
fi

echo "[Husky] Commit message hợp lệ!"
exit 0
