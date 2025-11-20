import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Vessels' }).click();
  await page.getByRole('link', { name: 'search View Vessels Manage' }).click();
  await page.getByText('Manage and view all').click();
  await page.getByRole('link', { name: 'Vessel Dashboard' }).click();
  await page.getByRole('link', { name: 'add Create Vessel Register a' }).click();
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('VSLTESTADD');
  await page.getByPlaceholder('Vessel IMO number').click();
  await page.getByPlaceholder('Vessel IMO number').fill('IMO 1234567');
  await page.getByPlaceholder('Select vessel type').click();
  await page.locator('.option__label').first().click();
  await page.locator('#vessel-owner').click();
  await page.locator('sl-select#vessel-owner sl-option').first().click();
  await page.getByPlaceholder('Length in meters').click();
  await page.getByPlaceholder('Length in meters').fill('100');
  await page.getByPlaceholder('Length in meters').press('Tab');
  await page.getByPlaceholder('Depth in meters').fill('10');
  await page.getByPlaceholder('Depth in meters').press('Tab');
  await page.getByPlaceholder('Draft in meters').fill('10');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Vessels Manage' }).click();
  await page.getByRole('main').filter({ hasText: 'Vessel Dashboard Search' }).getByPlaceholder('Search...').fill('VSLTESTADD');
  await expect(page.getByRole('link', { name: 'VSLTESTADD Capesize IMO' })).toBeVisible();
});

test('Ensure cannot submit vessel form with empty fields or invalid data', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Vessels' }).click();
  await page.getByRole('link', { name: 'add Create Vessel Register a' }).click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Create Vessel' })).toBeVisible();
});

test('Ensure cannot create duplicate vessel', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Vessels', exact: true }).click();
  await page.getByRole('link', { name: 'add Create Vessel Register a' }).click();
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('DUPL');
  await page.getByPlaceholder('Vessel IMO number').click();
  await page.getByPlaceholder('Vessel IMO number').fill('IMO 6699530');
  await page.getByPlaceholder('Select vessel type').click();
  await page.locator('.option__label').first().click();
  await page.locator('#vessel-owner').click();
  await page.locator('sl-select#vessel-owner sl-option').first().click();
  await page.getByPlaceholder('Length in meters').click();
  await page.getByPlaceholder('Length in meters').fill('10');
  await page.getByPlaceholder('Length in meters').press('Tab');
  await page.getByPlaceholder('Depth in meters').fill('10');
  await page.getByPlaceholder('Depth in meters').press('Tab');
  await page.getByPlaceholder('Draft in meters').fill('10');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.locator('.alert__message')).toBeVisible();
});

test('Ensure vessel editing works correctly', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Vessels' }).click();
  await page.getByRole('link', { name: 'search View Vessels Manage' }).click();
  await page.getByRole('main').filter({ hasText: 'Vessel Dashboard Search' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Vessel Dashboard Search' }).getByPlaceholder('Search...').fill('Ever Given');
  await page.getByRole('link', { name: 'Ever Given Panamax IMO' }).click();
  await page.getByRole('button', { name: 'Edit Vessel' }).click();
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('Ever Changed');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Ever Changed');
  await page.getByRole('button', { name: 'Edit Vessel' }).click();
  await page.getByPlaceholder('Vessel name').dblclick();
  await page.getByPlaceholder('Vessel name').fill('Ever Given');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Ever Given');
});