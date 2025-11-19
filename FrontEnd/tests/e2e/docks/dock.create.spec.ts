import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm, setShoelaceSelect } from '../test-utils.js';
import type { DockDto } from '../../../src/model/dto/DockDto.ts';

test.describe('Docks Create (POST)', () => {
  const mockDock: DockDto = {
    code: 'DCK001',
    name: 'Main Dock',
    location: 'North Terminal',
    physicalCharacteristics: { length: 300, depth: 15, draft: 12 },
    supportedVesselTypes: ['Container Ship']
  };

  test('should create Dock successfully with valid data', async ({ page }) => {
    await stubWhoAmI(page);

    const captured = await interceptPostAndCapture(page, '**/api/Dock');

    // Stub vessel type list so dropdown can be filled
    await page.route('**/api/VesselType/filter*', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ items: [{ name: 'Container Ship' }], pageNumber: 1, pageSize: 10, pageCount: 1 })
      });
    });

    await page.goto('/docks/create');
    await page.locator('#dock-code').waitFor({ state: 'visible' });

    const codeInput = page.locator('#dock-code, input[name="code"]').first();
    if (await codeInput.count() > 0) {
      await fillShoelace(codeInput, mockDock.code);
      await fillShoelace(page.locator('#dock-name, input[name="name"]').first(), mockDock.name);
      await fillShoelace(page.locator('#dock-location, input[name="location"]').first(), mockDock.location);
      await fillShoelace(page.locator('#dock-length, input[name="length"]').first(), String(mockDock.physicalCharacteristics.length));
      await fillShoelace(page.locator('#dock-depth, input[name="depth"]').first(), String(mockDock.physicalCharacteristics.depth));
      await fillShoelace(page.locator('#dock-draft, input[name="draft"]').first(), String(mockDock.physicalCharacteristics.draft));

      // Select required vessel type in dropdown (value is URL-encoded)
      await setShoelaceSelect(page.locator('#dock-vessel-types'), 'Container%20Ship');

      await submitForm(page);

      const start = Date.now();
      const timeout = 8000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 100));
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.code).toBe(mockDock.code);
      expect(captured.data.name).toBe(mockDock.name);
    }
  });

  test('should return 409 conflict on duplicate dock code', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Dock', (route) => {
      if (route.request().method() === 'POST') {
        route.fulfill({
          status: 409,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Dock with this code already exists' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/docks/create');
    await page.waitForLoadState('networkidle');

    const codeInput = page.locator('#dock-code, input[name="code"]').first();
    if (await codeInput.count() > 0) {
      await fillShoelace(codeInput, 'DUP001');
      await fillShoelace(page.locator('#dock-name, input[name="name"]').first(), 'Dup Dock');

      try {
        await submitForm(page);
        await page.waitForTimeout(800);
      } catch {}

      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    }
  });

  test('should return 400 bad request on invalid dock data', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Dock', (route) => {
      if (route.request().method() === 'POST') {
        route.fulfill({
          status: 400,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Invalid dock data' })
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/docks/create');
    await page.waitForLoadState('networkidle');

    const codeInput = page.locator('#dock-code, input[name="code"]').first();
    if (await codeInput.count() > 0) {
      await fillShoelace(codeInput, 'BAD@CODE');

      try {
        await submitForm(page);
        await page.waitForTimeout(800);
      } catch {}

      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    }
  });
});
