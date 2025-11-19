import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { Vessel } from '../../../src/model/Vessel.ts';

test.describe('Vessels Read (GET)', () => {
  const baseVessel: Vessel = {
    name: 'MV Read Vessel',
    imoNumber: 'IMO 0000001',
    type: { name: 'Cargo' } as any,
    owner: { name: 'Owner Inc' } as any,
    physicalCharacteristics: { length: 80, depth: 12, draft: 6 }
  } as any;

  test('should display vessel details when vessel exists', async ({ page }) => {
    await stubWhoAmI(page);

    // Use a wildcard route so URL-encoding or minor path variations don't prevent matching
    await page.route('**/api/Vessel*', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(baseVessel)
        });
      } else {
        route.continue();
      }
    });

    await page.goto(`/vessels/view/${encodeURIComponent(baseVessel.imoNumber)}`);
    await page.locator('.view-header .title').waitFor({ state: 'visible' });

    const title = await page.textContent('.view-header .title');
    expect(title).toBe(baseVessel.name);

    const imoText = await page.textContent('.view-header .subtitle');
    expect(imoText).toBe(baseVessel.imoNumber);

    // Check type and owner appear on the page
    const typeText = await page.textContent('text=' + baseVessel.type.name);
    expect(typeText).toBeTruthy();
    const ownerText = await page.textContent('text=' + baseVessel.owner.name);
    expect(ownerText).toBeTruthy();
  });
});
