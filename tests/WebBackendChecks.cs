// Compiled only into a disposable test executable with an ephemeral issuer/OS identity.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Testplan;
class WebBackendChecks {
    static int count;
    static void Check(bool value,string label){if(!value)throw new Exception(label);count++;Console.WriteLine("PASS "+label);}
    static void Denied(Action action,string label){try{action();}catch{Check(true,label);return;}throw new Exception("Unexpected allow: "+label);}
    static WebRequest Request(string operation,Dictionary<string,string> fields=null){return new WebRequest{id="test",operation=operation,args=new WebArguments{fields=fields}};}
    [STAThread]static void Main(string[] args){
        Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);WeifenLuo.WinFormsUI.Docking.PatchController.EnableAll=true;
        Denied(()=>new WorkspaceBackend(),"backend cannot initialize before authorization");
        Denied(()=>AppLicense.RefreshAsync(args[0]+".missing").GetAwaiter().GetResult(),"missing startup license rejected");
        AppLicense.RefreshAsync(args[0]).GetAwaiter().GetResult();
        System.IO.File.Delete(args[0]);
        AppLicense.Ensure(); AppLicense.RefreshAsync(args[0]).GetAwaiter().GetResult();
        Check(AppLicense.Edition=="Standard","startup admission survives unavailable list without revalidation");
        var webFile=System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Web","native.js");
        var originalWeb=System.IO.File.ReadAllBytes(webFile);
        try { System.IO.File.AppendAllText(webFile,"\n// runtime test"); WebIntegrity.Ensure(); AppLicense.Ensure(); Check(true,"no repeated integrity check during admitted process"); }
        finally { System.IO.File.WriteAllBytes(webFile,originalWeb); }
        using(var backend=new WorkspaceBackend()){
            var state=backend.Snapshot();Check(state.authorized&&state.points.Count==0&&state.script==""&&state.recipe=="","backend starts without seeded scripts, curves or recipes");
            Check(state.readings.Values.All(v=>string.IsNullOrEmpty(v)),"unmeasured instrument fields are empty");
            Check(state.choices["recipe.item"].Length==16,"all original recipe item choices exposed");
            var edit=Request("set",new Dictionary<string,string>{{"arm.script","//VGH\n 01 VGH\n//ENDVGH"},{"recipe.delay","2"}});edit.args.recipeRuns=new List<WebTextRun>{new WebTextRun{text="  Delay 1  \n",bold=true,color="#123456"}};
            var reply=backend.Execute(edit);Check(reply.state.script.Contains("01 VGH")&&reply.state.recipe.Contains("Delay 1"),"browser editor data reaches original C# editors");
            var format=Request("recipe.format");format.args.selectionStart=0;reply=backend.Execute(format);Check(reply.state.recipe.Trim()=="Delay 1","format delegates to original recipe handler");Check(reply.recipeRuns!=null,"native rich text can return as safe runs");
            var add=Request("recipe.add",new Dictionary<string,string>{{"recipe.item","Delay"},{"recipe.delay","2"}});add.args.selectionStart=backend.Snapshot().recipe.Length;reply=backend.Execute(add);Check(reply.state.recipe.Contains("Delay 2"),"recipe generation remains in original C# builder");
            Denied(()=>backend.Execute(Request("process.start")),"arbitrary native operations rejected");
            Denied(()=>backend.Execute(Request("set",new Dictionary<string,string>{{"arm.vgh","not-hex"}})),"invalid original HEX parameter rejected");
            DataChart.CurrentTest="VCOM";DataChart.fs.intEdit(0);DataChart.fs.intstrstrEdit(0,"10","12.5");state=backend.Snapshot();Check(state.points.Count==1&&state.points[0].x=="10"&&state.points[0].y==12.5,"original chart event bus forwards exact sample values");
            var encoded=WebJson.Encode(new WebReply{id="safe",ok=true,state=state});Check(WebJson.Decode<WebReply>(encoded).state.points[0].y==12.5,"JSON snapshot round-trip preserves sample");
            var hostile=Request("set",new Dictionary<string,string>{{"arm.script","</script><script>alert(1)</script>"}});reply=backend.Execute(hostile);Check(reply.state.script==hostile.args.fields["arm.script"],"script-like editor text stays data in C#");
            AppLicense.Lock("test shutdown");Denied(()=>backend.Execute(Request("recipe.run")),"explicit shutdown lock blocks execution");Denied(()=>backend.Snapshot(),"explicit shutdown lock blocks result publication");backend.Stop();Check(true,"stop remains callable after explicit lock");
        }
        intdelegate reset = i => {}; intstrstrdelegate sample = (i,x,y) => {};
        DataChart.fs.intevent += reset; DataChart.fs.intstrstrevent += sample;
        try { DataChart.fs.intEdit(0); DataChart.fs.intstrstrEdit(0,"10","12.5"); Check(true,"disposed backend no longer checks revoked authorization on chart events"); }
        finally { DataChart.fs.intevent -= reset; DataChart.fs.intstrstrevent -= sample; }
        AppLicense.Dispose();Denied(()=>AppLicense.Ensure(),"disposed application rejects access");Console.WriteLine("PASS "+count+" native bridge/backend checks; no device commands or SDK measurements executed");
    }
}
