import { test, expect } from '@playwright/test';

test('Ensure can create dock and listing works', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Docks' }).click();
  await page.getByRole('link', { name: 'add Create Dock Register a' }).click();
  await page.getByPlaceholder('Length in meters').click();
  await page.getByPlaceholder('Length in meters').fill('600');
  await page.getByPlaceholder('Depth in meters').click();
  await page.getByPlaceholder('Depth in meters').fill('30');
  await page.getByPlaceholder('Draft in meters').click();
  await page.getByPlaceholder('Draft in meters').fill('20');
  await page.getByPlaceholder('Select vessel types').click();
  await page.locator('.option').first().click();
  await page.getByPlaceholder('Dock code').click();
  await page.getByPlaceholder('Dock code').fill('DCKADDTST');
  await page.getByPlaceholder('Dock name').click();
  await page.getByPlaceholder('Dock name').fill('Test add Dock');
  await page.getByPlaceholder('Dock location').click();
  await page.getByPlaceholder('Dock location').fill('location');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Docks Manage and' }).click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').fill('Test add Dock');
  await expect(page.getByRole('link', { name: 'Test add Dock DCKADDTST' })).toBeVisible();
});

test('Ensure cant create duplicate dock', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Docks' }).click();
  await page.getByRole('link', { name: 'search View Docks Manage and' }).click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').fill('Dock C');
  await expect(page.getByRole('link', { name: 'Dock C DCK003 South Harbor' })).toBeVisible();
  await page.getByRole('link', { name: 'Docks' }).click();
  await page.getByRole('link', { name: 'add Create Dock Register a' }).click();
  await page.getByPlaceholder('Dock code').click();
  await page.getByPlaceholder('Dock code').fill('DCK003');
  await page.getByPlaceholder('Dock code').press('CapsLock');
  await page.getByPlaceholder('Dock name').click();
  await page.getByPlaceholder('Dock name').fill('Dock C');
  await page.getByPlaceholder('Dock location').click();
  await page.getByPlaceholder('Dock location').fill('location');
  await page.getByPlaceholder('Length in meters').click();
  await page.getByPlaceholder('Length in meters').fill('500');
  await page.getByPlaceholder('Depth in meters').click();
  await page.getByPlaceholder('Depth in meters').fill('30');
  await page.getByPlaceholder('Draft in meters').click();
  await page.getByPlaceholder('Draft in meters').fill('20');
  await page.getByPlaceholder('Select vessel types').click();
  await page.locator('.option').first().click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.locator('.alert__message')).toBeVisible();
});

test('Ensure can edit dock', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Docks' }).click();
  await page.getByRole('link', { name: 'search View Docks Manage and' }).click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Dock Dashboard Search Docks' }).getByPlaceholder('Search...').fill('Dock B');
  await page.getByRole('link', { name: 'Dock B DCK002 East Harbor' }).click();
  await page.getByRole('button', { name: 'Edit Dock' }).click();
  await page.getByPlaceholder('Dock name').click();
  await page.getByPlaceholder('Dock name').fill('Dock Edited');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Dock Edited');
  await page.getByRole('button', { name: 'Edit Dock' }).click();
  await page.getByPlaceholder('Dock name').click();
  await page.getByPlaceholder('Dock name').fill('Dock B');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('Dock B');
});

test('Ensure cant create dock with empty fields or invalid data', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Docks' }).click();
  await page.getByRole('link', { name: 'add Create Dock Register a' }).click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Create Dock' })).toBeVisible();
});
