import type { Page, Locator } from '@playwright/test';

type TestUser = {
  sub: string;
  name: string;
  email: string;
  role: string;
  picture?: string;
};

const defaultTestUser: TestUser = {
  sub: 'test-user',
  name: 'Playwright Test',
  email: 'test@example.com',
  role: 'Administrator',
  picture: '',
};

export async function stubWhoAmI(page: Page, user: TestUser = defaultTestUser) {
  await page.route('**/api/auth/me', (route) =>
    route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(user) })
  );
}

export async function interceptPostAndCapture(page: Page, urlPattern = '**/api/Staff') {
  const capturedRequest: { data: any } = { data: null };

  await page.route(urlPattern, async (route) => {
    const request = route.request();
    const raw = request.postData() ?? '';
    try {
      capturedRequest.data = raw ? JSON.parse(raw) : {};
    } catch (e) {
      capturedRequest.data = {};
    }

    const responseBody = JSON.stringify({ ...capturedRequest.data, mechanographicNumber: 'STF000000' });
    await route.fulfill({ status: 201, contentType: 'application/json', body: responseBody });
  });

  return capturedRequest;
}

export async function interceptPutAndCapture(page: Page, urlPattern = '**/api/Staff/**') {
  const capturedRequest: { data: any } = { data: null };

  await page.route(urlPattern, async (route) => {
    const request = route.request();
    const raw = request.postData() ?? '';
    try {
      capturedRequest.data = raw ? JSON.parse(raw) : {};
    } catch (e) {
      capturedRequest.data = {};
    }

    const responseBody = JSON.stringify(capturedRequest.data);
    await route.fulfill({ status: 200, contentType: 'application/json', body: responseBody });
  });

  return capturedRequest;
}

export async function fillShoelace(elementLocator: Locator, text: string) {
  await elementLocator.waitFor({ state: 'attached' });
  await elementLocator.evaluate((element: any, value: string) => {
    const innerInput = (element.shadowRoot && element.shadowRoot.querySelector('input')) || element.querySelector('input');
    if (innerInput) {
      innerInput.value = value;
      innerInput.dispatchEvent(new InputEvent('input', { bubbles: true, composed: true }));
    }
    element.value = value;
    element.dispatchEvent(new CustomEvent('sl-input', { detail: { value } }));
  }, text);
}

export async function setShoelaceSelect(elementLocator: Locator, value: string) {
  await elementLocator.waitFor({ state: 'attached' });
  await elementLocator.evaluate((element: any, val: string) => {
    element.value = val;
    element.dispatchEvent(new CustomEvent('sl-change', { detail: { value: val }, bubbles: true, composed: true }));
  }, value);
}

export async function submitForm(page: Page) {
  // Find all forms and submit only the visible one
  const forms = await page.locator('form').all();
  for (const form of forms) {
    const isVisible = await form.isVisible();
    if (isVisible) {
      await form.evaluate((formEl: HTMLFormElement) => {
        if ((formEl as any).requestSubmit) (formEl as any).requestSubmit();
        else formEl.submit();
      });
      return;
    }
  }
  // Fallback if no visible form found
  await page.locator('form').first().evaluate((form: HTMLFormElement) => {
    if ((form as any).requestSubmit) (form as any).requestSubmit();
    else form.submit();
  });
}
