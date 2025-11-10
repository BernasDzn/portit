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
  await page.route('**/api/Login/me', (route) =>
    route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(user) })
  );
}

export async function interceptPostAndCapture(page: Page, urlPattern = '**/api/Staff') {
  const capturedRequest: { data: any } = { data: null };

  await page.route(urlPattern, async (route, request) => {
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

export async function submitForm(page: Page) {
  await page.locator('form').evaluate((form: HTMLFormElement) => {
    if ((form as any).requestSubmit) (form as any).requestSubmit();
    else form.submit();
  });
}
