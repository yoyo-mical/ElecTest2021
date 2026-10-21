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
project=(root/'Testplan/ElecTest2026.csproj').read_text(encoding='utf-8-sig');assert 'v4.5.2' in project
main=(root/'Testplan/Modern/MainWorkspace.cs').read_text();assert 'await AppLicense.RefreshAsync' in main and main.index('await AppLicense.RefreshAsync')<main.index('if(!workspaceBuilt)BuildAuthorizedWorkspace()')
print('PASS net452 retained and authorization awaited before hardware workspace creation')
