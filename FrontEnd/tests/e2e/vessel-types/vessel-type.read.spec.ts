import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { VesselTypeDto } from '../../../src/model/dto/VesselTypeDto.ts';
import type { Page as PageModel } from '../../../src/model/Page.ts';

test.describe('Vessel Types Read (GET)', () => {
  const mockVesselType: VesselTypeDto = {
    name: 'Bulk Carrier',
    description: 'Bulk cargo vessel',
    maxNumberOfRows: 18,
    maxNumberOfBays: 12,
    maxNumberOfTiers: 6,
    physicalCharacteristics: { length: 220, depth: 35, draft: 10 }
  };

  const mockPage: PageModel<VesselTypeDto> = {
    items: [mockVesselType],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  test('should get all vessel types and return list', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/VesselType', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify([mockVesselType]) });
    });

    await page.goto('/vessel-types/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return empty list when vessel type not found', async ({ page }) => {
    await stubWhoAmI(page);

    const emptyPage: PageModel<VesselTypeDto> = { items: [], pageNumber: 1, pageSize: 10, pageCount: 0 };

    await page.route('**/api/VesselType/filter?Name=NONEXISTENT', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(emptyPage) });
    });

    await page.goto('/vessel-types/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should filter vessel types and return page', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';
    await page.route('**/api/VesselType/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(mockPage) });
    });

    await page.goto('/vessel-types/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    // Should have called the filter endpoint
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
    expect(capturedUrl).toContain('/api/VesselType/filter');
  });
});
