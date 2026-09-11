import { chromium } from 'playwright';

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();

  await new Promise(r => setTimeout(r, 8000));

  try {
    await page.goto('http://localhost:8081');
    await page.waitForTimeout(5000);
    await page.screenshot({ path: 'glod_app_portrait.png' });
    console.log('Screenshot taken at glod_app_portrait.png');

    // Simulate Landscape Tablet
    await page.setViewportSize({ width: 1024, height: 768 });
    await page.waitForTimeout(2000);
    await page.screenshot({ path: 'glod_app_landscape.png' });
    console.log('Screenshot taken at glod_app_landscape.png');

  } catch(e) {
    console.error('Playwright error:', e);
  } finally {
    await browser.close();
  }
})();
