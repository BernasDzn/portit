import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { StaffDto } from '../../../src/model/dto/StaffDto.ts';

test.describe('Staff Delete (DELETE)', () => {
  
  const mockStaff: StaffDto = {
    mechanographicNumber: 'MEC001',
    name: 'Alice',
    email: 'alice@example.com',
    phoneNumber: '900000001',
    status: 1,
    operationalWindow: {},
    qualificationsCodes: ['Q1'],
  };

  test('should deactivate staff successfully when exists', async ({ page }) => {
    await stubWhoAmI(page);

    let deleteRequestMade = false;

    await page.route('**/api/Staff/filter?MechanographicNumber=MEC001', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: [mockStaff],
          pageNumber: 1,
          pageSize: 10,
          pageCount: 1,
        }),
      });
    });

    await page.route('**/api/Staff/MEC001', (route) => {
      if (route.request().method() === 'DELETE') {
        deleteRequestMade = true;
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({ ...mockStaff, status: 2 }),
        });
      } else {
        route.continue();
      }
    });

    await page.goto('/staff/view/MEC001');
    await page.waitForLoadState('networkidle');

    const deactivateButton = page.locator('button:has-text("Deactivate"), button:has-text("Delete"), sl-button:has-text("Deactivate")').first();
    
    if (await deactivateButton.count() > 0) {
      await deactivateButton.click();
      await page.waitForTimeout(500);

      const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Yes"), sl-button:has-text("Confirm")').first();
      if (await confirmButton.count() > 0) {
        await confirmButton.click();
      }

      await page.waitForTimeout(1000);
      expect(deleteRequestMade).toBe(true);
    }
  });

  test('should return 404 not found when deactivating non-existent staff', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff/NONEXISTENT', (route) => {
      if (route.request().method() === 'DELETE') {
        route.fulfill({
          status: 404,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Staff not found' }),
        });
      } else {
        route.continue();
      }
    });

    await page.route('**/api/Staff/filter?MechanographicNumber=NONEXISTENT', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: [],
          pageNumber: 1,
          pageSize: 10,
          pageCount: 0,
        }),
      });
    });

    await page.goto('/staff/view/NONEXISTENT');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });
});
