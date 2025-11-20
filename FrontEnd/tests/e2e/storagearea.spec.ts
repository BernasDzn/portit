import { test, expect } from '@playwright/test';

test('Ensure storage area is created with valid data and is listed', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Storage Areas' }).click();
  await page.getByRole('link', { name: 'add Create Storage Area' }).click();
  await page.getByPlaceholder('Storage Area name code').click();
  await page.getByPlaceholder('Storage Area name code').fill('TESTE');
  await page.getByPlaceholder('Storage Area location').click();
  await page.getByPlaceholder('Storage Area location').fill('Test Location');
  await page.locator('.select__combobox').first().click();
  await page.locator('.option').first().click();
  await page.getByPlaceholder('storageArea.capacity.').click();
  await page.getByPlaceholder('storageArea.capacity.').fill('1000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Storage Areas' }).click();
  await expect(page.getByRole('link', { name: 'TESTE Test Location 0 docks' })).toBeVisible();
});

test('Ensure cant create duplicate storage area', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Storage Areas' }).click();
  await page.getByRole('link', { name: 'add Create Storage Area' }).click();
  await page.getByPlaceholder('Storage Area name code').click();
  await page.getByPlaceholder('Storage Area name code').fill('TestDuplicate');
  await page.getByPlaceholder('Storage Area location').click();
  await page.getByPlaceholder('Storage Area location').fill('location');
  await page.getByPlaceholder('Select Storage Area type').click();
  await page.locator('.option').first().click();
  await page.getByPlaceholder('storageArea.capacity.').click();
  await page.getByPlaceholder('storageArea.capacity.').fill('1000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'add Create Storage Area' }).click();
  await page.getByPlaceholder('Storage Area name code').click();
  await page.getByPlaceholder('Storage Area name code').fill('TestDuplicate');
  await page.getByPlaceholder('Storage Area location').click();
  await page.getByPlaceholder('Storage Area location').fill('location');
  await page.getByPlaceholder('storageArea.capacity.').click();
  await page.getByPlaceholder('storageArea.capacity.').fill('1000');
  await page.getByPlaceholder('Select Storage Area type').click();
  await page.locator('.option').first().click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.locator('.alert.alert--closable.alert--has-countdown.alert--has-icon.alert--danger > .alert__message')).toBeVisible();
});

test('Ensure validation prevents creating storage area with empty fields or incorrect data', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Storage Areas' }).click();
  await page.getByRole('link', { name: 'add Create Storage Area' }).click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Create Storage Area' })).toBeVisible();
});

test('Ensure update storage area changes fields correctly', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Storage Areas' }).click();
  await page.getByRole('link', { name: 'add Create Storage Area' }).click();
  await page.getByPlaceholder('Storage Area name code').click();
  await page.getByPlaceholder('Storage Area name code').fill('SAUPDATETST');
  await page.getByPlaceholder('Storage Area location').click();
  await page.getByPlaceholder('Storage Area location').press('CapsLock');
  await page.getByPlaceholder('Storage Area location').fill('location');
  await page.getByPlaceholder('Select Storage Area type').click();
  await page.locator('.option').first().click();
  await page.getByPlaceholder('storageArea.capacity.').click();
  await page.getByPlaceholder('storageArea.capacity.').fill('1000');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Storage Areas' }).click();
  await page.getByRole('main').filter({ hasText: 'Storage Area Dashboard Search' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Storage Area Dashboard Search' }).getByPlaceholder('Search...').fill('SAUPDATETST');
  await page.getByRole('link', { name: 'SAUPDATETST location 0 docks' }).click();
  await page.getByRole('button', { name: 'Edit Storage Area' }).click();
  await page.getByPlaceholder('storageArea.currentOccupancy.placeholder').click();
  await page.getByPlaceholder('storageArea.currentOccupancy.placeholder').fill('500');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('main')).toContainText('500 / 1000');
});