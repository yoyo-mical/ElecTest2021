from pathlib import Path
import os,shutil,subprocess,tempfile
root=Path(__file__).resolve().parents[1]
subprocess.run(['python3',str(root/'tools/generate-web-integrity.py'),'--check'],check=True)
with tempfile.TemporaryDirectory(prefix='elec-integrity-') as tmp:
 d=Path(tmp);web=d/'Web';shutil.copytree(root/'Testplan/Web',web)
 (d/'Check.cs').write_text('''using System; namespace Testplan {class Check {static int Main(string[] a){try{WebIntegrity.VerifyDirectory(a[0]);return 0;}catch{return 1;}}}}''')
 subprocess.run(['mono',os.environ.get('MCS_EXE','/workspace/.elec-env/root/usr/lib/mono/4.5/mcs.exe'),'-out:'+str(d/'check.exe'),str(d/'Check.cs'),str(root/'Testplan/Authorization/WebIntegrity.cs'),str(root/'Testplan/Authorization/WebIntegrityData.cs')],check=True)
 def check(ok):assert (subprocess.run(['mono',str(d/'check.exe'),str(web)]).returncode==0)==ok
 check(True)
 p=web/'native.js';original=p.read_bytes();p.write_bytes(original+b'\n// modification');check(False);p.write_bytes(original)
 p.unlink();check(False);p.write_bytes(original)
 extra=web/'injected.js';extra.write_text('');check(False);extra.unlink();check(True)
 print('PASS original UI accepted; modified, missing and extra files rejected; restored UI accepted')
