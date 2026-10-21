from pathlib import Path
import json
from playwright.sync_api import sync_playwright
root=Path(__file__).resolve().parents[1];assets=root/'Testplan/Web'
fixture={
 'authorized':False,'busy':False,'configuredPath':'','account':'BRIDGE-TEST（非硬件运行）','edition':'Standard','error':'','serialConnected':False,'meterConnected':False,'serialState':'串口未连接','meterState':'仪器未连接','serialLog':'','script':'','recipe':'','currentLine':0,'fields':{},'enabled':{},'choices':{},'readings':{},'test':'','metric':'Lv','unit':'cd/m²','points':[],'sheetColumns':[],'sheet':[]
}
for key,values in {'serial.port':['COM1'],'serial.baud':['115200'],'serial.bits':['8'],'serial.stop':['1'],'serial.parity':['无'],'recipe.item':['Delay','Pattern','Gamma','VCOM','VGH','VGL','CKH','CKV','Crosstalk','Flicker','DownLoad','Goto','Position','Lv','Read','Save'],'recipe.preset':['NA','White','Black','Gray','Red','Green','Blue','Flicker','PixelCheck','DotCheck','Crosstalk-W','Crosstalk-B']}.items():
 fixture['choices'][key]=values;fixture['fields'][key]=values[0]
for prefix,names in [('arm',['vgh','vgl','vcom','ckv','ckh','eq']),('recipe',['start','end','step','delay','time','goto']),('pattern',['red','green','blue','back','gray'])]:
 for name in names:fixture['fields'][prefix+'.'+name]='0'
fixture['fields'].update({'serial.send':'','serial.hex':'true','meter.mode':'JEITA','recipe.pattern':'','recipe.download':'','recipe.path':'','recipe.downcom':'false','pattern.width':'','pattern.height':''})
bootstrap='''(()=>{const listeners=[];window.testRequests=[];window.testState=FIXTURE;window.chrome=window.chrome||{};window.chrome.webview={addEventListener:(name,fn)=>listeners.push(fn),postMessage:raw=>{const m=JSON.parse(raw);window.testRequests.push(m);if(m.operation==='authorize')window.testState.authorized=true;if(m.operation==='set')Object.assign(window.testState.fields,m.args.fields);const reply={type:'reply',id:m.id,ok:true,state:JSON.parse(JSON.stringify(window.testState))};queueMicrotask(()=>listeners.forEach(fn=>fn({data:reply})));}};window.testPush=next=>{Object.assign(window.testState,next);listeners.forEach(fn=>fn({data:{type:'state',ok:true,state:JSON.parse(JSON.stringify(window.testState))}}));};})();'''.replace('FIXTURE',json.dumps(fixture,ensure_ascii=False))
with sync_playwright() as p:
 browser=p.chromium.launch(executable_path='/usr/bin/chromium',headless=True,args=['--no-sandbox'])
 def route_page(page):
  def serve(route):
   path=route.request.url.split('https://electest.local/',1)[-1].split('?')[0]
   file=assets/path
   if file.is_file() and file.resolve().parent==assets.resolve():route.fulfill(path=str(file))
   else:route.abort()
  page.route('https://electest.local/**',serve)
 page=browser.new_page(viewport={'width':1600,'height':980});errors=[];page.on('pageerror',lambda e:errors.append(str(e)));route_page(page)
 page.goto('https://electest.local/index.html');page.wait_for_timeout(150)
 assert page.locator('#auth-screen').is_visible()
 assert not page.locator('#app').is_visible()
 assert page.locator('#plot .plot-point').count()==0
 assert '请从 ElecTest2026' in page.locator('#auth-error').inner_text()
 page.add_init_script(bootstrap);page.reload();page.wait_for_timeout(100)
 page.fill('#license-path',r'C:\test\ElecTest2026.license.json');page.click('#authorize');page.wait_for_timeout(100)
 assert page.locator('#app').is_visible();assert not page.locator('#auth-screen').is_visible()
 assert page.locator('.panel').count()==6;assert page.locator('.pattern-card').count()==32
 assert page.locator('#pattern-disclosure').evaluate('(e)=>!e.open')
 assert page.locator('#plot .plot-point').count()==0
 assert page.locator('#editor').input_value()==''
 assert page.locator('#recipe-rows tr').count()==0
 assert page.locator('[data-chart-view=compare]').is_disabled()
 page.click('[data-action=send]');page.wait_for_timeout(50)
 assert page.evaluate('testRequests.at(-1).operation')=='serial.send'
 page.click('[data-action=measure]');page.wait_for_timeout(50)
 assert page.evaluate('testRequests.at(-1).operation')=='meter.measure'
 assert page.locator('#lv').inner_text()=='— —'
 page.click('[data-source=pictureBox10]');page.wait_for_timeout(50)
 assert page.evaluate('testRequests.at(-1).args.pattern')=='pictureBox10'
 assert page.evaluate('testRequests.at(-1).operation')=='pattern.select'
 page.click('#run-current');page.wait_for_timeout(50)
 assert page.evaluate('testRequests.at(-1).operation')=='recipe.current'
 page.click('#text-mode');page.locator('#raw-recipe').fill('Delay 1\nPattern 00 00 FF FF FF');page.click('#recipe-format');page.wait_for_timeout(50)
 request=page.evaluate('testRequests.at(-1)');assert request['operation']=='recipe.format';assert '\n' in ''.join(run['text'] for run in request['args']['recipeRuns'])
 page.click('#table-mode')
 page.evaluate('testPush({points:[{x:"10",y:5},{x:"11",y:3},{x:"12",y:4}],test:"VCOM",metric:"JEITA",unit:"dB"})')
 assert page.locator('#plot .plot-point').count()==3
 page.locator('#plot .plot-point').nth(1).click();assert '3 dB' in page.locator('#chart-reading').inner_text()
 page.locator('.chart-options').evaluate('(e)=>e.open=true');page.select_option('#curve-style','smooth');assert '插值' in page.locator('#interpolation-note').inner_text()
 page.locator('.chart-options').evaluate('(e)=>e.open=false')
 page.evaluate('testPush({busy:true})');assert page.locator('#raw-recipe').get_attribute('contenteditable')=='false'
 page.click('[data-action=pause]');page.wait_for_timeout(50);assert page.evaluate('testRequests.at(-1).operation')=='stop'
 page.evaluate('testPush({authorized:false,busy:false,error:"授权已到期"})');assert page.locator('#auth-screen').is_visible();assert page.locator('#plot .plot-point').count()==0
 page.evaluate('testPush({authorized:true,error:"",points:[],recipe:"",script:"",test:""})');page.wait_for_timeout(100)
 page.evaluate('document.querySelector(".top-right>.badge").textContent="桥接测试 · 无硬件"')
 page.wait_for_timeout(3600)
 for theme in ['blue','graphite','warm']:
  page.select_option('#desktop-theme',theme);page.screenshot(path=str(root/'web-preview'/('workspace-'+theme+'.png')))
 for width,height in [(1366,768),(1920,1080),(1280,720)]:
  page.set_viewport_size({'width':width,'height':height});assert page.locator('.panel').count()==6
  assert page.locator('#app').evaluate('(e)=>getComputedStyle(e).transform')=='none'
  assert page.locator('#app').evaluate('(e)=>e.scrollWidth<=e.clientWidth')
  page.screenshot(path=str(root/'web-preview'/('workspace-'+str(width)+'.png')))
 assert not errors,errors
 print('PASS browser has no native access without host; 6 panels; 32 original patterns; real-operation dispatch; no seeded measurements/curves/recipe; rich-text newline request; real-sample plot; interpolation label; busy/stop; expiration locks; 3 themes; 3 sizes; no JS errors')
 browser.close()
