import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm } from './test-utils.js';
import type { StaffCreate } from '../src/model/Staff.ts';

test('creates a staff member via the create form', async ({ page }) => {
  
  //Define test data
  const staffTest: StaffCreate = {
    mechanographicNumber: 'STF000001',
    status: 1,
    name: 'Test User',
    email: 'test.user@example.com',
    phoneNumber: '900000000',
    operationalWindow: {}
  };

  // Stub authentication check and intercept Staff POST using helpers
  await stubWhoAmI(page);
  const captured = await interceptPostAndCapture(page, '**/api/Staff');

  // Navigate to the staff creation page
  await page.goto('/staff/create');

  // Name
  await page.waitForSelector('#staff-name', { state: 'attached', timeout: 5000 });
  await fillShoelace(page.locator('#staff-name'), staffTest.name);

  // Email
  await page.waitForSelector('#staff-email', { state: 'attached', timeout: 5000 });
  await fillShoelace(page.locator('#staff-email'), staffTest.email);

  // Phone
  await page.waitForSelector('#staff-phone', { state: 'attached', timeout: 5000 });
  await fillShoelace(page.locator('#staff-phone'), staffTest.phoneNumber);

  await submitForm(page);

  // Poll for the intercepted POST payload (route handler sets captured.data)
  const start = Date.now();
  const timeout = 8000;
  while (!captured.data && Date.now() - start < timeout) {
    // eslint-disable-next-line no-await-in-loop
    await new Promise((r) => setTimeout(r, 100));
  }

  expect(captured.data).not.toBeNull();
  expect(captured.data.name).toBe(staffTest.name);
  expect(captured.data.email).toBe(staffTest.email);
  expect(captured.data.phoneNumber).toBe(staffTest.phoneNumber);
});
