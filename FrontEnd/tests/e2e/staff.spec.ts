import { test, expect } from '@playwright/test';

test('Ensure staff member is created with valid data and is listed', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Staffs' }).click();
  await page.getByRole('link', { name: 'add Create Staff Member' }).click();
  await page.getByPlaceholder('Staff name').click();
  await page.getByPlaceholder('Staff name').fill('STAFFTEST');
  await page.getByPlaceholder('Email address').click();
  await page.getByPlaceholder('Email address').fill('test@test.test');
  await page.getByPlaceholder('Phone number').click();
  await page.getByPlaceholder('Phone number').fill('910000000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Staff Members' }).click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').fill('STAFFTEST');
  await expect(page.getByRole('link', { name: 'STAFFTEST' }).first()).toBeVisible();
});

test('Ensure staff member is updated correctly on update', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Staffs' }).click();
  await page.getByRole('link', { name: 'add Create Staff Member' }).click();
  await page.getByPlaceholder('Staff name').click();
  await page.getByPlaceholder('Staff name').fill('STAFFEDITTEST');
  await page.getByPlaceholder('Email address').click();
  await page.getByPlaceholder('Email address').fill('test@test.test');
  await page.getByPlaceholder('Phone number').click();
  await page.getByPlaceholder('Phone number').fill('910000000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Staff Members' }).click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').fill('STAFFEDITTEST');
  await page.getByRole('link', { name: 'STAFFEDITTEST' }).first().click();
  await page.getByRole('button', { name: 'Edit Staff Member' }).click();
  await page.getByPlaceholder('Staff name').click();
  await page.getByPlaceholder('Staff name').fill('STAFFEDITTEST EDITED');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('STAFFEDITTEST EDITED');
});

test('Ensure validation prevents creating staff member with empty fields or incorrect data', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Staffs' }).click();
  await page.getByRole('link', { name: 'add Create Staff Member' }).click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Create Staff Member' })).toBeVisible();
});

test('Ensure staff member can be deactivated correctly', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Staffs' }).click();
  await page.getByRole('link', { name: 'add Create Staff Member' }).click();
  await page.getByPlaceholder('Staff name').click();
  await page.getByPlaceholder('Staff name').fill('STAFFDISABLE');
  await page.getByPlaceholder('Email address').click();
  await page.getByPlaceholder('Email address').fill('test@test.test');
  await page.getByPlaceholder('Phone number').click();
  await page.getByPlaceholder('Phone number').fill('910000000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Staff Members' }).click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').fill('STAFFDISABLE');
  await page.getByRole('link', { name: 'STAFFDISABLE' }).first().click();
  await page.getByRole('button', { name: 'Deactivate' }).click();
  await page.locator('sl-dialog').getByRole('button', { name: 'Deactivate' }).click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Staff Dashboard Search Staff' }).getByPlaceholder('Search...').fill('STAFFDISABLE');
  await expect(page.getByText('No results found.')).toBeVisible();
});