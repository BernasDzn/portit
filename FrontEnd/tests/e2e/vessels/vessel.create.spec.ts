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
    await page.route('**/api/ShippingAgentOrganization/filter*', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ items: [{ name: 'Owner Inc' }], pageNumber: 1, pageSize: 10, pageCount: 1 })
      });
    });

    await page.goto('/vessels/create');
    await page.locator('#vessel-name').waitFor({ state: 'visible' });

    const nameInput = page.locator('#vessel-name, input[name="name"]').first();
    if (await nameInput.count() > 0) {
      await fillShoelace(nameInput, mockVessel.name);
      await fillShoelace(page.locator('#vessel-imo, input[name="imoNumber"]').first(), mockVessel.imoNumber);
      await setShoelaceSelect(page.locator('#vessel-type'), encodeURIComponent('Cargo'));
      // Select owner by typing into the visible combobox/input and confirming the choice.
      const ownerField = page.locator('#vessel-owner, input[name="owner"]').first();
      if (await ownerField.count() > 0) {
        await ownerField.click();
        await fillShoelace(ownerField, 'Owner Inc');
        // Press Enter to accept the suggestion
        await page.keyboard.press('Enter');
        // Fallback: if a suggestion list item is present, click it
        const suggestion = page.locator('text=Owner Inc').first();
        if (await suggestion.count() > 0) {
          await suggestion.click().catch(() => {});
        }
      }
      await fillShoelace(page.locator('#vessel-length, input[name="length"]').first(), String(mockVessel.physicalCharacteristics.length));
      await fillShoelace(page.locator('#vessel-depth, input[name="depth"]').first(), String(mockVessel.physicalCharacteristics.depth));
      await fillShoelace(page.locator('#vessel-draft, input[name="draft"]').first(), String(mockVessel.physicalCharacteristics.draft));

      await submitForm(page);

      const start = Date.now();
      const timeout = 8000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 100));
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.name).toBe(mockVessel.name);
      expect(captured.data.imoNumber).toBe(mockVessel.imoNumber);
    }
  });
});
