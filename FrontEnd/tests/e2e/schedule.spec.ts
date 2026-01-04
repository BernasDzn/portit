import { test, expect } from '@playwright/test';

test('Create schedule request', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Schedule' }).click();
  await page.getByRole('link', { name: 'calendar_month Request Task' }).click();
  await page.locator('div').filter({ hasText: /^4$/ }).first().click();
  await page.getByRole('button', { name: 'Generate tasks Schedule' }).click();
  await page.getByRole('button', { name: 'Generate', exact: true }).click();
  await page.waitForTimeout(7500);
  await page.reload();
  await expect(page.getByRole('cell').filter({ hasText: 'completed' })).toBeVisible();
});