using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
namespace Testplan {
    internal static class BrowserRuntime {
        internal static IEnumerable<string> Candidates(string root){
            yield return null; // Try actual system runtime initialization first.
            var folder=Path.Combine(root,"WebView2Runtime");
            if(!Directory.Exists(folder))yield break;
            var candidates=new[]{folder}.Concat(Directory.GetDirectories(folder));
            foreach(var path in candidates)if(IsX86Runtime(path))yield return path;
        }
        internal static bool IsX86Runtime(string path){
            if(!new[]{"msedgewebview2.exe","msedge.dll","icudtl.dat","resources.pak"}.All(f=>File.Exists(Path.Combine(path,f))))return false;
            try{using(var r=new BinaryReader(File.OpenRead(Path.Combine(path,"msedgewebview2.exe")))){if(r.ReadUInt16()!=0x5a4d)return false;r.BaseStream.Position=0x3c;int offset=r.ReadInt32();if(offset<64||offset>r.BaseStream.Length-6)return false;r.BaseStream.Position=offset;return r.ReadUInt32()==0x4550&&r.ReadUInt16()==0x14c;}}catch(IOException){return false;}
        }
        internal static async Task<CoreWebView2Environment> Create(string runtime,string userData){return await CoreWebView2Environment.CreateAsync(runtime,userData,new CoreWebView2EnvironmentOptions());}
    }
}
