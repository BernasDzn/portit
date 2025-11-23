import { test, expect } from '@playwright/test';

test('Ensure creating vessel with valid data works', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Vessels' }).click();
  await page.getByRole('link', { name: 'add Create Vessel Register a' }).click();
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('test');
  await page.getByPlaceholder('Vessel name').press('Tab');
  await page.getByPlaceholder('Vessel IMO number').fill('IMO 1817993');
  await page.getByPlaceholder('Vessel IMO number').press('Tab');
  await page.waitForTimeout(500);
  await page.getByPlaceholder('Select vessel type').press('Enter');
  await page.getByRole('option', { name: 'Capesize' }).press('Enter');
  await page.waitForTimeout(500);
  await page.getByPlaceholder('Select vessel type').press('Tab');
  await page.getByPlaceholder('Select owning organization').press('Enter');
  await page.getByRole('option', { name: 'Global Shipping Co.' }).press('Enter');
  await page.getByPlaceholder('Select owning organization').press('Tab');
  await page.getByPlaceholder('Length in meters').fill('1');
  await page.getByPlaceholder('Length in meters').press('Tab');
  await page.getByPlaceholder('Depth in meters').fill('1');
  await page.getByPlaceholder('Depth in meters').press('Tab');
  await page.getByPlaceholder('Draft in meters').fill('1');
  await page.getByPlaceholder('Draft in meters').press('Tab');
  await page.getByRole('button', { name: 'Cancel' }).press('Tab');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Vessels Manage' }).click();
  await page.getByRole('main').filter({ hasText: 'Vessel Dashboard Search' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Vessel Dashboard Search' }).getByPlaceholder('Search...').fill('test');
  await expect(page.getByRole('link', { name: 'test Capesize IMO 1817993' })).toBeVisible();
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
  await page.getByRole('link', { name: 'Vessels' }).click();
  await page.getByRole('link', { name: 'add Create Vessel Register a' }).click();
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('test');
  await page.getByPlaceholder('Vessel name').press('Tab');
  await page.getByPlaceholder('Vessel IMO number').fill('IMO 1817993');
  await page.getByPlaceholder('Vessel IMO number').press('Tab');
  await page.waitForTimeout(500);
  await page.getByPlaceholder('Select vessel type').press('Enter');
  await page.getByRole('option', { name: 'Capesize' }).press('Enter');
  await page.waitForTimeout(500);
  await page.getByPlaceholder('Select vessel type').press('Tab');
  await page.getByPlaceholder('Select owning organization').press('Enter');
  await page.getByRole('option', { name: 'Global Shipping Co.' }).press('Enter');
  await page.getByPlaceholder('Select owning organization').press('Tab');
  await page.getByPlaceholder('Length in meters').fill('1');
  await page.getByPlaceholder('Length in meters').press('Tab');
  await page.getByPlaceholder('Depth in meters').fill('1');
  await page.getByPlaceholder('Depth in meters').press('Tab');
  await page.getByPlaceholder('Draft in meters').fill('1');
  await page.getByPlaceholder('Draft in meters').press('Tab');
  await page.getByRole('button', { name: 'Cancel' }).press('Tab');
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
  await page.waitForTimeout(1000);
  await page.getByPlaceholder('Vessel name').click();
  await page.getByPlaceholder('Vessel name').fill('Ever Changed');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Ever Changed');
  await page.getByRole('button', { name: 'Edit Vessel' }).click();
  await page.waitForTimeout(1000);
  await page.getByPlaceholder('Vessel name').dblclick();
  await page.getByPlaceholder('Vessel name').fill('Ever Given');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Ever Given');
});