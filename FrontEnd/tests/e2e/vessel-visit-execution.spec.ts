import { test, expect } from '@playwright/test';

test('Start VVE', async ({ page }) => {
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
  await page.getByRole('link', { name: 'Operation Plan - 2026-PORTO-' }).click();
  await page.getByRole('button', { name: 'Start Execution' }).click();
  await page.getByRole('link', { name: 'Vessel Visit Executions' }).click();
  await page.getByRole('link', { name: 'search Search Executions' }).click();
  await page.getByRole('link', { name: 'View Execution' }).click();
  await expect(page.getByRole('heading', { name: 'VVE-PORTO-' })).toBeVisible();
});

test('Update VVE with Berth Time and Dock', async ({ page }) => {
  await page.goto('http://localhost:5173/vessel-visit-executions/2026-PORTO-000001');
  await page.getByRole('button', { name: 'Edit berth data' }).click();
  await page.locator('.select__combobox').click();
  await page.getByRole('option', { name: 'Dock C' }).press('Enter');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByText('Berth DockDCK003')).toBeVisible();
  await expect(page.getByText('Berth Time', { exact: true })).toBeVisible();
});

test('Update VVE Operations', async ({ page }) => {
  await page.goto('http://localhost:5173/vessel-visit-executions/2026-PORTO-000001');
  await expect(page.getByRole('heading', { name: 'VVE-PORTO-' })).toBeVisible();
  await page.getByRole('button', { name: 'Update Execution' }).click();
  await page.getByRole('button', { name: 'Start' }).first().click();
  await page.getByRole('button', { name: 'Start' }).nth(2).click();
  await expect(page.getByText('Started', { exact: true })).toBeVisible();
  await page.getByRole('button', { name: 'Complete' }).click();
  await page.getByRole('button', { name: 'Complete' }).nth(1).click();
  await expect(page.getByText('Completed', { exact: true })).toBeVisible();
});