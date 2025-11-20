# Testing Guide for Backend and Frontend

This document explains how to run the different types of tests found in this project. That includes Frontend tests and Backend tests, as well as tests that span the whole system.

## Table of Contents
- [Backend Tests](#backend-tests)
- [Frontend Tests](#frontend-tests)
- [System Tests](#system-tests)

---

## Backend Tests

The backend uses **xUnit** for testing with three test levels:

### Test Types

1. **Unit Tests** - Test individual components in isolation
2. **Integration Tests** - Test interactions between components  
3. **Application Tests** - Test complete API endpoints with in-memory database

### Running Backend Tests

#### Run All Tests
```bash
cd BackEnd
dotnet test
```

#### Run Tests with Coverage
```bash
cd BackEnd
dotnet test --collect:"XPlat Code Coverage"
```

#### Run Specific Test Project
```bash
cd BackEnd/Tests
dotnet test
```

#### Run Specific Test Class
```bash
cd BackEnd/Tests
dotnet test --filter "FullyQualifiedName~StaffControllerTest"
```

#### Run Specific Test Method
```bash
cd BackEnd/Tests
dotnet test --filter "FullyQualifiedName~StaffControllerTest.GetAll_ShouldReturnOk_WithListOfStaffs"
```

### Test Structure
```
BackEnd/Tests/
├── Unitary/          # Unit tests for controllers, services, etc.
├── Integration/      # Integration tests for repositories, database interactions
└── Application/      # End-to-end API tests with in-memory database
```

### Test Environment

The backend uses **Testing environment** for tests:
- In-memory database (no real database needed)
- Authentication/Authorization disabled
- Set via `ASPNETCORE_ENVIRONMENT=Testing`

---

## Frontend Tests

The frontend uses **Vitest** for unit/integration tests and **Playwright** for E2E tests.

### Test Types

1. **Unit Tests** - Test individual services, components in isolation (Vitest)
2. **Integration Tests** - Test multiple components working together (Vitest)
3. **E2E Tests** - Test complete user workflows in a browser (Playwright)

### Running Frontend Tests

#### Run All Unit Tests
```bash
cd FrontEnd
npm run test:unitary
```

#### Run All Integration Tests
```bash
cd FrontEnd
npm run test:integration
```

#### Run Integration Tests with UI
```bash
npx playwright test --ui
```

#### Run All E2E Tests
```bash
cd FrontEnd
npm run test:e2e
```

#### Run All Vitest Tests (Unit + Integration)
```bash
cd FrontEnd
npm run test:unit
```

#### Run Tests in Watch Mode (Vitest)
```bash
cd FrontEnd
npm run test:unit -- --watch
```

#### Run Specific Test File (Vitest)
```bash
cd FrontEnd
npm run test:unitary -- tests/unitary/StaffServiceTest.spec.ts
```

#### Run E2E Tests in Headed Mode (see browser)
```bash
cd FrontEnd
npm run test:e2e -- --headed
```

#### Run E2E Tests for Specific Browser
```bash
cd FrontEnd
npm run test:e2e -- --project=chromium
npm run test:e2e -- --project=firefox
npm run test:e2e -- --project=webkit
```

#### Debug E2E Tests
```bash
cd FrontEnd
npm run test:e2e -- --debug
```

#### List All E2E Tests
```bash
cd FrontEnd
npm run test:e2e -- --list
```

### Test Structure
```
FrontEnd/tests/
├── unitary/          # Unit tests (services, utilities)
├── integration/      # Integration tests (multiple components)
└── e2e/              # End-to-end tests (Playwright)
    ├── staff/        # Staff-related E2E tests
    └── test-utils.ts # Shared test utilities
```

### E2E Test Environment

E2E tests run with:
- `VITE_TEST_BYPASS_AUTH=true` - Authentication bypassed automatically
- Mock API responses via route interception
- No real backend needed (tests are fully mocked)

---

## System Tests

System tests verify the entire application stack working together.

### Prerequisites

1. **Backend** running in Testing environment:
```bash
cd BackEnd/Api
dotnet run --launch-profile http-testing
```
2. **Frontend** running on local:
```bash
cd Frontend
npm run local
```

### Running System Tests

System tests are E2E tests that connect to the real backend instead of mocking:
> **IMPORTANT NOTE**: the backend __MUST__ be re-run _after every sequence of "creation" test of an entity_ since the in memory db does not remove inserted data by tests.

```bash
cd FrontEnd
npm run test:e2e
```

For easier visualization of test workflow and **single test execution**, use the following commands:

```bash
cd FrontEnd
npx playwright test --ui
```

### System Test Characteristics

- Real HTTP requests to backend API
- Real in-memory database operations
- Real authentication bypass (Testing environment)
- Tests complete user workflows end-to-end

---

## Quick Reference

### Backend
```bash
cd BackEnd
dotnet test                                    # All tests
dotnet test --filter "FullyQualifiedName~..."  # Specific test
```

### Frontend
```bash
cd FrontEnd
npm run test:unitary      # Unit tests
npm run test:integration  # Integration tests  
npm run test:e2e          # E2E tests
npm run test:e2e -- --ui  # E2E with UI
```

### System (Full Stack)
```bash
# Terminal 1: Backend
cd BackEnd/Api && dotnet run --launch-profile http-testing

# Terminal 2: Frontend
cd Frontend && npm run local

# Terminal 3 ver1: E2E Tests no UI
cd FrontEnd && npm run test:e2e

# Terminal 3 ver2: E2E Tests with UI
cd FrontEnd && npx playwright test --ui
```

---

## CI/CD Considerations

When running tests in CI pipelines:

**Backend:**
```bash
dotnet test --configuration Release --no-build --logger "trunit"
```

**Frontend:**
```bash
CI=true npm run test:unit  # Vitest runs once and exits
CI=true npm run test:e2e   # Playwright runs headless
```

---

## Troubleshooting

### Backend Tests Fail
- Ensure .NET SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`
- Check test output for specific errors

### Frontend Unit/Integration Tests Fail
- Clear node modules: `rm -rf node_modules && npm install`
- Check TypeScript compilation: `npm run type-check`

### E2E Tests Fail
- Install Playwright browsers: `npx playwright install`
- Check if ports 5173 (frontend) are available
- Run in headed mode to see what's happening: `npm run test:e2e -- --headed`

### System Tests Fail
- Ensure backend is running on correct port (5195 for testing)
- Ensure frontend can proxy to backend
- Check browser console for errors in headed mode
