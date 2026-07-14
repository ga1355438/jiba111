import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  timeout: 30000,
  retries: 0,
  use: {
    baseURL: 'http://localhost:5000',
    channel: 'msedge',
    headless: true,
    screenshot: 'only-on-failure',
    trace: 'retain-on-failure',
  },
  webServer: {
    command: 'dotnet run --urls http://localhost:5000',
    cwd: './src/LibrarySeatReservation.Web',
    port: 5000,
    reuseExistingServer: true,
    timeout: 60000,
  },
  projects: [
    { name: 'edge', use: { channel: 'msedge' } },
  ],
});
