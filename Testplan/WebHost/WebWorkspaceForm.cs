using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
namespace Testplan {
    public sealed class WebWorkspaceForm : Form {
        internal const string AppUrl="https://electest.local/index.html";
        private WebView2 browser;
        private WorkspaceBackend backend;
        private readonly Timer stateTimer=new Timer{Interval=500};
        private readonly Timer authorizationTimer=new Timer{Interval=60000};
        private DateTime refreshAt;
        private string path="",lastError="";
        private bool closed,refreshing,ready,authorized;
        private readonly Label failure=new Label{Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,Visible=false,Padding=new Padding(30)};
        public WebWorkspaceForm(){
            Text="ElecTest2026 · Design by Wuhao Studio";Font=new Font("Segoe UI",9f);AutoScaleMode=AutoScaleMode.Dpi;AutoScaleDimensions=new SizeF(96,96);MinimumSize=new Size(900,650);Size=new Size(1600,980);StartPosition=FormStartPosition.CenterScreen;
            var work=Screen.FromControl(this).WorkingArea;Size=new Size(Math.Min(1600,work.Width),Math.Min(980,work.Height));
            Controls.Add(failure);AppLicense.Locked+=OnLocked;
            Shown+=async(s,e)=>await StartBrowser();
            stateTimer.Tick+=(s,e)=>PushState();
            authorizationTimer.Tick+=async(s,e)=>{if(closed||refreshing||backend==null)return;try{AppLicense.Ensure();if(DateTime.UtcNow>=refreshAt&&!backend.Busy)await RefreshAuthorization(path,false);}catch(Exception ex){OnLocked(ex.Message);}};
            FormClosing+=(s,e)=>{if(backend!=null&&backend.Busy){backend.Stop();e.Cancel=true;lastError="正在停止当前操作，请待本次设备调用返回后再关闭。";PushState();}};
            FormClosed+=(s,e)=>{closed=true;stateTimer.Dispose();authorizationTimer.Dispose();AppLicense.Locked-=OnLocked;if(backend!=null)backend.Dispose();AppLicense.Dispose();if(browser!=null)browser.Dispose();};
        }
        private async Task StartBrowser(){
            try{
                WebIntegrity.Ensure();
                path=AppLicense.ReadSettings().licensePath??"";
                var assets=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Web");
                if(!File.Exists(Path.Combine(assets,"index.html")))throw new IOException("缺少 Web 界面文件，请完整解压程序包。");
                string userData=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ElecTest2026","WebView2");
                Exception last=null;
                foreach(var runtime in BrowserRuntime.Candidates(AppDomain.CurrentDomain.BaseDirectory)){
                    var view=new WebView2{Dock=DockStyle.Fill};Controls.Add(view);
                    try{var environment=await BrowserRuntime.Create(runtime,userData);await view.EnsureCoreWebView2Async(environment);browser=view;last=null;break;}
                    catch(Exception ex){last=ex;Controls.Remove(view);view.Dispose();}
                }
                if(last!=null||browser==null)throw new InvalidOperationException("无法启动 WebView2。请安装 Microsoft Edge WebView2 Runtime，或随包提供完整 x86 WebView2Runtime 目录。",last);
                var core=browser.CoreWebView2;
                core.Settings.AreDevToolsEnabled=false;core.Settings.AreDefaultContextMenusEnabled=false;core.Settings.IsStatusBarEnabled=false;core.Settings.AreHostObjectsAllowed=false;core.Settings.IsZoomControlEnabled=false;
                core.SetVirtualHostNameToFolderMapping("electest.local",assets,CoreWebView2HostResourceAccessKind.DenyCors);
                core.NavigationStarting+=(s,e)=>{if(e.Uri!=AppUrl)e.Cancel=true;};
                core.NewWindowRequested+=(s,e)=>e.Handled=true;
                core.PermissionRequested+=(s,e)=>e.State=CoreWebView2PermissionState.Deny;
                core.DownloadStarting+=(s,e)=>e.Cancel=true; // Only the original C# export/save paths may write user files.
                core.AddWebResourceRequestedFilter("*",CoreWebView2WebResourceContext.All);
                core.WebResourceRequested+=(s,e)=>{Uri uri;if(!Uri.TryCreate(e.Request.Uri,UriKind.Absolute,out uri)||uri.Scheme!="https"||uri.Host!="electest.local"){e.Response=core.Environment.CreateWebResourceResponse(new MemoryStream(),403,"Forbidden","");}};
                core.WebMessageReceived+=OnMessage;
                core.NavigationCompleted+=(s,e)=>{ready=e.IsSuccess;if(!ready)ShowFailure("本地界面加载失败："+e.WebErrorStatus);};
                core.Navigate(AppUrl);browser.BringToFront();stateTimer.Start();authorizationTimer.Start();
            }catch(Exception ex){ShowFailure(ex.Message+(ex.InnerException!=null?"\n"+ex.InnerException.Message:""));}
        }
        private void ShowFailure(string message){failure.Text=message;failure.Visible=true;failure.BringToFront();}
        private async Task RefreshAuthorization(string newPath,bool save){
            if(refreshing)throw new InvalidOperationException("授权检查正在进行。");
            if(backend!=null&&backend.Busy)throw new InvalidOperationException("请先停止配方再检查授权。");
            refreshing=true;
            try{await AppLicense.RefreshAsync(newPath);if(closed)return;if(save)AppLicense.SavePath(newPath);if(backend==null)backend=new WorkspaceBackend();authorized=true;path=newPath;refreshAt=DateTime.UtcNow.AddHours(1);lastError="";}
            catch(Exception ex){AppLicense.Lock(ex.Message);throw;}
            finally{refreshing=false;}
        }
        private WebState State(){
            if(authorized&&backend!=null){try{var state=backend.Snapshot();state.configuredPath=path;state.error=lastError;return state;}catch(Exception ex){lastError=ex.Message;}}
            return new WebState{authorized=false,configuredPath=path,error=lastError};
        }
        private void Post(WebReply reply){if(!closed&&ready&&browser!=null&&browser.CoreWebView2!=null)browser.CoreWebView2.PostWebMessageAsJson(WebJson.Encode(reply));}
        private void PushState(){Post(new WebReply{type="state",ok=true,state=State()});}
        private void OnLocked(string reason){authorized=false;lastError=reason;if(backend!=null)backend.Stop();if(InvokeRequired){BeginInvoke(new Action(PushState));}else PushState();}
        private async void OnMessage(object sender,CoreWebView2WebMessageReceivedEventArgs e){
            if(e.Source!=AppUrl||closed)return;
            ready=true;
            WebRequest request=null;
            try{
                var json=e.TryGetWebMessageAsString();if(json==null||json.Length>1048576)throw new InvalidOperationException("本机请求过大。");
                request=WebJson.Decode<WebRequest>(json);if(request==null||string.IsNullOrEmpty(request.id)||request.id.Length>80||string.IsNullOrEmpty(request.operation))throw new InvalidOperationException("本机请求无效。");
                var args=request.args??new WebArguments();
                if(request.operation=="hello"){Post(new WebReply{id=request.id,ok=true,state=State()});return;}
                if(request.operation=="authorize"){await RefreshAuthorization(args.path??path,args.savePath);Post(new WebReply{id=request.id,ok=true,state=State()});return;}
                if(request.operation=="stop"){if(backend!=null)backend.Stop();Post(new WebReply{id=request.id,ok=true,state=State()});return;}
                if(request.operation=="disconnect"){if(backend!=null){if(backend.Busy){backend.Stop();throw new InvalidOperationException("停止请求已提交；请待当前设备调用返回后再次断开。");}backend.Disconnect();}Post(new WebReply{id=request.id,ok=true,state=State()});return;}
                AppLicense.Ensure();if(backend==null)throw new UnauthorizedAccessException("请先完成授权检查。");
                Post(backend.Execute(request));
            }catch(Exception ex){Post(new WebReply{id=request!=null?request.id:"",ok=false,error=ex.Message,state=State()});}
        }
    }
}
