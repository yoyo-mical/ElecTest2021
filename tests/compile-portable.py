"""Compile managed integration with a NON-SHIPPING SDK type substitute.
This does not validate CASDK2 signatures or hardware. Production project is unchanged.
Run after sourcing the cloud Mono activation script, or set MONO/MCS_EXE.
"""
from pathlib import Path
import os,re,subprocess,tempfile
root=Path(__file__).resolve().parents[1];src=root/'Testplan'
out=Path(os.environ.get('ELEC_CHECK_DIR',tempfile.mkdtemp(prefix='elec-check-')));out.mkdir(exist_ok=True)
mono=os.environ.get('MONO','mono');mcs=os.environ.get('MCS_EXE','/workspace/.elec-env/root/usr/lib/mono/4.5/mcs.exe')
# Disposable copy replaces unavailable SDK object dispatch with dynamic. Never shipped.
ca=(src/'CA410.cs').read_text(encoding='utf-8-sig')
ca=re.sub(r'static CASDK2\w+ (obj\w+);',r'static dynamic \1;',ca)
(out/'CA410.compile-only.cs').write_text(ca)
(out/'Sdk.compile-only.cs').write_text('''namespace CASDK2 {
 public class CASDK2Ca200 { public CASDK2Ca200(){throw new System.NotSupportedException("SDK test substitute; hardware unavailable");} }
 public struct CASDK2MeasData { public Measure data; } public struct Measure { public double lv,sx,sy; }
 public struct CASDK2IntervalMetadata { public int totalNumMeas; }
 public struct CASDK2IntervalData { public CASDK2MeasData measdata; }
 public static class GlobalFunctions { public static int CASDK2_GetLocalizedErrorMsgFromErrorCode(int a,int b,ref string c){throw new System.NotSupportedException();}public static int CASDK2_GetLocalizedErrorMsgFromMeasurementErrorCode(int a,int b,ref string c){throw new System.NotSupportedException();} }
}''')
files=[p for p in src.glob('*.cs') if p.name!='CA410.cs']+list((src/'Modern').glob('*.cs'))+list((src/'WebHost').glob('*.cs'))+list((src/'Authorization').rglob('*.cs'))+list((src/'Properties').glob('*.cs'))+[out/'CA410.compile-only.cs',out/'Sdk.compile-only.cs']
# Regenerate source resources; no dependency on the historical screenshot harness.
resgen=os.environ.get('RESGEN_EXE',str(Path(mcs).with_name('resgen.exe')))
resources=[]
for resx in list(src.glob('*.resx'))+[src/'Properties/Resources.resx']:
 name='Testplan.'+('Properties.' if resx.parent.name=='Properties' else '')+resx.stem+'.resources'
 target=out/name
 subprocess.run([mono,resgen,'/useSourcePath',str(resx),str(target)],check=True,stdout=subprocess.DEVNULL)
 resources.append(target)
# Test-only entry/source substitution exists only in this build script, never in the application.
if os.environ.get('ELEC_SHARED_TEST_DIR'):
 replacement=Path(os.environ['ELEC_SHARED_TEST_DIR'])
 assert str(replacement).startswith('/tmp/')
 files=[p for p in files if p.parent!=src/'Authorization/Shared']+list(replacement.glob('*.cs'))
if os.environ.get('ELEC_TEST_ENTRY'):files.append(Path(os.environ['ELEC_TEST_ENTRY']))
args=[mono,mcs,'-sdk:4.8','-target:exe','-out:'+str(out/'ManagedCompileOnly.exe')]
if os.environ.get('ELEC_TEST_ENTRY'):args.append('-main:WebBackendChecks')
for name in ['System.Windows.Forms','System.Drawing','System.Data','System.Configuration','System.Security','System.Runtime.Serialization','System.Core','Microsoft.CSharp','System.Windows.Forms.DataVisualization']:args.append('-r:'+name)
for package in ['DockPanelSuite.3.0.6','DockPanelSuite.ThemeVS2015.3.0.6']:
 for dll in (root/'packages'/package/'lib/net40').glob('*.dll'):args.append('-r:'+str(dll))
args+=['-r:'+str(root/'packages/Microsoft.Web.WebView2.1.0.4258.31/lib/net462/Microsoft.Web.WebView2.Core.dll'),'-r:'+str(root/'packages/Microsoft.Web.WebView2.1.0.4258.31/lib/net462/Microsoft.Web.WebView2.WinForms.dll')]
args+=['-resource:'+str(p)+','+p.name for p in resources]
subprocess.run(args+[str(p) for p in files],check=True)
print('PASS managed integration compilation (temporary dynamic SDK substitute; not a production build):',out)

# The backend validates shipped UI even in test assemblies; no integrity bypass.
import shutil
shutil.copytree(src / "Web", out / "Web", dirs_exist_ok=True)
