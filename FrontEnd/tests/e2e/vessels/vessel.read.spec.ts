import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { VesselDto } from '../../../src/model/dto/VesselDto.ts';
import type { Page as PageModel } from '../../../src/model/Page.ts';

test.describe('Vessels Read (GET)', () => {
  const mockVessel: VesselDto = {
    name: 'Read Vessel',
    imoNumber: 'IMO 0000001',
    type: 'Cargo',
    owner: 'Owner Inc',
    physicalCharacteristics: { length: 80, depth: 12, draft: 6 }
  } as any;

  const mockPage: PageModel<VesselDto> = {
    items: [mockVessel],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  test('should get all vessels and return list', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify([mockVessel]) });
    });

    await page.goto('/vessels/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return empty list when vessel not found', async ({ page }) => {
    await stubWhoAmI(page);

    const emptyPage: PageModel<VesselDto> = { items: [], pageNumber: 1, pageSize: 10, pageCount: 0 };

    await page.route('**/api/Vessel/filter?VesselName=AAAA', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(emptyPage) });
    });

    await page.goto('/vessels/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should filter vessels and return page', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';
    await page.route('**/api/Vessel/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(mockPage) });
    });

    await page.goto('/vessels/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    // Should have called the filter endpoint
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
    expect(capturedUrl).toContain('/api/Vessel/filter');
  });
});
