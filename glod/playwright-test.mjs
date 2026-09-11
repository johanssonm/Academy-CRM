import { chromium } from 'playwright';

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();

  // Wait a moment for Expo server to be up
  await new Promise(r => setTimeout(r, 5000));

  try {
    await page.goto('http://localhost:8081');
    // Wait for the fonts and react app to mount
    await page.waitForTimeout(5000);
    await page.screenshot({ path: 'glod_app.png' });
    console.log('Screenshot taken at glod_app.png');
  } catch(e) {
    console.error('Playwright error:', e);
  } finally {
    await browser.close();
  }
})();
