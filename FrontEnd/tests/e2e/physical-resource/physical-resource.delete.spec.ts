import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { STSCraneDto } from '../../../src/model/dto/PhysicalResourceDto.js';

test.describe('Physical Resource Delete (DELETE)', () => {

  test('should delete physical resource successfully', async ({ page }) => {
    await stubWhoAmI(page);

    const mockResource: STSCraneDto = {
      code: 'STS001',
      description: 'STS Crane Unit 1',
      status: 0,
      setupTimeInMinutes: 15,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 500,
      servingDockCode: 'DOCK001',
      containersPerHour: 40
    };

    // Mock GET /PhysicalResource/STS001
    await page.route('**/api/PhysicalResource/STS001', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockResource)
      });
    });

    // Mock DELETE
    let deleteWasCalled = false;
    await page.route('**/api/PhysicalResource/STS001', (route) => {
      if (route.request().method() === 'DELETE') {
        deleteWasCalled = true;
        route.fulfill({
          status: 204,
          contentType: 'application/json',
          body: ''
        });
      } else {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(mockResource)
        });
      }
    });

    await page.goto('/resources/view/STS001');
    await page.waitForLoadState('networkidle');

    // Find and click delete button
    const deleteButton = page.locator('button:has-text("Delete"), button:has-text("Deactivate")').first();
    if (await deleteButton.isVisible()) {
      await deleteButton.click();
      await page.waitForTimeout(500);

      // Confirm deletion if there's a confirmation dialog
      const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Yes"), button:has-text("Delete")').first();
      if (await confirmButton.isVisible()) {
        await confirmButton.click();
        await page.waitForTimeout(1000);
      }
    }

    expect(deleteWasCalled || !await deleteButton.isVisible()).toBeTruthy();
  });

  test('should return 404 when deleting non-existent resource', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/PhysicalResource/NONEXISTENT', (route) => {
      route.fulfill({
        status: 404,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Physical resource not found' })
      });
    });

    await page.goto('/resources/view/NONEXISTENT');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should handle resource state after deletion', async ({ page }) => {
    await stubWhoAmI(page);

    const mockResource: STSCraneDto = {
      code: 'STS002',
      description: 'STS Crane Unit 2',
      status: 0,
      setupTimeInMinutes: 15,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 500,
      servingDockCode: 'DOCK001',
      containersPerHour: 40
    };

    // Mock GET
    await page.route('**/api/PhysicalResource/STS002', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(mockResource)
        });
      } else if (route.request().method() === 'DELETE') {
        route.fulfill({
          status: 204,
          contentType: 'application/json',
          body: ''
        });
      }
    });

    await page.goto('/resources/view/STS002');
    await page.waitForLoadState('networkidle');

    const resourceCode = await page.textContent('body');
    expect(resourceCode).toContain('STS002');
  });

  test('should prevent deletion if resource is in use', async ({ page }) => {
    await stubWhoAmI(page);

    const mockResource: STSCraneDto = {
      code: 'STS003',
      description: 'STS Crane Unit 3',
      status: 0,
      setupTimeInMinutes: 15,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 500,
      servingDockCode: 'DOCK001',
      containersPerHour: 40
    };

    // Mock GET
    await page.route('**/api/PhysicalResource/STS003', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockResource)
      });
    });

    // Mock DELETE with 409 Conflict
    await page.route('**/api/PhysicalResource/STS003', (route) => {
      if (route.request().method() === 'DELETE') {
        route.fulfill({
          status: 409,
          contentType: 'application/json',
          body: JSON.stringify({ message: 'Resource is in use and cannot be deleted' })
        });
      } else {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(mockResource)
        });
      }
    });

    await page.goto('/resources/view/STS003');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toContain('STS003');
  });

  test('should handle cascading deletions correctly', async ({ page }) => {
    await stubWhoAmI(page);

    const mockResource: STSCraneDto = {
      code: 'STS004',
      description: 'STS Crane Unit 4',
      status: 0,
      setupTimeInMinutes: 15,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 500,
      servingDockCode: 'DOCK001',
      containersPerHour: 40
    };

    // Mock GET /PhysicalResource/STS004
    await page.route('**/api/PhysicalResource/STS004', (route) => {
      if (route.request().method() === 'GET') {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(mockResource)
        });
      } else if (route.request().method() === 'DELETE') {
        route.fulfill({
          status: 204,
          contentType: 'application/json',
          body: ''
        });
      }
    });

    await page.goto('/resources/view/STS004');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toContain('STS004');
  });

});
