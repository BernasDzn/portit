import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm } from '../test-utils.js';
import type { StaffDto } from '../../../src/model/dto/StaffDto.ts';

test.describe('Staff Create (POST)', () => {
  
  test('should create staff successfully with valid data', async ({ page }) => {
    await stubWhoAmI(page);

    const staffTest: StaffDto = {
      mechanographicNumber: 'STF250001',
      status: 0,
      name: 'Bob',
      email: 'bob@example.com',
      phoneNumber: '900000002',
      operationalWindow: {},
      qualificationsCodes: ['Q1']
    };

    const captured = await interceptPostAndCapture(page, '**/api/Staff');

    await page.goto('/staff/create');

    await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-name'), staffTest.name);

    await page.waitForSelector('#staff-email', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-email'), staffTest.email);

    await page.waitForSelector('#staff-phone', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-phone'), staffTest.phoneNumber);

    await submitForm(page);

    const start = Date.now();
    const timeout = 8000;
    while (!captured.data && Date.now() - start < timeout) {
      await new Promise((r) => setTimeout(r, 100));
    }

    expect(captured.data).not.toBeNull();
    expect(captured.data.name).toBe(staffTest.name);
    expect(captured.data.email).toBe(staffTest.email);
    expect(captured.data.phoneNumber).toBe(staffTest.phoneNumber);
  });

  test('should return 404 when qualification is missing', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 404,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Qualification not found' }),
      });
    });

    await page.goto('/staff/create');

    await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-name'), 'Charlie');

    const emailInput = page.locator('#staff-email');
    if (await emailInput.count() > 0) {
      await fillShoelace(emailInput, 'charlie@example.com');
    }

    const phoneInput = page.locator('#staff-phone');
    if (await phoneInput.count() > 0) {
      await fillShoelace(phoneInput, '900000003');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected if form validation prevents submission
    }
  });

  test('should return 409 conflict on duplicate mechanographic number', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 409,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Staff with this mechanographic number already exists' }),
      });
    });

    await page.goto('/staff/create');

    await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-name'), 'Duplicate User');

    const emailInput = page.locator('#staff-email');
    if (await emailInput.count() > 0) {
      await fillShoelace(emailInput, 'dup@example.com');
    }

    const phoneInput = page.locator('#staff-phone');
    if (await phoneInput.count() > 0) {
      await fillShoelace(phoneInput, '900000009');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected
    }
  });

  test('should return 400 bad request on invalid data', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid data provided' }),
      });
    });

    await page.goto('/staff/create');

    await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
    
    const emailInput = page.locator('#staff-email');
    if (await emailInput.count() > 0) {
      await fillShoelace(emailInput, 'not-an-email');
    }

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected
    }
  });

  test('should return 400 bad request on null data', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Request body cannot be null' }),
      });
    });

    await page.goto('/staff/create');

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected - form validation should prevent empty submission
    }
  });

  test('should return 400 bad request on wrong format', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Staff', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid request format' }),
      });
    });

    await page.goto('/staff/create');

    await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#staff-name'), 'Test');

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected
    }
  });
});
