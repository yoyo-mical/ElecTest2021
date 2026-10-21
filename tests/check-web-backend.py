from pathlib import Path
import os,subprocess,tempfile,json,base64,datetime,hashlib
from cryptography.hazmat.primitives.asymmetric import rsa,padding
from cryptography.hazmat.primitives import hashes
root=Path(__file__).resolve().parents[1];shared=root/'Testplan/Authorization/Shared'
with tempfile.TemporaryDirectory(prefix='elec-web-backend-') as work:
 d=Path(work);sources=d/'shared';sources.mkdir();before={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in shared.glob('*.cs')}
 key=rsa.generate_private_key(public_exponent=65537,key_size=2048);numbers=key.public_key().public_numbers()
 def n(value):return base64.b64encode(value.to_bytes((value.bit_length()+7)//8,'big')).decode()
 pub='<RSAKeyValue><Modulus>'+n(numbers.n)+'</Modulus><Exponent>'+n(numbers.e)+'</Exponent></RSAKeyValue>'
 (sources/'IssuerPublicKey.cs').write_text('namespace SharedAuthorization {public static class IssuerPublicKey {public const string Xml="'+pub+'";}}')
 (sources/'Licensing.cs').write_text((shared/'Licensing.cs').read_text().replace('using (var identity = WindowsIdentity.GetCurrent()) return identity.Name;','return "TEST-PC\\\\demo";'))
 (sources/'AuthorizationHistory.cs').write_text((shared/'AuthorizationHistory.cs').read_text().replace('return ProtectedData.Protect(value, null, DataProtectionScope.CurrentUser);','return value;').replace('return ProtectedData.Unprotect(value, null, DataProtectionScope.CurrentUser);','return value;').replace('Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)','"'+str(d)+'"'))
 payload=json.dumps({'product':'ElecTest2026','issuedUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'users':[{'account':'TEST-PC\\demo','edition':'Standard','enabled':True,'expiresUtc':''}]}).encode()
 license=d/'test-license.json';license.write_text(json.dumps({'payload':base64.b64encode(payload).decode(),'signature':base64.b64encode(key.sign(payload,padding.PKCS1v15(),hashes.SHA256())).decode()}))
 env=dict(os.environ,ELEC_CHECK_DIR=str(d/'build'),ELEC_SHARED_TEST_DIR=str(sources),ELEC_TEST_ENTRY=str(root/'tests/WebBackendChecks.cs'))
 subprocess.run(['python3',str(root/'tests/compile-portable.py')],env=env,check=True,stdout=subprocess.DEVNULL)
 subprocess.run(['cp',str(root/'packages/DockPanelSuite.3.0.6/lib/net40/WeifenLuo.WinFormsUI.Docking.dll'),str(d/'build')],check=True)
 subprocess.run(['cp',str(root/'packages/DockPanelSuite.ThemeVS2015.3.0.6/lib/net40/WeifenLuo.WinFormsUI.Docking.ThemeVS2015.dll'),str(d/'build')],check=True)
 # Optional test of the actual obfuscator against the same backend checks.
 obfuscator=os.environ.get('ELEC_OBFUSCAR')
 if obfuscator:
  import shutil,xml.etree.ElementTree as ET
  for dll in (root/'packages/Microsoft.Web.WebView2.1.0.4258.31/lib/net462').glob('*.dll'):shutil.copy2(dll,d/'build')
  tree=ET.parse(root/'security/obfuscar.xml')
  for var in tree.getroot().findall('Var'):
   if var.attrib['name']=='InPath':var.set('value',str(d/'build'))
   if var.attrib['name']=='OutPath':var.set('value',str(d/'obfuscated'))
  tree.getroot().find('Module').set('file',str(d/'build/ManagedCompileOnly.exe'))
  config=d/'obfuscar.xml';tree.write(config)
  subprocess.run(['mono',obfuscator,str(config)],check=True)
  shutil.copy2(d/'obfuscated/ManagedCompileOnly.exe',d/'build/ManagedCompileOnly.exe')
 subprocess.run(['xvfb-run','-a','-s','-screen 0 1600x980x24','mono',str(d/'build/ManagedCompileOnly.exe'),str(license)],check=True)
 assert before=={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in shared.glob('*.cs')}
 print('PASS production shared sources unchanged; temporary key/fixture removed')
