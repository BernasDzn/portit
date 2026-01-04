import { test, expect } from '@playwright/test';

test('Generate operation plan', async ({ page }) => {
await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Schedule' }).click();
  await page.getByRole('link', { name: 'calendar_month Request Task' }).click();
  await page.locator('div').filter({ hasText: /^4$/ }).first().click();
  await page.getByRole('button', { name: 'Generate tasks Schedule' }).click();
  await page.getByRole('button', { name: 'Generate', exact: true }).click();
  await page.waitForTimeout(5000);
  await page.reload();
  await page.locator('.bi.bi-check').first().click();
  await page.getByRole('link', { name: 'Scheduling Dashboard' }).click();
  await page.getByRole('link', { name: 'search List Operation Plans' }).click();
  await expect(page.getByRole('link', { name: 'Operation Plan - 2026-PORTO-' })).toBeVisible();
});

test('Edit operation plan', async ({ page }) => {
  await page.goto('http://localhost:5173/');
  await page.getByRole('link', { name: 'Schedule' }).click();
  await page.getByRole('link', { name: 'calendar_month Request Task' }).click();
  await page.locator('div').filter({ hasText: /^4$/ }).first().click();
  await page.getByRole('button', { name: 'Generate tasks Schedule' }).click();
  await page.getByRole('button', { name: 'Generate', exact: true }).click();
  await page.waitForTimeout(5000);
  await page.reload();
  await page.locator('.bi.bi-check').first().click();
  await page.getByRole('link', { name: 'Scheduling Dashboard' }).click();
  await page.getByRole('link', { name: 'search List Operation Plans' }).click();
  await expect(page.getByRole('link', { name: 'Operation Plan - 2026-PORTO-' })).toBeVisible();
  await page.getByRole('link', { name: 'Operation Plan - 2026-PORTO-' }).click();
  await page.getByRole('button', { name: 'Edit Plan' }).click();
  await page.locator('div').filter({ hasText: /^Op\. #1$/ }).nth(1).click();
  await page.locator('.select__combobox').click();
  await page.getByRole('option', { name: 'Maria Silva' }).press('Enter');
  await page.getByRole('button', { name: 'Add' }).click();
  await page.getByRole('button', { name: 'Close' }).nth(1).click();
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByText('carlos.santos@oceanicport.com')).toBeVisible();
});