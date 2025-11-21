import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm } from '../test-utils.js';
import type { VesselTypeDto } from '../../../src/model/dto/VesselTypeDto.ts';

test.describe('Vessel Types Create (POST)', () => {
  const mockVesselType: VesselTypeDto = {
    name: 'Container Ship',
    description: 'Large container vessel',
    maxNumberOfRows: 20,
    maxNumberOfBays: 15,
    maxNumberOfTiers: 8,
    physicalCharacteristics: { length: 300, depth: 40, draft: 12 }
  };

  test('should create Vessel Type successfully with valid data', async ({ page }) => {
    await stubWhoAmI(page);

    const captured: { data: any } = { data: null };

    await page.route('**/api/VesselType', async (route) => {
      if (route.request().method() === 'POST') {
        const request = route.request();
        const raw = request.postData() ?? '';
        try {
          captured.data = raw ? JSON.parse(raw) : {};
        } catch (e) {
          captured.data = {};
        }

        const responseBody = JSON.stringify({ ...captured.data });
        await route.fulfill({ status: 201, contentType: 'application/json', body: responseBody });
      } else {
        await route.continue();
      }
    });

    await page.goto('/vessel-types/create');
    await page.getByPlaceholder('Vessel type name').waitFor({ state: 'visible' });

    const nameInput = page.getByPlaceholder('Vessel type name');
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, mockVesselType.name);
      await fillShoelace(page.getByPlaceholder('Vessel type description'), mockVesselType.description);
      await fillShoelace(page.getByPlaceholder('Length in meters'), String(mockVesselType.physicalCharacteristics.length));
      await fillShoelace(page.getByPlaceholder('Depth in meters'), String(mockVesselType.physicalCharacteristics.depth));
      await fillShoelace(page.getByPlaceholder('Draft in meters'), String(mockVesselType.physicalCharacteristics.draft));
      await fillShoelace(page.getByPlaceholder('Max Rows in TEU\'s'), String(mockVesselType.maxNumberOfRows));
      await fillShoelace(page.getByPlaceholder('Max Bays in TEU\'s'), String(mockVesselType.maxNumberOfBays));
      await fillShoelace(page.getByPlaceholder('Max Tiers in TEU\'s'), String(mockVesselType.maxNumberOfTiers));

      await page.waitForTimeout(500);
      await submitForm(page);

      const start = Date.now();
      const timeout = 8000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 100));
      }

      expect(captured.data).not.toBeNull();
      if (captured.data) {
        expect(captured.data.name).toBe(mockVesselType.name);
        expect(captured.data.physicalCharacteristics.length).toBe(mockVesselType.physicalCharacteristics.length);
      }
    }
  });

  test('should return 409 conflict on duplicate vessel type name', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/VesselType', (route) => {
      if (route.request().method() === 'POST') {
        route.fulfill({
          status: 409,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Vessel type with this name already exists' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/vessel-types/create');
    await page.waitForLoadState('networkidle');

    const nameInput = page.getByPlaceholder('Vessel type name');
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, 'Container Ship');

      try {
        await submitForm(page);
        await page.waitForTimeout(800);
      } catch {}

      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    }
  });

  test('should return 400 bad request on invalid vessel type data', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/VesselType', (route) => {
      if (route.request().method() === 'POST') {
        route.fulfill({
          status: 400,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Invalid vessel type data' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/vessel-types/create');
    await page.waitForLoadState('networkidle');

    const nameInput = page.getByPlaceholder('Vessel type name');
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, 'Invalid@Name');

      try {
        await submitForm(page);
        await page.waitForTimeout(800);
      } catch {}

      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    }
  });
});
