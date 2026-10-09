import { useState } from 'react';
import { BranchSelect } from '../components/BranchSelect';
import { Icon } from '../components/Icon';
import { formatMoney } from '../data/money';
import { products, stockStatus, totalProductCount, type StockStatus } from '../data/sampleData';
import styles from './ProductsPage.module.css';

type Filter = 'all' | 'low' | 'out';

const filters: { value: Filter; label: string }[] = [
  { value: 'all', label: 'All' },
  { value: 'low', label: 'Low stock' },
  { value: 'out', label: 'Out of stock' },
];

const statusDisplay: Record<StockStatus, { label: string; className: string }> = {
  ok: { label: 'In stock', className: 'pill-ok' },
  low: { label: 'Low stock', className: 'pill-warn' },
  out: { label: 'Out of stock', className: 'pill-danger' },
};

function matchesFilter(status: StockStatus, filter: Filter): boolean {
  if (filter === 'all') {
    return true;
  }
  return filter === 'low' ? status !== 'ok' : status === 'out';
}

export function ProductsPage() {
  const [filter, setFilter] = useState<Filter>('all');
  const rows = products
    .map((p) => ({ ...p, status: stockStatus(p) }))
    .filter((p) => matchesFilter(p.status, filter));

  return (
    <>
      <div className="page-head">
        <div className="page-head-text">
          <h1>Products &amp; stock</h1>
          <span className="muted">
            Stock is a record of every movement, so each balance traces back to sales, receipts and transfers. Sample
            data.
          </span>
        </div>
        <div className={styles.actions}>
          <button type="button" className="btn">
            Stock transfer
          </button>
          <button type="button" className="btn">
            Stock count
          </button>
          <button type="button" className="btn btn-primary">
            Add product
          </button>
        </div>
      </div>

      <div className={styles.filterBar}>
        <div className={`search ${styles.search}`}>
          <label className="sr-only" htmlFor="prod-search">
            Search products
          </label>
          <input id="prod-search" className="field" type="search" placeholder="Search by name, SKU or barcode" />
          <Icon name="search" />
        </div>
        <div role="group" aria-label="Filter" className="segmented">
          {filters.map((f) => (
            <button key={f.value} type="button" aria-pressed={filter === f.value} onClick={() => setFilter(f.value)}>
              {f.label}
            </button>
          ))}
        </div>
        <BranchSelect id="prod-branch" />
      </div>

      <section aria-label="Product list" className={styles.listCard}>
        <div className="table-scroll">
          <table className={`table ${styles.table}`}>
            <thead>
              <tr>
                <th>SKU</th>
                <th>Product</th>
                <th>Category</th>
                <th className="num">Price</th>
                <th className="num">In stock</th>
                <th className="num">Reorder at</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id}>
                  <td className={`mono ${styles.sku}`}>{r.sku}</td>
                  <td className={styles.name}>{r.name}</td>
                  <td className={styles.category}>{r.category}</td>
                  <td className="num">{formatMoney(r.price, 2)}</td>
                  <td className={`num ${styles.stock}`}>{r.stock}</td>
                  <td className="num muted">{r.reorderAt}</td>
                  <td>
                    <span className={`pill ${statusDisplay[r.status].className}`}>{statusDisplay[r.status].label}</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className={styles.pager}>
          <span>
            Showing {rows.length} of {totalProductCount.toLocaleString('en-US')} products
          </span>
          <div className={styles.pagerButtons}>
            <button type="button" className="btn btn-sm">
              Previous
            </button>
            <button type="button" className="btn btn-sm">
              Next
            </button>
          </div>
        </div>
      </section>
    </>
  );
}
