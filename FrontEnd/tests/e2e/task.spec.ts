import { test, expect } from '@playwright/test';

test('Create Task Category', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Tasks' }).click();
  await page.getByRole('link', { name: 'add Create Task Category' }).click();
  await page.getByPlaceholder('Category code').click();
  await page.getByPlaceholder('Category code').fill('aaaaaaaa');
  await page.getByPlaceholder('Task category name').click();
  await page.getByPlaceholder('Task category name').fill('bbbbbbb');
  await page.getByPlaceholder('Description of the task').click();
  await page.getByPlaceholder('Description of the task').fill('ccccccc');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search View Task Categories' }).click();
  await expect(page.getByRole('link', { name: 'aaaaaaaa bbbbbbb ccccccc' })).toBeVisible();
});

test('Can\'t create Task Category with invalid code', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Tasks' }).click();
  await page.getByRole('link', { name: 'add Create Task Category' }).click();
  await page.getByPlaceholder('Category code').click();
  await page.getByPlaceholder('Category code').fill('aaaaa´');  
  await page.getByPlaceholder('Task category name').click();
  await page.getByPlaceholder('Task category name').fill('bbb');
  await page.getByPlaceholder('Description of the task').click();
  await page.getByPlaceholder('Description of the task').fill('aaaaaa');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await expect(page.locator('.alert__message')).toBeVisible();
});