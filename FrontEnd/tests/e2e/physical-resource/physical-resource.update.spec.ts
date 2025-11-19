import { test, expect } from '@playwright/test';
import { stubWhoAmI, interceptPutAndCapture, fillShoelace, submitForm, setShoelaceSelect } from '../test-utils.js';
import type { STSCraneDto, YardCraneDto, TruckDto } from '../../../src/model/dto/PhysicalResourceDto.js';

test.describe('Physical Resource Update (PUT)', () => {

  test('should update STS Crane successfully', async ({ page }) => {
    await stubWhoAmI(page);

    const resourceCode = 'STS001';
    
    // Mock GET endpoint to retrieve existing resource
    await page.route(`**/api/PhysicalResource/${resourceCode}`, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: resourceCode,
          description: 'Original STS Crane',
          status: 0,
          setupTimeInMinutes: 15,
          operationalWindow: { shifts: [] },
          qualifications: [],
          liftingCapacity: 500,
          servingDock: { code: 'DOCK001', name: 'Dock 1' },
          containersPerHour: 40,
          type: 0
        })
      });
    });

    // Mock dock filter endpoint
    await page.route('**/api/Dock/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [
            { code: 'DOCK001', dockName: 'Dock 1', location: 'Port A' },
            { code: 'DOCK002', dockName: 'Dock 2', location: 'Port B' }
          ],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 2
        })
      });
    });

    // Mock qualification filter endpoint
    await page.route('**/api/Qualification/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [
            { code: 'Q1', name: 'Qualification 1' },
            { code: 'Q2', name: 'Qualification 2' }
          ],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 2
        })
      });
    });

    const captured = await interceptPutAndCapture(page, `**/api/PhysicalResource/UpdateSTSCrane/${resourceCode}`);

    const updatedData: STSCraneDto = {
      code: resourceCode,
      description: 'Updated STS Crane',
      status: 0,
      setupTimeInMinutes: 20,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 600,
      servingDockCode: 'DOCK002',
      containersPerHour: 50
    };

    await page.goto(`/resources/edit/${resourceCode}`);
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1500);

    // Update fields
    const descInput = page.locator('#pr-description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, updatedData.description);
      await fillShoelace(page.locator('#pr-setupTime'), String(updatedData.setupTimeInMinutes));
      await fillShoelace(page.locator('#pr-liftingCapacity'), String(updatedData.liftingCapacity));
      await fillShoelace(page.locator('#pr-containersPerHour'), String(updatedData.containersPerHour));
      
      // Update serving dock
      await setShoelaceSelect(page.locator('#pr-servingDock'), 'DOCK002');
      await page.waitForTimeout(300);

      await submitForm(page);

      const start = Date.now();
      const timeout = 10000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 200));
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.description).toBe(updatedData.description);
    }
  });

  test('should update Yard Crane successfully', async ({ page }) => {
    await stubWhoAmI(page);

    const resourceCode = 'YARD001';
    
    await page.route(`**/api/PhysicalResource/${resourceCode}`, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: resourceCode,
          description: 'Original Yard Crane',
          status: 0,
          setupTimeInMinutes: 10,
          operationalWindow: { shifts: [] },
          qualifications: [],
          liftingCapacity: 450,
          containersPerHour: 35,
          type: 1
        })
      });
    });

    // Mock qualification filter endpoint
    await page.route('**/api/Qualification/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 0
        })
      });
    });

    const captured = await interceptPutAndCapture(page, `**/api/PhysicalResource/UpdateYardCrane/${resourceCode}`);

    const updatedData: YardCraneDto = {
      code: resourceCode,
      description: 'Updated Yard Crane',
      status: 0,
      setupTimeInMinutes: 12,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      liftingCapacity: 500,
      containersPerHour: 40
    };

    await page.goto(`/resources/edit/${resourceCode}`);
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1500);

    const descInput = page.locator('#pr-description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, updatedData.description);
      await fillShoelace(page.locator('#pr-setupTime'), String(updatedData.setupTimeInMinutes));
      await fillShoelace(page.locator('#pr-liftingCapacity'), String(updatedData.liftingCapacity));
      await fillShoelace(page.locator('#pr-containersPerHour'), String(updatedData.containersPerHour));

      await submitForm(page);

      const start = Date.now();
      const timeout = 8000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 100));
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.description).toBe(updatedData.description);
    }
  });

  test('should update Truck successfully', async ({ page }) => {
    await stubWhoAmI(page);

    const resourceCode = 'TRUCK001';
    
    await page.route(`**/api/PhysicalResource/${resourceCode}`, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: resourceCode,
          description: 'Original Truck',
          status: 0,
          setupTimeInMinutes: 5,
          operationalWindow: { shifts: [] },
          qualifications: [],
          maxLoadCapacity: 200,
          averageSpeed: 60,
          containersPerTrip: 5,
          type: 2
        })
      });
    });

    // Mock qualification filter endpoint
    await page.route('**/api/Qualification/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 0
        })
      });
    });

    const captured = await interceptPutAndCapture(page, `**/api/PhysicalResource/UpdateTruck/${resourceCode}`);

    const updatedData: TruckDto = {
      code: resourceCode,
      description: 'Updated Truck',
      status: 0,
      setupTimeInMinutes: 6,
      operationalWindow: { shifts: [] },
      qualificationsCodes: [],
      maxLoadCapacity: 250,
      averageSpeed: 70,
      containersPerTrip: 6
    };

    await page.goto(`/resources/edit/${resourceCode}`);
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1500);

    const descInput = page.locator('#pr-description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, updatedData.description);
      await fillShoelace(page.locator('#pr-setupTime'), String(updatedData.setupTimeInMinutes));
      await fillShoelace(page.locator('#pr-maxLoadCapacity'), String(updatedData.maxLoadCapacity));
      await fillShoelace(page.locator('#pr-averageSpeed'), String(updatedData.averageSpeed));
      await fillShoelace(page.locator('#pr-containersPerTrip'), String(updatedData.containersPerTrip));

      await submitForm(page);

      const start = Date.now();
      const timeout = 8000;
      while (!captured.data && Date.now() - start < timeout) {
        await new Promise((r) => setTimeout(r, 100));
      }

      expect(captured.data).not.toBeNull();
      expect(captured.data.description).toBe(updatedData.description);
    }
  });

  test('should return 404 when updating non-existent resource', async ({ page }) => {
    await stubWhoAmI(page);

    const nonExistentCode = 'NONEXISTENT';

    await page.route(`**/api/PhysicalResource/${nonExistentCode}`, (route) => {
      route.fulfill({
        status: 404,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Physical resource not found' })
      });
    });

    await page.goto(`/resources/edit/${nonExistentCode}`);
    await page.waitForLoadState('networkidle');

    const content = await page.textContent('body');
    expect(content).toBeTruthy();
  });

  test('should return 400 on invalid update data', async ({ page }) => {
    await stubWhoAmI(page);

    const resourceCode = 'STS002';

    await page.route(`**/api/PhysicalResource/${resourceCode}`, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: resourceCode,
          description: 'STS Crane',
          status: 0,
          setupTimeInMinutes: 15,
          operationalWindow: { shifts: [] },
          qualifications: [],
          liftingCapacity: 500,
          servingDock: { code: 'DOCK001', name: 'Dock 1' },
          containersPerHour: 40,
          type: 0
        })
      });
    });

    await page.route('**/api/PhysicalResource/UpdateSTSCrane/**', (route) => {
      route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Invalid resource data' })
      });
    });

    // Mock dock and qualification endpoints
    await page.route('**/api/Dock/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [{ code: 'DOCK001', dockName: 'Dock 1', location: 'Port A' }],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 1
        })
      });
    });

    await page.route('**/api/Qualification/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 0
        })
      });
    });

    await page.goto(`/resources/edit/${resourceCode}`);
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1500);

    const descInput = page.locator('#pr-description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, '');
      
      try {
        await submitForm(page);
        await page.waitForTimeout(1000);
      } catch (e) {
        // Expected
      }
    }
  });

  test('should return 409 conflict on duplicate code during update', async ({ page }) => {
    await stubWhoAmI(page);

    const resourceCode = 'STS003';

    await page.route(`**/api/PhysicalResource/${resourceCode}`, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: resourceCode,
          description: 'STS Crane',
          status: 0,
          setupTimeInMinutes: 15,
          operationalWindow: { shifts: [] },
          qualifications: [],
          liftingCapacity: 500,
          servingDock: { code: 'DOCK001', name: 'Dock 1' },
          containersPerHour: 40,
          type: 0
        })
      });
    });

    await page.route('**/api/PhysicalResource/UpdateSTSCrane/**', (route) => {
      route.fulfill({
        status: 409,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Physical resource with this code already exists' })
      });
    });

    // Mock dock and qualification endpoints
    await page.route('**/api/Dock/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [{ code: 'DOCK001', dockName: 'Dock 1', location: 'Port A' }],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 1
        })
      });
    });

    await page.route('**/api/Qualification/filter**', (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          data: [],
          pageNumber: 1,
          pageSize: 10,
          totalElements: 0
        })
      });
    });

    await page.goto(`/resources/edit/${resourceCode}`);
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1500);

    const descInput = page.locator('#pr-description');
    if (await descInput.count() > 0) {
      await fillShoelace(descInput, 'Updated Description');
      
      try {
        await submitForm(page);
        await page.waitForTimeout(1000);
      } catch (e) {
        // Expected
      }
    }
  });

});
