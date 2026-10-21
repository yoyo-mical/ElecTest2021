using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SharedAuthorization;
class AuthorizationChecks {
    static int count;
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);count++;Console.WriteLine("PASS "+label);}
    static void Denied(Action action,string label){try{action();}catch{Check(true,label);return;}throw new Exception("UNEXPECTED ALLOW: "+label);}
    static string Sign(string product,string account,string edition,bool enabled,string expiry,string issued){
        var list=new LicenseList{product=product,issuedUtc=issued,users=new System.Collections.Generic.List<Grant>{new Grant{account=account,edition=edition,enabled=enabled,expiresUtc=expiry}}};
        var bytes=Encoding.UTF8.GetBytes(JsonCodec.Serialize(list));using(var rsa=new RSACryptoServiceProvider()){rsa.PersistKeyInCsp=false;rsa.FromXmlString(File.ReadAllText("test-private.xml"));return JsonCodec.Serialize(new SignedLicense{payload=Convert.ToBase64String(bytes),signature=Convert.ToBase64String(rsa.SignData(bytes,CryptoConfig.MapNameToOID("SHA256")))});}
    }
    static void Main(){
        var issued=DateTimeOffset.UtcNow.ToString("o");var good=Sign(Products.Electrical,"TEST-PC\\demo","Standard",true,"",issued);
        var session=AuthorizationSession.Issue(good,Products.Electrical);session.Ensure();Check(session.Edition=="Standard","signed electrical grant accepted");
        Denied(()=>AuthorizationSession.Issue(good,Products.Timing),"cross-product grant rejected");
        Denied(()=>AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\other","Standard",true,"",issued),Products.Electrical),"wrong Windows account rejected");
        Denied(()=>AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\demo","Standard",false,"",issued),Products.Electrical),"disabled grant rejected");
        Denied(()=>AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\demo","Standard",true,DateTimeOffset.UtcNow.AddMinutes(-1).ToString("o"),issued),Products.Electrical),"expired grant rejected");
        Denied(()=>AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\demo","Standard",true,"",""),Products.Electrical),"electrical issuedUtc required");
        Denied(()=>Licensing.Verify(good.Replace("signature\":\"","signature\":\"AA"),Products.Electrical),"tampered signature rejected");
        var earlier=DateTimeOffset.UtcNow.AddDays(-1).ToString("o");Denied(()=>AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\demo","Standard",true,"",earlier),Products.Electrical),"older signed list rejected after observed newer version");
        var timing=AuthorizationSession.Issue(Sign(Products.Timing,"TEST-PC\\demo","Pro",true,"",earlier),Products.Timing);Check(timing.Edition=="Standard","product rollback histories isolated and legacy edition mapped");
        var beta=AuthorizationSession.Issue(Sign(Products.Electrical,"TEST-PC\\demo","Basic",true,"",issued),Products.Electrical);Check(beta.Edition=="Beta","legacy Basic mapped");
        session.Revoke();Denied(()=>session.Ensure(),"revoked session rejected");
        File.WriteAllText("good.json",good);var gate=new AuthorizationGate(Products.Electrical);Denied(()=>gate.Ensure(),"gate denied before startup refresh");gate.RefreshAsync("good.json").GetAwaiter().GetResult();gate.Ensure();Check(gate.Edition=="Standard","gate allows only after signed refresh");
        Denied(()=>gate.RefreshAsync("missing.json").GetAwaiter().GetResult(),"missing list refresh rejected");Denied(()=>gate.Ensure(),"refresh failure revokes previous session");
        gate.RefreshAsync("good.json").GetAwaiter().GetResult();gate.Dispose();Denied(()=>gate.Ensure(),"disposed gate rejects operations");Denied(()=>gate.RefreshAsync("good.json").GetAwaiter().GetResult(),"disposed gate rejects late refresh");
        Console.WriteLine("PASS "+count+" authorization checks; ephemeral key/identity/DPAPI substitutions only in temporary test copies");
    }
}
