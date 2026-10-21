#!/usr/bin/env bash
set -euo pipefail
repo_dir="$(cd "$(dirname "$0")/.." && pwd)"
if [[ -f /workspace/.elec-env/activate.sh ]]; then source /workspace/.elec-env/activate.sh; fi
python3 "$repo_dir/tests/check-web-integrity.py"
python3 "$repo_dir/tests/check-invariants.py"
python3 "$repo_dir/tests/check-authorization.py"
python3 "$repo_dir/tests/check-web-backend.py"
node --check "$repo_dir/Testplan/Web/native.js"
python3 "$repo_dir/tests/check-web-ui.py"
