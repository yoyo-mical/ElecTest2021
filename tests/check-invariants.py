"""Review guards and preserve ordered device calls, protocol literals and timing expressions."""
from pathlib import Path
import re,subprocess,hashlib
root=Path(__file__).resolve().parents[1]
def old(file):return subprocess.check_output(['git','show','84210471974c6c16cb517ddec8269540b75114e2:Testplan/'+file],cwd=root).decode('utf-8-sig')
def uncomment(s):return re.sub(r'//[^\n]*|/\*.*?\*/','',s,flags=re.S)
for file in ['UART.cs','ARMCode.cs','Control.cs','Pattern.cs','CA410.cs']:
 before=old(file);after=(root/'Testplan'/file).read_text(encoding='utf-8-sig')
 for label,pattern in [('delay expressions',r'(?:TimeDelay|Thread.Sleep)\([^;]*?\)'),('protocol literals',r'"AA\s*[0-9A-F][^"\n]*"'),('SDK operations',r'obj\w+\.\w+\([^;\n]*')]:
  assert re.findall(pattern,uncomment(before))==re.findall(pattern,uncomment(after)),(file,label)
 print('PASS original timing expressions / protocol literals / SDK call sequence:',file)
for file,method in [('UART.cs','HexSend'),('Control.cs','controlCodeRun'),('ARMCode.cs','saveCsv'),('Pattern.cs','CodeSend'),('CA410.cs','Measurement')]:
 s=(root/'Testplan'/file).read_text(encoding='utf-8-sig');assert re.search(r'void '+method+r'\([^\n]*\)\s*\{\s*if \(!AppLicense.AllowOperation\(\)\) return;',s),(file,method)
print('PASS authorization guards at send / recipe / save / pattern / measurement entry points')
project=(root/'Testplan/ElecTest2026.csproj').read_text(encoding='utf-8-sig');assert 'v4.8' in project
main=(root/'Testplan/WebHost/WebWorkspaceForm.cs').read_text();assert 'await AppLicense.RefreshAsync' in main and main.index('await AppLicense.RefreshAsync')<main.index('backend=new WorkspaceBackend()');assert 'new WebWorkspaceForm()' in (root/'Testplan/Program.cs').read_text(encoding='utf-8-sig');assert 'e.Source!=AppUrl' in main and 'AreHostObjectsAllowed=false' in main
print('PASS net48 WebView2 entry, trusted message source, no host objects, and authorization before backend creation')
