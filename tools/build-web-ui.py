"""Reuse approved preview markup/styles; ship no preview simulation JavaScript."""
from pathlib import Path
import re,json,hashlib
root=Path(__file__).resolve().parents[1];source=root/'design-preview/index.html';out=root/'Testplan/Web';out.mkdir(exist_ok=True)
s=source.read_text();defs=json.loads(re.search(r'const defs=(\[.*?\]);let patternIndex',s,re.S)[1])
# All scripts are replaced, not overridden: no seeded curves, fake connections or fake recipe executor ships.
s=re.sub(r'<script>.*?</script>','',s,flags=re.S)
s=s.replace('<head>','<head>\n<meta http-equiv="Content-Security-Policy" content="default-src \'none\'; script-src \'self\'; style-src \'self\' \'unsafe-inline\'; img-src \'self\' data:; connect-src \'none\'; object-src \'none\'; base-uri \'none\'; form-action \'none\'">')
s=re.sub(r'<header class="review">.*?</header>','',s,flags=re.S)
s=re.sub(r'<div class="review-note">.*?</div>','',s,flags=re.S)
s=re.sub(r'<footer class="help">.*?</footer>','',s,flags=re.S)
s=re.sub(r'(<select id="port">).*?(</select>)',r'\1\2',s,flags=re.S)
s=s.replace('实测点（演示）','C# 实测点').replace('等待演示采样…','等待实测数据…').replace('当前测试图案（演示）','当前原图案预览').replace('完整工作台设计预览','本机测试工作台')
s=re.sub(r'<span id="previous-legend".*?</span>','',s,flags=re.S)
s=s.replace('</head>' ,'<link rel="stylesheet" href="desktop.css"></head>')
s=s.replace('</body>','''<div id="auth-screen" role="dialog" aria-modal="true" aria-label="ElecTest2026 授权"><div class="auth-card"><div class="eyebrow">WUHAO STUDIO</div><h1>ElecTest2026</h1><p>Design by Wuhao Studio</p><label class="field"><span>授权名单路径</span><input id="license-path" autocomplete="off" placeholder="本地文件或共享盘 UNC 路径"></label><label class="auth-save"><input type="checkbox" id="save-license">验证成功后保存路径</label><button id="authorize" class="primary">检查授权</button><p id="auth-error" role="status">正在连接本机程序…</p><button id="disconnect-locked">断开设备</button></div></div><script src="patterns.js"></script><script src="native.js"></script></body>''')
s=s.replace('演示脚本编辑器','ARM 脚本编辑器').replace('演示配方步骤','配方步骤').replace('可编辑富文本配方','配方原文编辑器')
s=re.sub(r'(<textarea class="editor".*?>).*?(</textarea>)',r'\1\2',s,flags=re.S)
s=re.sub(r'(<div id="raw-recipe"[^>]*>).*?(</div>)',r'\1\2',s,flags=re.S)
s=s.replace('交互演示','C# 本机引擎').replace('无真实硬件','等待设备连接').replace('设计预览 · 无硬件连接','本机工作台').replace('演示数据','真实数据待采集').replace('演示值','实测值')
s=s.replace('Chart2 · 对比','Chart2 · 待定义').replace('步骤表 · 演示','原文步骤视图').replace('演示测量项目','当前测量项目').replace('表格为独立演示，不覆盖原文','原文与步骤视图使用同一配方')
s=s.replace('运行配方','从头运行').replace('演示配方','配方原文').replace('演示脚本','ARM 脚本')
s=s.replace('仅预览，不向设备发送指令','下方预览仅展示；选择图案将调用原 C# 图案发送流程')
s=s.replace('点击图案预览','选择图案并输出')
s=s.replace('<span>Back · 背景</span>','<span>Back · 参数</span>')
s=s.replace('<input type="number" value="1920"','<input id="pattern-width" type="number" value="0"').replace('<input type="number" value="1080"','<input id="pattern-height" type="number" value="0"')
s=s.replace('<div class="row" style="justify-content:space-between"><span class="section-label" style="margin:0">PATTERN LIBRARY', '<div class="pattern-apply"><button id="apply-rgb">应用 RGB</button><button id="apply-gray">应用 Gray</button><button id="apply-back">应用 Back</button></div><div class="row" style="justify-content:space-between"><span class="section-label" style="margin:0">PATTERN LIBRARY')
s=s.replace('<input type="checkbox" checked>DownCOM','<input id="downcom" type="checkbox">DownCOM').replace('<label><input type="checkbox" checked>DownLoad</label>','')
s=s.replace('START · 起始','START · HEX').replace('END · 结束','END · HEX').replace('STEP · 步长','STEP · HEX').replace('Delay · ms','Delay · 原单位').replace('Step Time · ms','Step Time · 原单位')
for key in ['start-value','end-value','step-value']:s=s.replace('id="'+key+'" type="number"','id="'+key+'" type="text"')
# Dedicated original GoTo argument distinct from viewer line navigation.
s=s.replace('<div class="recipe-modes">','<label class="field goto-argument"><span>Goto 指令目标行（原参数）</span><input id="goto-value" type="number" min="0" max="65535" value="0"></label><div class="recipe-modes">')
s=s.replace('<div class="recipe-toolbar">','<div class="recipe-toolbar"><button id="bold-recipe" data-icon="format" title="配方选中文字加粗">加粗</button><button id="italic-recipe" title="配方选中文字斜体">斜体</button>')
s=s.replace('<div class="table-wrap" id="recipe-table">','<div class="table-wrap" id="recipe-table">')
s=s.replace('<h2>测试图案</h2>','<h2>测试图案</h2>').replace('<span id="recipe-message">选择步骤以编辑 · 配方原文</span>','<span id="recipe-message">配方尚未加载</span>')
# Selector stays a read-only label: the current C# recipe determines Chart1.
s=re.sub(r'<select id="chart-project".*?</select>','<span id="chart-project">等待当前测试</span>',s,flags=re.S)
s=re.sub(r'<div class="chart-foot">.*?</div>','<div class="chart-foot"><span id="chart-live-state">等待实测数据</span></div>',s,flags=re.S)
s=re.sub(r'<div id="sheet-view".*?<div class="voltage">','<div id="sheet-view" class="sheet" hidden><table id="native-sheet"><thead></thead><tbody></tbody></table></div><div class="voltage">',s,flags=re.S)
s=s.replace('<small id="pattern-count">32 个图案</small>','<small id="pattern-count">32 个原图案</small>')
s=s.replace('WHITE / 255','尚未选择图案').replace('300.00','—')
# Seven original metric fields (Lv is shown separately); no fabricated frequency reading.
for label,key in [('sx','sx'),('sy','sy'),('X','x'),('Y','y'),('Z','z'),('Flicker','jeita'),('F','fma')]:
 s=re.sub(r'(<div class="metric">)'+label+r' <b data-measure="[^"]*">.*?</b></div>',lambda m:m[1]+('JEITA · dB' if key=='jeita' else 'FMA · %' if key=='fma' else label)+' <b data-reading="'+key+'">—</b></div>',s)
s=re.sub(r'<div class="metric">dB <b data-measure="[^"]*">.*?</b></div>','',s)
# ARM parameters use the original hexadecimal editors.
s=re.sub(r'(<div class="voltage">)(.*?)(</div>)',lambda m:m[1]+m[2].replace('type="number"','type="text"')+m[3],s,flags=re.S)
# Unconnected metrics must never look measured.
s=re.sub(r'(<b data-measure="[^"]*">).*?(</b>)',r'\1—\2',s)
s=re.sub(r'(<span data-measure="[^"]*">).*?(</span>)',r'\1—\2',s)
(out/'index.html').write_text(s)
(out/'patterns.js').write_text('"use strict";\nconst patternDefinitions='+json.dumps(defs,ensure_ascii=False,separators=(',',':'))+';\n')
(out/'SOURCE.json').write_text(json.dumps({'source':'design-preview/index.html','sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'builder':'tools/build-web-ui.py','runtimeScripts':['patterns.js','native.js'],'previewSimulationIncluded':False},indent=2)+'\n')
print('Built Web/index.html from approved markup; all simulation scripts excluded')
