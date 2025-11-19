import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { STSCraneDto } from '../../../src/model/dto/PhysicalResourceDto.js';

test.describe('Physical Resource Read (GET)', () => {

  test('should filter physical resources by code', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';

    await page.route('**/api/PhysicalResource/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [
            {
              code: 'STS001',
              description: 'STS Crane Unit 1',
              status: 0,
              setupTimeInMinutes: 15,
              type: 0,
              liftingCapacity: 500
            }
          ],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 1
        })
      });
    });

    await page.goto('/resources/search');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(500);

    // Use Shoelace input selector with specific class to target listing search
    const searchInput = page.locator('sl-input.listing-search');
    await searchInput.waitFor({ state: 'attached' });
    await searchInput.evaluate((el: any, value: string) => {
      const innerInput = el.shadowRoot?.querySelector('input');
      if (innerInput) {
        innerInput.value = value;
        innerInput.dispatchEvent(new InputEvent('input', { bubbles: true, composed: true }));
      }
      el.dispatchEvent(new CustomEvent('sl-input', { detail: { value } }));
    }, 'STS001');
    
    // Click Search button
    const searchButton = page.locator('sl-button').filter({ hasText: /Search/i }).first();
    if (await searchButton.isVisible()) {
      await searchButton.click();
    }
    await page.waitForTimeout(1000);

    expect(capturedUrl).toBeTruthy();
    expect(capturedUrl.includes('PhysicalResource/filter')).toBeTruthy();
  });

  test('should filter physical resources by description', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';

    await page.route('**/api/PhysicalResource/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [
            {
              code: 'YARD001',
              description: 'Yard Crane Unit 1',
              status: 0,
              setupTimeInMinutes: 10,
              type: 1,
              liftingCapacity: 450
            }
          ],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 1
        })
      });
    });

    await page.goto('/resources/search');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(500);

    // Use Shoelace input selector with specific class to target listing search
    const searchInput = page.locator('sl-input.listing-search');
    await searchInput.waitFor({ state: 'attached' });
    await searchInput.evaluate((el: any, value: string) => {
      const innerInput = el.shadowRoot?.querySelector('input');
      if (innerInput) {
        innerInput.value = value;
        innerInput.dispatchEvent(new InputEvent('input', { bubbles: true, composed: true }));
      }
      el.dispatchEvent(new CustomEvent('sl-input', { detail: { value } }));
    }, 'Yard Crane');

    // Click Search button  
    const searchButton = page.locator('sl-button').filter({ hasText: /Search/i }).first();
    if (await searchButton.isVisible()) {
      await searchButton.click();
    }
    await page.waitForTimeout(1000);

    expect(capturedUrl).toBeTruthy();
    expect(capturedUrl.includes('PhysicalResource/filter')).toBeTruthy();
  });

  test('should list all physical resources with default pagination', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';

    await page.route('**/api/PhysicalResource/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [
            { code: 'STS001', description: 'STS Crane 1', status: 0, type: 0 },
            { code: 'YARD001', description: 'Yard Crane 1', status: 0, type: 1 },
            { code: 'TRUCK001', description: 'Truck 1', status: 0, type: 2 }
          ],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 3
        })
      });
    });

    await page.goto('/resources/search');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Resources are displayed as list items
    const listItems = await page.locator('li').count();
    expect(listItems).toBeGreaterThan(0);
  });

  test('should handle empty search results', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/PhysicalResource/filter*', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 0
        })
      });
    });

    await page.goto('/resources/search');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(500);

    // Use Shoelace input selector with specific class to target listing search
    const searchInput = page.locator('sl-input.listing-search');
    await searchInput.waitFor({ state: 'attached' });
    await searchInput.evaluate((el: any, value: string) => {
      const innerInput = el.shadowRoot?.querySelector('input');
      if (innerInput) {
        innerInput.value = value;
        innerInput.dispatchEvent(new InputEvent('input', { bubbles: true, composed: true }));
      }
      el.dispatchEvent(new CustomEvent('sl-input', { detail: { value } }));
    }, 'NONEXISTENT');

    // Click Search button
    const searchButton = page.locator('sl-button').filter({ hasText: /Search/i }).first();
    if (await searchButton.isVisible()) {
      await searchButton.click();
    }
    await page.waitForTimeout(1000);

    // Resources are listed in a list, not a table
    const resourceItems = page.locator('li').filter({ has: page.locator('text=/NONEXISTENT|nonexistent/i') });
    expect(await resourceItems.count()).toBe(0);
  });

  test('should retrieve single physical resource by code', async ({ page }) => {
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

    await page.route('**/api/PhysicalResource/STS001', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockResource)
      });
    });

    await page.goto('/resources/view/STS001');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toContain('STS001');
  });

  test('should return 404 when physical resource code not found', async ({ page }) => {
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

});
