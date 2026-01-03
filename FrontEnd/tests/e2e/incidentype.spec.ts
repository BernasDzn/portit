import { test, expect } from '@playwright/test';

test('Ensure can create and filter incident types', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Incidents' }).click();
  await page.getByRole('link', { name: 'add Create Incident Type' }).click();
  await page.getByPlaceholder('Incident type name').click();
  await page.getByPlaceholder('Incident type name').fill('incddd');
  await page.getByPlaceholder('Incident type name').press('Tab');
  await page.getByPlaceholder('Detailed description of the').fill('incddd');
  await page.locator('.select__combobox').first().click();
  await page.locator('.option__label').first().click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Incident Types' }).click();
  await page.getByRole('main').filter({ hasText: 'Incident Types Dashboard' }).getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Incident Types Dashboard' }).getByPlaceholder('Search...').fill('incddd');
  await page.getByRole('link', { name: 'incddd incddd' }).click();
  await expect(page.getByRole('heading', { name: 'incddd' })).toBeVisible();
});

test('Ensure can update inicident types', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Incidents' }).click();
  await page.getByRole('link', { name: 'add Create Incident Type' }).click();
  await page.getByPlaceholder('Incident type name').click();
  await page.getByPlaceholder('Incident type name').fill('update');
  await page.getByPlaceholder('Detailed description of the').click();
  await page.getByPlaceholder('Detailed description of the').fill('update');
  await page.locator('.select__combobox').first().click();
  await page.locator('.option__label').first().click();
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Incident Types' }).click();
  await page.getByRole('main').getByPlaceholder('Search...').click();
  await page.getByRole('main').filter({ hasText: 'Incident Types Dashboard' }).getByPlaceholder('Search...').fill('update');
  await page.getByRole('link', { name: 'update update' }).click();
  await page.getByRole('button', { name: 'Edit' }).click();
  await page.getByPlaceholder('Incident type name').click();
  await page.getByPlaceholder('Incident type name').fill('updated type');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('updated type');
  await page.getByRole('button', { name: 'Edit' }).click();
  await page.getByPlaceholder('Incident type name').click();
  await page.getByPlaceholder('Incident type name').fill('update');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('heading')).toContainText('update');
});