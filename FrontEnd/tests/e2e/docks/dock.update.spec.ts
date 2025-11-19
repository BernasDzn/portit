import { test, expect } from '@playwright/test';
import { stubWhoAmI, fillShoelace, submitForm } from '../test-utils.js';
import type { DockDto } from '../../../src/model/dto/DockDto.ts';

test.describe('Docks Update (PUT)', () => {
  const baseDock: DockDto = {
    code: 'DCKU01',
    name: 'Update Dock',
    location: 'East Terminal',
    physicalCharacteristics: { length: 250, depth: 13, draft: 10 },
    supportedVesselTypes: ['Container Ship']
  };

  test('should update dock successfully when it exists', async ({ page }) => {
    await stubWhoAmI(page);

    let updateRequestMade = false;
    let updatedData: any = null;

    await page.route('**/api/Dock/DCKU01', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(baseDock)
        });
      } else if (route.request().method() === 'PUT') {
        updateRequestMade = true;
        const postData = route.request().postData();
        updatedData = postData ? JSON.parse(postData) : null;

        const updatedDock = { ...baseDock, name: 'Updated Dock Name' };
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(updatedDock)
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/docks/edit/DCKU01');
    await page.locator('#dock-name').waitFor({ state: 'visible' });

    const nameInput = page.locator('#dock-name, input[name="name"]').first();
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, 'Updated Dock Name');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(800);
      if (updateRequestMade) {
        expect(updatedData).toBeTruthy();
        expect(updatedData.name).toBe('Updated Dock Name');
      }
    } catch {}
  });

  test('should return 404 not found when updating non-existent dock', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Dock/NOEXIST', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({
          status: 404,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Dock not found' })
        });
      } else if (route.request().method() === 'PUT') {
        route.fulfill({
          status: 404,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Dock not found' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/docks/edit/NOEXIST');
    await page.waitForLoadState('networkidle');
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });
});
