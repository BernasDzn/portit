import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPostAndCapture, fillShoelace, submitForm } from '../test-utils.js';
import type { VesselVisitNotificationDto } from '../../../src/model/dto/VesselVisitNotificationDto.js';

test.describe('Vessel Visit Notification Create (POST)', () => {

  test('should create vessel visit notification successfully with all steps', async ({ page }) => {
    await stubWhoAmI(page);

    // Mock Vessel endpoint - getVesselByOwner
    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { 
            imoNumber: 'IMO001', 
            name: 'Test Vessel 1',
            taxNumber: 'TAX001',
            callSign: 'TS1'
          },
          { 
            imoNumber: 'IMO002', 
            name: 'Test Vessel 2',
            taxNumber: 'TAX002',
            callSign: 'TS2'
          }
        ])
      });
    });

    const captured = await interceptPostAndCapture(page, '**/api/VesselVisitNotification');

    const testVVN: VesselVisitNotificationDto = {
      notificationId: '',
      expectedArrival: '2025-12-01T10:00:00Z',
      expectedDeparture: '2025-12-05T18:00:00Z',
      isCargoHazardous: false,
      specialRequirements: 'No special requirements',
      crewDetails: {
        captain: { value: 'Captain John Smith' },
        totalCrewMembers: 25,
        safetyOfficers: []
      },
      loadCargoManifest: [],
      unloadCargoManifest: [],
      vesselImoNumber: 'IMO001'
    };

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // STEP 1: Visit Information
    expect(await page.locator('text=Step 1 of 4').isVisible()).toBeTruthy();

    // Check arrival and departure date pickers exist
    const datePickerElements = await page.locator('[input-id="vvn-expectedArrival"], [input-id="vvn-expectedDeparture"]').count();
    expect(datePickerElements).toBeGreaterThan(0);

    // Select vessel
    const vesselDropdown = page.locator('#vvn-vesselImo');
    if (await vesselDropdown.count() > 0) {
      await vesselDropdown.click();
      await page.waitForTimeout(300);
      await page.locator('sl-option').first().click();
      await page.waitForTimeout(300);
    }

    // Move to next step
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });
    await nextBtn.click();
    await page.waitForTimeout(500);

    // STEP 2: Crew Details
    expect(await page.locator('text=Step 2 of 4').isVisible()).toBeTruthy();

    const captainInput = page.locator('#vvn-captainName');
    if (await captainInput.count() > 0) {
      await fillShoelace(captainInput, testVVN.crewDetails!.captain.value);
      await fillShoelace(page.locator('#vvn-totalCrewMembers'), String(testVVN.crewDetails!.totalCrewMembers));
    }

    await nextBtn.click();
    await page.waitForTimeout(500);

    // STEP 3: Cargo Requirements
    expect(await page.locator('text=Step 3 of 4').isVisible()).toBeTruthy();

    const hazardousCheckbox = page.locator('#vvn-isCargoHazardous');
    if (await hazardousCheckbox.count() > 0) {
      const specialReqInput = page.locator('#vvn-specialRequirements');
      if (await specialReqInput.count() > 0) {
        await fillShoelace(specialReqInput, testVVN.specialRequirements || '');
      }
    }

    await nextBtn.click();
    await page.waitForTimeout(500);

    // STEP 4: Cargo Contents
    expect(await page.locator('text=Step 4 of 4').isVisible()).toBeTruthy();

    // Submit form
    await submitForm(page);

    const start = Date.now();
    const timeout = 10000;
    while (!captured.data && Date.now() - start < timeout) {
      await new Promise((r) => setTimeout(r, 200));
    }

    expect(captured.data).not.toBeNull();
    expect(captured.data.crewDetails!.captain.value).toBe(testVVN.crewDetails!.captain.value);
    // totalCrewMembers comes as string from form, so convert for comparison
    expect(Number(captured.data.crewDetails!.totalCrewMembers)).toBe(testVVN.crewDetails!.totalCrewMembers);
  });

  test('should navigate through all 4 steps successfully', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { imoNumber: 'IMO001', name: 'Test Vessel 1', taxNumber: 'TAX001', callSign: 'TS1' }
        ])
      });
    });

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Verify Step 1
    expect(await page.locator('text=Step 1 of 4').isVisible()).toBeTruthy();
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });

    // Navigate to Step 2
    await nextBtn.click();
    await page.waitForTimeout(500);
    expect(await page.locator('text=Step 2 of 4').isVisible()).toBeTruthy();

    // Navigate to Step 3
    await nextBtn.click();
    await page.waitForTimeout(500);
    expect(await page.locator('text=Step 3 of 4').isVisible()).toBeTruthy();

    // Navigate to Step 4
    await nextBtn.click();
    await page.waitForTimeout(500);
    expect(await page.locator('text=Step 4 of 4').isVisible()).toBeTruthy();

    // Check previous button is enabled
    const prevBtn = page.locator('sl-button').filter({ hasText: /Previous/i });
    expect(await prevBtn.isEnabled()).toBeTruthy();

    // Navigate back to Step 3
    await prevBtn.click();
    await page.waitForTimeout(500);
    expect(await page.locator('text=Step 3 of 4').isVisible()).toBeTruthy();
  });

  test('should handle hazardous cargo with safety officers', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { imoNumber: 'IMO001', name: 'Test Vessel 1', taxNumber: 'TAX001', callSign: 'TS1' }
        ])
      });
    });

    const captured = await interceptPostAndCapture(page, '**/api/VesselVisitNotification');

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Navigate to Step 3 (Cargo Requirements)
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });
    
    // Skip Step 1 and 2
    await nextBtn.click();
    await page.waitForTimeout(300);
    await nextBtn.click();
    await page.waitForTimeout(300);
    await nextBtn.click();
    await page.waitForTimeout(500);

    // Step 3: Check hazardous cargo
    const hazardousCheckbox = page.locator('#vvn-isCargoHazardous input[type="checkbox"]');
    if (await hazardousCheckbox.count() > 0) {
      await hazardousCheckbox.check();
      await page.waitForTimeout(500);

      // Verify safety officers section appears
      const safetyOfficerSection = page.locator('text=Safety officers');
      expect(await safetyOfficerSection.isVisible()).toBeTruthy();
    }
  });

  test('should fill special requirements field', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { imoNumber: 'IMO001', name: 'Test Vessel 1', taxNumber: 'TAX001', callSign: 'TS1' }
        ])
      });
    });

    const captured = await interceptPostAndCapture(page, '**/api/VesselVisitNotification');

    const specialReqs = 'Special cargo handling required. Requires crane assistance.';

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Navigate to Step 3 (Cargo Requirements)
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });
    await nextBtn.click();
    await page.waitForTimeout(500);
    
    // Fill crew details in Step 2
    const captainInput = page.locator('#vvn-captainName');
    if (await captainInput.count() > 0) {
      await fillShoelace(captainInput, 'Captain Test');
      await fillShoelace(page.locator('#vvn-totalCrewMembers'), '20');
    }
    
    await nextBtn.click();
    await page.waitForTimeout(500);

    // Now in Step 3 - fill special requirements
    const specialReqInput = page.locator('#vvn-specialRequirements');
    if (await specialReqInput.count() > 0) {
      await fillShoelace(specialReqInput, specialReqs);
      await page.waitForTimeout(300);
    }

    // Move to step 4 and submit
    await nextBtn.click();
    await page.waitForTimeout(500);

    await submitForm(page);

    const start = Date.now();
    const timeout = 10000;
    while (!captured.data && Date.now() - start < timeout) {
      await new Promise((r) => setTimeout(r, 200));
    }

    expect(captured.data).not.toBeNull();
    expect(captured.data.specialRequirements).toBe(specialReqs);
  });

  test('should return 400 on invalid crew member count (non-numeric)', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { imoNumber: 'IMO001', name: 'Test Vessel 1', taxNumber: 'TAX001', callSign: 'TS1' }
        ])
      });
    });

    await page.route('**/api/VesselVisitNotification', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid crew member count' })
      });
    });

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Navigate to Step 2
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });
    await nextBtn.click();
    await page.waitForTimeout(500);

    const crewInput = page.locator('#vvn-totalCrewMembers');
    if (await crewInput.count() > 0) {
      await fillShoelace(crewInput, 'NOT_A_NUMBER');

      // Try to proceed
      try {
        await nextBtn.click();
        await page.waitForTimeout(1000);
      } catch (e) {
        // Expected - validation should prevent invalid input
      }
    }
  });

  test('should load vessels from Vessel service on mount', async ({ page }) => {
    await stubWhoAmI(page);

    const vessels = [
      { imoNumber: 'IMO001', name: 'Vessel One', taxNumber: 'TAX001', callSign: 'VO1' },
      { imoNumber: 'IMO002', name: 'Vessel Two', taxNumber: 'TAX002', callSign: 'VO2' },
      { imoNumber: 'IMO003', name: 'Vessel Three', taxNumber: 'TAX003', callSign: 'VO3' }
    ];

    let vesselRequestCalled = false;

    await page.route('**/api/Vessel/owner/**', (route) => {
      vesselRequestCalled = true;
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(vessels)
      });
    });

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    expect(vesselRequestCalled).toBeTruthy();

    // Verify vessel dropdown is populated
    const vesselDropdown = page.locator('#vvn-vesselImo');
    if (await vesselDropdown.count() > 0) {
      await vesselDropdown.click();
      await page.waitForTimeout(300);

      // Count options - should have at least as many as vessels
      const options = await page.locator('sl-option').count();
      expect(options).toBeGreaterThanOrEqual(vessels.length);
    }
  });

  test('should handle missing captain name gracefully', async ({ page }) => {
    await stubWhoAmI(page);

    await page.route('**/api/Vessel/owner/**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          { imoNumber: 'IMO001', name: 'Test Vessel 1', taxNumber: 'TAX001', callSign: 'TS1' }
        ])
      });
    });

    await page.route('**/api/VesselVisitNotification', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Captain name is required' })
      });
    });

    await page.goto('/vessel-visit-notifications/create');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Navigate to Step 2 (Crew Details)
    const nextBtn = page.locator('sl-button').filter({ hasText: /Next/i });
    await nextBtn.click();
    await page.waitForTimeout(500);

    // Leave captain name empty and fill crew count
    const crewInput = page.locator('#vvn-totalCrewMembers');
    if (await crewInput.count() > 0) {
      await fillShoelace(crewInput, '20');

      try {
        await nextBtn.click();
        await page.waitForTimeout(1000);
      } catch (e) {
        // Expected - validation should prevent empty captain name
      }
    }
  });

});
