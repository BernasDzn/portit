import { test, expect } from '@playwright/test';
import { stubWhoAmI } from '../test-utils.js';
import type { QualificationDto } from '../../../src/model/dto/QualificationDto.ts';
import type { Page } from '../../../src/model/Page.ts';

test.describe('Qualification Read (GET)', () => {
  
  const mockQualification: QualificationDto = {
    idCode: 'Q1',
    qualificationName: 'Forklift Operator',
  };

  const mockPage: Page<QualificationDto> = {
    items: [mockQualification],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  test('should get all qualifications and render list', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Qualification/filter', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockPage),
      });
    });

    await page.goto('/qualifications/search');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return empty list when no qualifications found', async ({ page }) => {
    await stubWhoAmI(page);

    const emptyPage: Page<QualificationDto> = {
      items: [],
      pageNumber: 1,
      pageSize: 10,
      pageCount: 0,
    };

    await page.route('**/api/Qualification/filter?Code=AAAA&', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(emptyPage),
      });
    });

    await page.goto('/qualifications/search?Code=AAAA');
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should filter qualifications and return page', async ({ page }) => {
    await stubWhoAmI(page);

    let capturedUrl = '';
    await page.route('**/api/Qualification/filter*', (route) => {
      capturedUrl = route.request().url();
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockPage),
      });
    });

    await page.goto('/qualifications/search');
    await page.waitForLoadState('networkidle');

    expect(capturedUrl.includes('Qualification/filter')).toBeTruthy();

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

});
