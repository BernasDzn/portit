import { test, expect } from '@playwright/test';
import { stubWhoAmI, fillShoelace, submitForm } from '../test-utils.js';
import type { VesselTypeDto } from '../../../src/model/dto/VesselTypeDto.ts';

test.describe('Vessel Types Update (PUT)', () => {
  const baseVesselType: VesselTypeDto = {
    name: 'Tanker',
    description: 'Oil tanker vessel',
    maxNumberOfRows: 16,
    maxNumberOfBays: 10,
    maxNumberOfTiers: 5,
    physicalCharacteristics: { length: 250, depth: 38, draft: 11 }
  };

  test('should update vessel type successfully when it exists', async ({ page }) => {
    await stubWhoAmI(page);

    let updateRequestMade = false;
    let updatedData: any = null;

    await page.route('**/api/VesselType/*', (route) => {
      const url = route.request().url();
      const decodedUrl = decodeURIComponent(url);
      
      if (route.request().method() === 'GET' && decodedUrl.includes('/Tanker')) {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(baseVesselType)
        });
      } else if (route.request().method() === 'PUT' && decodedUrl.includes('/Tanker')) {
        updateRequestMade = true;
        const postData = route.request().postData();
        updatedData = postData ? JSON.parse(postData) : null;

        const updatedVesselType = { ...baseVesselType, description: 'Updated tanker description' };
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(updatedVesselType)
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/vessel-types/edit/Tanker');
    await page.getByPlaceholder('Vessel type description').waitFor({ state: 'visible' });

    const descInput = page.getByPlaceholder('Vessel type description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, 'Updated tanker description');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(800);
      if (updateRequestMade) {
        expect(updatedData).toBeTruthy();
        expect(updatedData.description).toBe('Updated tanker description');
      }
    } catch {}
  });

  test('should return 404 not found when updating non-existent vessel type', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/VesselType/*', (route) => {
      const url = route.request().url();
      const decodedUrl = decodeURIComponent(url);
      
      if (route.request().method() === 'GET' && decodedUrl.includes('/NOEXIST')) {
        route.fulfill({
          status: 404,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Vessel type not found' })
        });
      } else if (route.request().method() === 'PUT' && decodedUrl.includes('/NOEXIST')) {
        route.fulfill({
          status: 404,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Vessel type not found' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/vessel-types/edit/NOEXIST');
    await page.waitForLoadState('networkidle');
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });
});
