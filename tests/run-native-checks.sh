#!/usr/bin/env bash
set -euo pipefail
repo_dir="$(cd "$(dirname "$0")/.." && pwd)"
# Cloud-only Mono setup. On another host provide MONO/MCS_EXE/RESGEN_EXE as needed.
if [[ -f /workspace/.elec-env/activate.sh ]]; then source /workspace/.elec-env/activate.sh; fi
python3 "$repo_dir/tests/check-invariants.py"
python3 "$repo_dir/tests/check-authorization.py"
check_dir="$(mktemp -d /tmp/elec-managed-XXXXXX)"
export ELEC_CHECK_DIR="$check_dir"
python3 "$repo_dir/tests/compile-portable.py"
cp "$repo_dir/packages/DockPanelSuite.3.0.6/lib/net40/WeifenLuo.WinFormsUI.Docking.dll" "$check_dir/"
cp "$repo_dir/packages/DockPanelSuite.ThemeVS2015.3.0.6/lib/net40/WeifenLuo.WinFormsUI.Docking.ThemeVS2015.dll" "$check_dir/"
mono "${MCS_EXE:-/workspace/.elec-env/root/usr/lib/mono/4.5/mcs.exe}" -out:"$check_dir/LayoutChecks.exe" -r:System.Windows.Forms -r:System.Drawing -r:System.Core -r:"$check_dir/ManagedCompileOnly.exe" -r:"$check_dir/WeifenLuo.WinFormsUI.Docking.dll" -r:"$check_dir/WeifenLuo.WinFormsUI.Docking.ThemeVS2015.dll" "$repo_dir/tests/NativeLayoutChecks.cs"
mkdir -p "$repo_dir/native-preview"
xvfb-run -a -s '-screen 0 1600x980x24' mono "$check_dir/LayoutChecks.exe" "$repo_dir/native-preview/layout-mono.png"
xvfb-run -a -s '-screen 0 1600x980x24' mono "$check_dir/LayoutChecks.exe" "$repo_dir/native-preview/startup-locked-mono.png" startup
