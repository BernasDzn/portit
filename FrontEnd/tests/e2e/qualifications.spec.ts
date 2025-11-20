import { test, expect } from '@playwright/test';

test('Ensure Qualification creation and search works', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Qualifications' }).click();
  await page.getByRole('link', { name: 'add Create Qualification' }).click();
  await page.getByPlaceholder('Qualification code').click();
  await page.getByPlaceholder('Qualification code').fill('QLFTESTADD');
  await page.getByPlaceholder('Qualification name').click();
  await page.getByPlaceholder('Qualification name').fill('Qualification test add');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Qualifications' }).click();
  await page.getByRole('main').filter({ hasText: 'Qualification Dashboard' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Qualification Dashboard' }).getByPlaceholder('Search...').fill('Qualification test add');
  await expect(page.getByRole('link', { name: 'Qualification test add' })).toBeVisible();
});

test('Ensure cannot create Qualification without required fields', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Qualifications' }).click();
  await page.getByRole('link', { name: 'add Create Qualification' }).click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Create Qualification' })).toBeVisible();
});

test('Ensure cannot create duplicate Qualification', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Qualifications' }).click();
  await page.getByRole('link', { name: 'add Create Qualification' }).click();
  await page.getByPlaceholder('Qualification code').click();
  await page.getByPlaceholder('Qualification code').fill('QUALTESTDUP');
  await page.getByPlaceholder('Qualification name').click();
  await page.getByPlaceholder('Qualification name').fill('Qualification duplicate test');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'add Create Qualification' }).click();
  await page.getByPlaceholder('Qualification code').click();
  await page.getByPlaceholder('Qualification code').fill('QUALTESTDUP');
  await page.getByPlaceholder('Qualification name').click();
  await page.getByPlaceholder('Qualification name').fill('already added qualification');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByText('An error has occured')).toBeVisible();
});

test('Ensure can edit Qualification', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Qualifications' }).click();
  await page.getByRole('link', { name: 'search View Qualifications' }).click();
  await page.getByRole('main').filter({ hasText: 'Qualification Dashboard' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Qualification Dashboard' }).getByPlaceholder('Search...').fill('Truck Driver');
  await page.getByRole('link', { name: 'Truck Driver TRKDR' }).click();
  await page.getByRole('button', { name: 'Edit Qualification' }).click();
  await page.getByPlaceholder('Qualification name').click();
  await page.getByPlaceholder('Qualification name').fill('Truck Driver Edited');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Truck Driver Edited');
  await page.getByRole('button', { name: 'Edit Qualification' }).click();
  await page.getByPlaceholder('Qualification name').click();
  await page.getByPlaceholder('Qualification name').fill('Truck Driver');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Truck Driver');
});