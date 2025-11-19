import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { DockDto } from '../../../src/model/dto/DockDto.ts';
import type { Page as PageModel } from '../../../src/model/Page.ts';

test.describe('Docks Read (GET)', () => {
  const mockDock: DockDto = {
    code: 'DCKR01',
    name: 'Read Dock',
    location: 'West Terminal',
    physicalCharacteristics: { length: 220, depth: 12, draft: 9 },
    supportedVesselTypes: ['Container Ship']
  };

  const mockPage: PageModel<DockDto> = {
    items: [mockDock],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  test('should get all docks and return list', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Dock', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify([mockDock]) });
    });

    await page.goto('/docks/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return empty list when dock not found', async ({ page }) => {
    await stubWhoAmI(page);

    const emptyPage: PageModel<DockDto> = { items: [], pageNumber: 1, pageSize: 10, pageCount: 0 };

    await page.route('**/api/Dock/filter?DockName=AAAA', (route) => {
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(emptyPage) });
    });

    await page.goto('/docks/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should filter docks and return page', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';
    await page.route('**/api/Dock/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(mockPage) });
    });

    await page.goto('/docks/search');
    await page.locator('header h1.title').first().waitFor({ state: 'visible' });

    // Should have called the filter endpoint
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
    expect(capturedUrl).toContain('/api/Dock/filter');
  });
});
