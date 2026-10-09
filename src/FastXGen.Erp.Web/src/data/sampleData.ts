// Sample data from the design canvas. Replaced by API calls once the modules exist.

export const branches = ['Main Street', 'Harbour Road', 'City Centre'] as const;

export type Category = 'Groceries' | 'Beverages' | 'Household' | 'Personal care';

export const categories: Category[] = ['Groceries', 'Beverages', 'Household', 'Personal care'];

export interface Product {
  id: string;
  sku: string;
  name: string;
  category: Category;
  price: number;
  stock: number;
  reorderAt: number;
}

export const products: Product[] = [
  { id: 'p1', sku: 'GR-1001', name: 'Basmati rice 5kg', category: 'Groceries', price: 2450, stock: 9, reorderAt: 15 },
  { id: 'p2', sku: 'GR-1002', name: 'Red lentils 1kg', category: 'Groceries', price: 420, stock: 64, reorderAt: 20 },
  { id: 'p3', sku: 'GR-1003', name: 'Milk powder 400g', category: 'Groceries', price: 1180, stock: 6, reorderAt: 20 },
  { id: 'p4', sku: 'GR-1004', name: 'Sunflower oil 1L', category: 'Groceries', price: 890, stock: 11, reorderAt: 24 },
  { id: 'p5', sku: 'BV-2001', name: 'Tea leaves 200g', category: 'Beverages', price: 640, stock: 13, reorderAt: 30 },
  { id: 'p6', sku: 'BV-2002', name: 'Instant coffee 100g', category: 'Beverages', price: 1350, stock: 38, reorderAt: 12 },
  { id: 'p7', sku: 'BV-2003', name: 'Orange juice 1L', category: 'Beverages', price: 560, stock: 0, reorderAt: 18 },
  { id: 'p8', sku: 'HH-3001', name: 'Dish wash liquid 500ml', category: 'Household', price: 395, stock: 52, reorderAt: 20 },
  { id: 'p9', sku: 'HH-3002', name: 'Laundry powder 1kg', category: 'Household', price: 780, stock: 29, reorderAt: 15 },
  { id: 'p10', sku: 'HH-3003', name: 'Paper towels 2 rolls', category: 'Household', price: 350, stock: 75, reorderAt: 30 },
  { id: 'p11', sku: 'PC-4001', name: 'Toothpaste 120g', category: 'Personal care', price: 310, stock: 88, reorderAt: 25 },
  { id: 'p12', sku: 'PC-4002', name: 'Shampoo 200ml', category: 'Personal care', price: 690, stock: 41, reorderAt: 20 },
];

export const totalProductCount = 1284;

export type StockStatus = 'ok' | 'low' | 'out';

export function stockStatus(product: Product): StockStatus {
  if (product.stock === 0) {
    return 'out';
  }
  return product.stock < product.reorderAt ? 'low' : 'ok';
}

export const dashboard = {
  kpis: [
    { label: 'Sales today', value: 486250, isMoney: true, trend: '+8.4% vs last Sunday', tone: 'ok' },
    { label: 'Bills', value: 312, isMoney: false, trend: '+21 vs last Sunday', tone: 'ok' },
    { label: 'Average bill', value: 1558, isMoney: true, trend: '−2.1% vs last Sunday', tone: 'warn' },
    { label: 'Low-stock items', value: 14, isMoney: false, trend: '4 need reorder today', tone: 'warn' },
  ],
  week: [
    { day: 'Mon', sales: 412300 },
    { day: 'Tue', sales: 389900 },
    { day: 'Wed', sales: 455100 },
    { day: 'Thu', sales: 438700 },
    { day: 'Fri', sales: 521600 },
    { day: 'Sat', sales: 604200 },
    { day: 'Sun', sales: 486250 },
  ],
  approvals: [
    { id: 'a1', title: 'Discount 18% on a 42,500 sale', meta: 'Cashier 2 · Main Street · limit 10%' },
    { id: 'a2', title: 'Stock transfer TR-2026-0117', meta: 'Main Street → Harbour Road · 36 items' },
    { id: 'a3', title: 'Expense: shop repairs', meta: 'City Centre · Rs 28,000' },
  ],
  notifications: [
    { title: 'Low stock: Milk powder 400g', body: 'Only 6 left at Harbour Road.', time: '10 min ago', tone: 'warn' },
    { title: 'Discount needs approval', body: 'Cashier 2 asked for 18% on a 42,500 sale.', time: '24 min ago', tone: 'ok' },
    { title: 'Day closed at City Centre', body: 'Cash drawer reconciled, no difference.', time: '1 h ago', tone: 'ok' },
    { title: 'Supplier invoice due', body: 'Fresh Foods Ltd invoice due in 2 days.', time: 'Yesterday', tone: 'neutral' },
  ],
  initialUnread: 3,
  branchesToday: [
    { name: 'Main Street', sales: 214900, bills: 138, change: 12.0 },
    { name: 'Harbour Road', sales: 156300, bills: 104, change: 4.5 },
    { name: 'City Centre', sales: 115050, bills: 70, change: -3.2 },
  ],
  lowStock: [
    { name: 'Milk powder 400g', branch: 'Harbour Road', reorderAt: 20, qty: 6 },
    { name: 'Basmati rice 5kg', branch: 'Main Street', reorderAt: 15, qty: 9 },
    { name: 'Sunflower oil 1L', branch: 'City Centre', reorderAt: 24, qty: 11 },
    { name: 'Tea leaves 200g', branch: 'Main Street', reorderAt: 30, qty: 13 },
  ],
} as const;

export const ruleSimulation = {
  days: [
    { day: 'Mon 5 Oct', original: 412300, movedOut: 96400 },
    { day: 'Tue 6 Oct', original: 389900, movedOut: 88200 },
    { day: 'Wed 7 Oct', original: 455100, movedOut: 101500 },
    { day: 'Thu 8 Oct', original: 438700, movedOut: 97300 },
    { day: 'Fri 9 Oct', original: 521600, movedOut: 118900 },
    { day: 'Sat 10 Oct', original: 604200, movedOut: 142600 },
    { day: 'Sun 11 Oct', original: 486250, movedOut: 109800 },
  ],
  carriedIntoFirstDay: 91700,
  salesChecked: 1946,
  salesMoved: 412,
};
