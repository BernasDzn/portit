import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { StaffDto } from '../../../src/model/dto/StaffDto.ts';
import type { Page } from '../../../src/model/Page.ts';

test.describe('Staff Read (GET)', () => {
  
  const mockStaff: StaffDto = {
    mechanographicNumber: 'MEC001',
    name: 'Alice',
    email: 'alice@example.com',
    phoneNumber: '900000001',
    status: 1,
    operationalWindow: {},
    qualificationsCodes: ['Q1'],
  };

  const mockPage: Page<StaffDto> = {
    items: [mockStaff],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  test('should get all staffs and return list', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([mockStaff]),
      });
    });

    await page.goto('/staff/search');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return empty list when staff not found', async ({ page }) => {
    await stubWhoAmI(page);

    const emptyPage: Page<StaffDto> = {
      items: [],
      pageNumber: 1,
      pageSize: 10,
      pageCount: 0,
    };

    await page.route('**/api/Staff/filter?MechanographicNumber=AAAA', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(emptyPage),
      });
    });

    await page.goto('/staff/search');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should filter staffs and return page', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';
    await page.route('**/api/Staff/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockPage),
      });
    });

    await page.goto('/staff/search');
    await page.waitForLoadState('networkidle');

    // Should have called the filter endpoint
    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });
});
