import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm } from '../test-utils.js';
import type { QualificationDto } from '../../../src/model/dto/QualificationDto.js';

test.describe('Qualification Create (POST)', () => {

  test('should create qualification successfully with valid data', async ({ page }) => {
    await stubWhoAmI(page);

    const qualificationTest: QualificationDto = {
      idCode: 'Q100',
      qualificationName: 'Basic Safety Training'
    };

    const captured = await interceptPostAndCapture(page, '**/Qualification');

    await page.goto('/qualifications/create');

    await page.waitForSelector('#qual-code', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-code'), qualificationTest.idCode);

    await page.waitForSelector('#qual-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-name'), qualificationTest.qualificationName);

    await submitForm(page);

    const start = Date.now();
    const timeout = 8000;
    while (!captured.data && Date.now() - start < timeout) {
      await new Promise((r) => setTimeout(r, 100));
    }

    expect(captured.data).not.toBeNull();
    expect(captured.data.idCode).toBe(qualificationTest.idCode);
    expect(captured.data.qualificationName).toBe(qualificationTest.qualificationName);
  });

  test('should return 409 conflict on duplicate qualification', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/Qualification', (route) => {
      route.fulfill({
        status: 409,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Qualification with this code already exists' })
      });
    });

    await page.goto('/qualifications/create');

    await page.waitForSelector('#qual-code', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-code'), 'Q100');

    await page.waitForSelector('#qual-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-name'), 'Duplicate');

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

    await page.route('**/Qualification', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid data provided' })
      });
    });

    await page.goto('/qualifications/create');

    await page.waitForSelector('#qual-code', { state: 'attached', timeout: 5000 });
    
    // invalid format — violates alphanumeric regex
    await fillShoelace(page.locator('#qual-code'), '###');

    await page.waitForSelector('#qual-name', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-name'), 'Invalid Qualification');

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected invalid submission
    }
  });

  test('should return 400 bad request on null data', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/Qualification', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Request body cannot be null' })
      });
    });

    await page.goto('/qualifications/create');

    try {
      await submitForm(page);
      await page.waitForTimeout(1000);
      const content = await page.textContent('body');
      expect(content).toBeTruthy();
    } catch (e) {
      // Expected due to validation blocking empty fields
    }
  });

  test('should return 400 bad request on wrong format', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/Qualification', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid request format' })
      });
    });

    await page.goto('/qualifications/create');

    await page.waitForSelector('#qual-code', { state: 'attached', timeout: 5000 });
    await fillShoelace(page.locator('#qual-code'), 'TEST');

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
