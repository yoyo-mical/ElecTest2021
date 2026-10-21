from pathlib import Path
import base64,hashlib,os,subprocess,tempfile
from cryptography.hazmat.primitives.asymmetric import rsa
root=Path(__file__).resolve().parents[1];shared=root/'Testplan/Authorization/Shared'
mono=os.environ.get('MONO','mono');mcs=os.environ.get('MCS_EXE','/workspace/.elec-env/root/usr/lib/mono/4.5/mcs.exe')
with tempfile.TemporaryDirectory(prefix='elec-auth-') as work:
 d=Path(work);before={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in shared.glob('*.cs')}
 key=rsa.generate_private_key(public_exponent=65537,key_size=2048).private_numbers()
 def n(value):return base64.b64encode(value.to_bytes((value.bit_length()+7)//8,'big')).decode()
 fields={'Modulus':key.public_numbers.n,'Exponent':key.public_numbers.e,'P':key.p,'Q':key.q,'DP':key.dmp1,'DQ':key.dmq1,'InverseQ':key.iqmp,'D':key.d}
 (d/'test-private.xml').write_text('<RSAKeyValue>'+''.join('<'+k+'>'+n(v)+'</'+k+'>' for k,v in fields.items())+'</RSAKeyValue>')
 pub='<RSAKeyValue>'+''.join('<'+k+'>'+n(fields[k])+'</'+k+'>' for k in ['Modulus','Exponent'])+'</RSAKeyValue>'
 (d/'IssuerPublicKey.cs').write_text('namespace SharedAuthorization {public static class IssuerPublicKey {public const string Xml="'+pub+'";}}')
 licensing=(shared/'Licensing.cs').read_text().replace('using (var identity = WindowsIdentity.GetCurrent()) return identity.Name;','return "TEST-PC\\\\demo";')
 (d/'Licensing.cs').write_text(licensing)
 history=(shared/'AuthorizationHistory.cs').read_text().replace('return ProtectedData.Protect(value, null, DataProtectionScope.CurrentUser);','return value;').replace('return ProtectedData.Unprotect(value, null, DataProtectionScope.CurrentUser);','return value;').replace('Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)','"'+str(d).replace('\\','\\\\')+'"')
 (d/'AuthorizationHistory.cs').write_text(history)
 subprocess.run([mono,mcs,'-sdk:4.5.2','-out:'+str(d/'Checks.exe'),'-r:System.Core','-r:System.Security','-r:System.Runtime.Serialization',str(root/'tests/AuthorizationChecks.cs')]+[str(p) for p in d.glob('*.cs')],check=True)
 subprocess.run([mono,str(d/'Checks.exe')],cwd=d,check=True)
 assert before=={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in shared.glob('*.cs')}
 print('PASS production shared-module sources unchanged; no test private key retained')
