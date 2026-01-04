// Jest setup file for application tests
// This runs before any tests, setting environment variables before modules are loaded

process.env.DISABLE_AUTH = 'true';
process.env.RUN_MODE = 'local';
process.env.NODE_ENV = 'test';
