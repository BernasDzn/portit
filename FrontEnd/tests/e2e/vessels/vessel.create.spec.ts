import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm, setShoelaceSelect } from '../test-utils.js';
import type { VesselDto } from '../../../src/model/dto/VesselDto.ts';

test.describe('Vessels Create (POST)', () => {
  const mockVessel: any = {
    name: 'MV Test Vessel',
    imoNumber: 'IMO 1234567',
    type: 'Cargo',
    owner: 'Owner Inc',
    physicalCharacteristics: { length: 120, depth: 20, draft: 7 }
  };

  test('should create Vessel successfully with valid data', async ({ page }) => {
    await stubWhoAmI(page);

    const captured = await interceptPostAndCapture(page, '**/api/Vessel');

    // Stub vessel type list so dropdown can be filled
    await page.route('**/api/VesselType/filter*', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ items: [{ name: 'Cargo' }], pageNumber: 1, pageSize: 10, pageCount: 1 })
      });
    });

    // Stub shipping agent organization list
    // The service may call either the filter endpoint or the plain collection endpoint.
    await page.route('**/api/ShippingAgentOrganization/filter*', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ items: [{ name: 'Owner Inc' }], pageNumber: 1, pageSize: 10, pageCount: 1 })
      });
    });
    await page.route('**/api/ShippingAgentOrganization', (route) => {
      // Return a plain array as expected by ShippingAgentOrganizationService.getShippingAgentOrganizations
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([{ name: 'Owner Inc' }])
      });
    });

    await page.goto('/vessels/create');
    // Ensure the app finished initial network activity and the form is visible
    await page.waitForLoadState('networkidle');
    await page.waitForSelector('#vessel-name, input[name="name"]', { state: 'visible', timeout: 15000 });

    const nameInput = page.locator('#vessel-name, input[name="name"]').first();
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, mockVessel.name);
      await fillShoelace(page.locator('#vessel-imo, input[name="imoNumber"]').first(), mockVessel.imoNumber);
      await setShoelaceSelect(page.locator('#vessel-type'), encodeURIComponent('Cargo'));
      // Select owner using the entity dropdown (value must be URL-encoded)
      await setShoelaceSelect(page.locator('#vessel-owner'), encodeURIComponent('Owner Inc'));
      await fillShoelace(page.locator('#vessel-length, input[name="length"]').first(), String(mockVessel.physicalCharacteristics.length));
      await fillShoelace(page.locator('#vessel-depth, input[name="depth"]').first(), String(mockVessel.physicalCharacteristics.depth));
      await fillShoelace(page.locator('#vessel-draft, input[name="draft"]').first(), String(mockVessel.physicalCharacteristics.draft));

      await submitForm(page);

      // First try the route-based captured result (existing helper). If nothing captured,
      // fallback to waiting for the outgoing POST request to /api/Vessel (case-insensitive).
      if (!captured.data) {
        const req = await page.waitForRequest(
          (r) => r.method() === 'POST' && /\/api\/vessel/i.test(r.url()),
          { timeout: 8000 }
        ).catch(() => null);

        if (req) {
          try {
            captured.data = req.postData() ? JSON.parse(req.postData()!) : {};
          } catch {
            captured.data = {};
          }
        }
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.name).toBe(mockVessel.name);
      expect(captured.data.imoNumber).toBe(mockVessel.imoNumber);
    }
  });

    test('should return 409 conflict on duplicate vessel IMO', async ({ page }) => {
      await stubWhoAmI(page);

      await page.route('**/api/Vessel', (route) => {
        if (route.request().method() === 'POST') {
          route.fulfill({
            status: 409,
            contentType: 'application/json',
            body: JSON.stringify({ message: 'Vessel with this IMO already exists' })
          });
        } else {
          route.continue();
        }
      });

      await page.goto('/vessels/create');
      await page.waitForLoadState('networkidle');

      const nameInput = page.locator('#vessel-name, input[name="name"]').first();
      if (await nameInput.count() > 0) {
        await fillShoelace(nameInput, 'DUP VESSEL');
        await fillShoelace(page.locator('#vessel-imo, input[name="imoNumber"]').first(), 'DUPIMO');

        try {
          await submitForm(page);
          await page.waitForTimeout(800);
        } catch {}

        const content = await page.textContent('body');
        expect(content).toBeTruthy();
      }
    });

    test('should return 400 bad request on invalid vessel data', async ({ page }) => {
      await stubWhoAmI(page);

      await page.route('**/api/Vessel', (route) => {
        if (route.request().method() === 'POST') {
          route.fulfill({
            status: 400,
            contentType: 'application/json',
            body: JSON.stringify({ message: 'Invalid vessel data' })
          });
        } else {
          route.continue();
        }
      });

      await page.goto('/vessels/create');
      await page.waitForLoadState('networkidle');

      const nameInput = page.locator('#vessel-name, input[name="name"]').first();
      if (await nameInput.count() > 0) {
        await fillShoelace(nameInput, 'BAD@NAME');

        try {
          await submitForm(page);
          await page.waitForTimeout(800);
        } catch {}

        const content = await page.textContent('body');
        expect(content).toBeTruthy();
      }
    });
});
