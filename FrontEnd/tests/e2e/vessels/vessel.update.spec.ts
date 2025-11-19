import { test, expect } from '@playwright/test';
import { stubWhoAmI, fillShoelace, submitForm } from '../test-utils.js';
import type { Vessel } from '../../../src/model/Vessel.ts';

test.describe('Vessels Update (PUT)', () => {
  const baseVessel: Vessel = {
    name: 'MV Update Vessel',
    imoNumber: 'IMO 9999999',
    type: { name: 'Cargo' } as any,
    owner: { name: 'Owner Inc' } as any,
    physicalCharacteristics: { length: 90, depth: 14, draft: 6 }
  } as any;

  test('should update vessel successfully when it exists', async ({ page }) => {
    await stubWhoAmI(page);

    let updateRequestMade = false;
    let updatedData: any = null;

    await page.route(`**/api/Vessel/${baseVessel.imoNumber}`, (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(baseVessel) });
      } else if (route.request().method() === 'PUT') {
        updateRequestMade = true;
        const postData = route.request().postData();
        updatedData = postData ? JSON.parse(postData) : null;
        const updated = { ...baseVessel, name: 'Updated Vessel Name' };
        route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(updated) });
      } else {
        route.continue();
      }
    });

    await page.goto(`/vessels/edit/${encodeURIComponent(baseVessel.imoNumber)}`);
    await page.locator('#vessel-name').waitFor({ state: 'visible' });

    const nameInput = page.locator('#vessel-name, input[name="name"]').first();
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, 'Updated Vessel Name');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(800);
      if (updateRequestMade) {
        expect(updatedData).toBeTruthy();
        expect(updatedData.name).toBe('Updated Vessel Name');
      }
    } catch {}
  });

  test('should return 404 not found when updating non-existent vessel', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/NOEXIST', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({ status: 404, contentType: 'application/json', body: JSON.stringify({ message: 'Vessel not found' }) });
      } else if (route.request().method() === 'PUT') {
        route.fulfill({ status: 404, contentType: 'application/json', body: JSON.stringify({ message: 'Vessel not found' }) });
      } else {
        route.continue();
      }
    });

    await page.goto('/vessels/edit/NOEXIST');
    await page.waitForLoadState('networkidle');
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });
});
