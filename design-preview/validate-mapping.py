from pathlib import Path
import re
from playwright.sync_api import sync_playwright
root=Path(__file__).resolve().parent;r=root.parent
with sync_playwright() as p:
 b=p.chromium.launch(executable_path='/usr/bin/chromium',args=['--no-sandbox']);page=b.new_page(viewport={'width':1720,'height':1160});errors=[];page.on('pageerror',lambda e:errors.append(str(e)));page.set_content((root/'index.html').read_text())
 designer=(r/'Testplan/Pattern.Designer.cs').read_text(encoding='utf-8-sig')
 for n in range(1,33):
  button=page.locator(f'[data-source="pictureBox{n}"]');tag=re.search(rf'this.pictureBox{n}\.Tag = "([^"]+)"',designer)[1];assert button.get_attribute('data-tag')==tag
  button.click();assert page.locator('#back').is_enabled()==(tag[0] in ['1','2']);assert page.locator('#red').is_enabled()==(tag[0]=='0')
 page.locator('#pattern-disclosure summary').click();page.locator('#patterns').screenshot(path=str(root/'pattern-expanded.png'))
 page.get_by_role('button',name='恢复初始布局与演示数据',exact=True).click()
 page.locator('#text-mode').click();page.locator('#raw-recipe').fill('Delay 10\nDownLoad AB\nSave demo')
 page.fill('#goto','2');page.get_by_role('button',name='跳转步骤',exact=True).click();page.locator('#run-current').click();page.get_by_role('button',name='暂停',exact=True).click();assert page.locator('#progress').inner_text().startswith('02 /')
 page.get_by_role('button',name='上移选中步骤',exact=True).click();assert page.locator('#raw-recipe').inner_text().startswith('DownLoad AB')
 page.get_by_role('button',name='删除选中步骤',exact=True).click();assert 'DownLoad' not in page.locator('#raw-recipe').inner_text()
 page.locator('.control-inputs summary').click();page.locator('#recipe').screenshot(path=str(root/'control-text.png'))
 assert not errors,errors
 print('PASS: all 32 source Tags and parameter enablement; raw current-line/goto/move/delete; expanded Pattern and Control screenshots; no JS errors')
 b.close()
