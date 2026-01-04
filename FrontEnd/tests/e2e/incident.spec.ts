import { test, expect } from '@playwright/test';

test('Create incident type and incident', async ({ page }) => {
  await page.goto('http://localhost:5173/incident-types/dashboard');
  //create incident type
  await page.getByRole('link', { name: 'add Create Incident Type' }).click();
  await page.getByPlaceholder('Incident type name').click();
  await page.getByPlaceholder('Incident type name').fill('Crane failure');
  await page.getByPlaceholder('Detailed description of the').click();
  await page.getByPlaceholder('Detailed description of the').fill('A crane failed');
  await page.locator('.select__combobox').first().click();
  await page.getByRole('option', { name: 'Minor' }).press('ArrowDown');
  await page.getByRole('option', { name: 'Major' }).press('Enter');
  await page.getByPlaceholder('Select severity level').press('Tab');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  //create incident
  await page.getByRole('link', { name: 'add Create Incident Report a' }).click();
  await page.locator('.select__combobox').first().click();
  await page.getByRole('option', { name: 'Crane failure' }).press('Enter');
  await page.getByPlaceholder('When did the incident start?').click();
  await page.getByPlaceholder('When did the incident start?').press('ArrowRight');
  await page.getByPlaceholder('When did the incident start?').fill('2026-01-04T00:00');
  await page.getByPlaceholder('Describe the incident in').click();
  await page.getByPlaceholder('Describe the incident in').fill('the crane sts 1 failed');
  await page.locator('.form-field.field-dropdown > div > .entity-dropdown > .form-control > .form-control-input > .select > .select__combobox').click();
  await page.getByRole('option', { name: 'Minor' }).press('ArrowDown');
  await page.getByRole('option', { name: 'Major' }).press('Enter');
  await page.getByRole('button', { name: 'Create', exact: true }).click();
  await page.getByRole('link', { name: 'search Search Incidents' }).click();
  await expect(page.getByRole('link', { name: 'the crane sts 1 failed' })).toBeVisible();
});

test('View incident details, edit and close', async ({ page }) => {
  await page.goto('http://localhost:5173/incident-types/dashboard');
  await page.getByRole('link', { name: 'search Search Incidents' }).click();
  await page.getByRole('link', { name: 'the crane sts 1 failed' }).click();
  await page.getByRole('button', { name: 'Edit Incident' }).click();
  await page.locator('.form-field.field-dropdown > div > .entity-dropdown > .form-control > .form-control-input > .select > .select__combobox').first().click();
  await page.getByRole('option', { name: 'Major' }).press('ArrowDown');
  await page.getByRole('option', { name: 'Critical' }).press('Enter');
  await page.getByRole('button', { name: 'Save' }).click();
  await expect(page.locator('sl-badge').filter({ hasText: 'Critical' }).getByRole('status')).toBeVisible();
  await page.getByRole('button', { name: 'Resolve Incident' }).click();
  await page.getByRole('button', { name: 'Yes, close this incident' }).click();
  await expect(page.getByText('Resolved')).toBeVisible();
});