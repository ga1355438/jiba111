import { test, expect } from '@playwright/test';

test.describe('用户端主链路', () => {
  test('TC-01 首页加载并显示用户信息', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toContainText('图书馆座位预约');
    await expect(page.locator('p.text-muted').first()).toContainText('当前用户');
    await expect(page.locator('p.text-muted').first()).toContainText('今天');
  });

  test('TC-02 查看座位列表', async ({ page }) => {
    await page.goto('/Seat/List');
    await expect(page.locator('h2')).toContainText('座位列表');
    await expect(page.locator('table tbody tr').first()).toBeVisible();
  });

  test('TC-03 查看座位详情', async ({ page }) => {
    await page.goto('/Seat/Detail/1');
    await expect(page.locator('h5')).toContainText('A-01');
    await expect(page.locator('text=预约此时段').first()).toBeVisible();
  });

  test('TC-04 提交预约', async ({ page }) => {
    let booked = false;
    for (const seatId of [16, 17, 18, 19, 1, 2, 3, 4]) {
      await page.goto(`/Seat/Detail/${seatId}`);
      const bookBtn = page.locator('text=预约此时段').first();
      if (await bookBtn.isVisible({ timeout: 2000 }).catch(() => false)) {
        await bookBtn.click();
        await expect(page).toHaveURL(/Reservation\/Create/);
        await page.click('button:has-text("提交预约")');
        await expect(page).toHaveURL(/Reservation\/My/);
        booked = true;
        break;
      }
    }
    expect(booked).toBeTruthy();
  });

  test('TC-05 查看我的预约', async ({ page }) => {
    await page.goto('/Reservation/My');
    await expect(page.locator('h2')).toContainText('我的预约');
    await expect(page.locator('p').filter({ hasText: '当前用户' }).first()).toBeVisible();
  });

  test('TC-06 取消预约', async ({ page }) => {
    await page.goto('/Reservation/My');
    const cancelBtn = page.locator('button:has-text("取消")').first();
    if (await cancelBtn.isVisible()) {
      page.on('dialog', dialog => dialog.accept());
      await cancelBtn.click();
      await expect(page.locator('h2')).toContainText('我的预约');
    }
  });
});

test.describe('管理端主链路', () => {
  test('TC-07 管理员登录成功', async ({ page }) => {
    await page.goto('/Admin/Login');
    await expect(page.locator('h4')).toContainText('管理员登录');
    await page.fill('input[name="username"]', 'admin');
    await page.fill('input[name="password"]', '123456');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL(/Admin/);
  });

  test('TC-08 座位管理列表', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="username"]', 'admin');
    await page.fill('input[name="password"]', '123456');
    await page.click('button[type="submit"]');
    await page.goto('/Admin/SeatIndex');
    await expect(page.locator('h2')).toContainText('座位管理');
    await expect(page.locator('table tbody tr').first()).toBeVisible();
  });

  test('TC-09 预约管理列表', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="username"]', 'admin');
    await page.fill('input[name="password"]', '123456');
    await page.click('button[type="submit"]');
    await page.goto('/Admin/ReservationIndex');
    await expect(page.locator('h2')).toContainText('预约管理');
  });

  test('TC-10 统计页', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="username"]', 'admin');
    await page.fill('input[name="password"]', '123456');
    await page.click('button[type="submit"]');
    await page.goto('/Admin/Statistics');
    await expect(page.locator('h2')).toContainText('统计');
    await expect(page.locator('text=总预约数')).toBeVisible();
  });
});

test.describe('关键规则验证', () => {
  test('TC-11 重复预约冲突提示', async ({ page }) => {
    let conflictTested = false;
    for (const seatId of [16, 17, 18, 19, 1, 2, 3, 4]) {
      await page.goto(`/Seat/Detail/${seatId}`);
      const bookBtn = page.locator('text=预约此时段').first();
      if (await bookBtn.isVisible({ timeout: 2000 }).catch(() => false)) {
        const href = await bookBtn.getAttribute('href');
        if (!href) continue;
        const slotMatch = href.match(/timeSlot=([^&]+)/);
        if (!slotMatch) continue;
        const timeSlot = slotMatch[1];
        await page.goto(`/Reservation/Create?seatId=${seatId}&timeSlot=${timeSlot}`);
        await page.click('button:has-text("提交预约")');
        await expect(page).toHaveURL(/Reservation\/My/);
        await page.goto(`/Reservation/Create?seatId=${seatId}&timeSlot=${timeSlot}`);
        await page.click('button:has-text("提交预约")');
        await expect(page.locator('.alert-danger')).toContainText('该时段已被预约');
        conflictTested = true;
        break;
      }
    }
    expect(conflictTested).toBeTruthy();
  });

  test('TC-12 停用座位不可预约', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="username"]', 'admin');
    await page.fill('input[name="password"]', '123456');
    await page.click('button[type="submit"]');
    await page.goto('/Admin/SeatEdit/20');
    await page.selectOption('select[name="Status"]', '1');
    await page.click('button:has-text("保存")');
    await page.goto('/Seat/Detail/20');
    await expect(page.locator('text=维护中').first()).toBeVisible();
  });
});
