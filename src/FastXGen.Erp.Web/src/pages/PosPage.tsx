import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Icon } from '../components/Icon';
import { formatMoney } from '../data/money';
import { categories, products, type Category } from '../data/sampleData';
import styles from './PosPage.module.css';

type CategoryFilter = 'All' | Category;
type PaymentMethod = 'Cash' | 'Card' | 'Transfer' | 'Split';

const paymentMethods: PaymentMethod[] = ['Cash', 'Card', 'Transfer', 'Split'];
const lowStockThreshold = 15;

// Bill totals here are a preview. The sale endpoint calculates and returns the authoritative amounts.
export function PosPage() {
  const [category, setCategory] = useState<CategoryFilter>('All');
  const [cart, setCart] = useState<Record<string, number>>({ p1: 2, p5: 1, p9: 1 });
  const [method, setMethod] = useState<PaymentMethod>('Cash');
  const [paid, setPaid] = useState(false);

  const changeQuantity = (id: string, delta: number) =>
    setCart((current) => {
      const next = { ...current };
      const quantity = (next[id] ?? 0) + delta;
      if (quantity <= 0) {
        delete next[id];
      } else {
        next[id] = quantity;
      }
      return next;
    });

  const lines = products.filter((p) => cart[p.id]).map((p) => ({ product: p, quantity: cart[p.id] }));
  const itemCount = lines.reduce((sum, l) => sum + l.quantity, 0);
  const total = lines.reduce((sum, l) => sum + l.quantity * l.product.price, 0);
  const shown = products.filter((p) => category === 'All' || p.category === category);

  const startNewSale = () => {
    setCart({});
    setMethod('Cash');
    setPaid(false);
  };

  return (
    <div className={styles.page}>
      <header className={styles.header}>
        <Link to="/" className={styles.brand}>
          <span className={styles.logo}>FX</span>
          <span>POS</span>
        </Link>
        <span className={styles.headerMeta}>Main Street · Counter 2</span>
        <span className={styles.headerMeta}>Cashier 2 · shift open since 08:02</span>
        <div className={styles.headerActions}>
          <button type="button" className={styles.headerButton}>
            Held bills (2)
          </button>
          <button type="button" className={styles.headerButton}>
            Returns
          </button>
          <Link to="/" className={`${styles.headerButton} ${styles.closeShift}`}>
            Close shift
          </Link>
        </div>
      </header>

      <div className={styles.body}>
        <section aria-label="Products" className={styles.catalog}>
          <div className={styles.scan}>
            <label className="sr-only" htmlFor="pos-search">
              Scan barcode or search
            </label>
            <input id="pos-search" type="search" placeholder="Scan barcode or type name, SKU or alias" />
            <Icon name="barcode" size={22} />
          </div>

          <div role="group" aria-label="Categories" className={styles.chips}>
            {(['All', ...categories] as CategoryFilter[]).map((name) => (
              <button key={name} type="button" aria-pressed={category === name} onClick={() => setCategory(name)}>
                {name}
              </button>
            ))}
          </div>

          <div className={styles.tiles}>
            {shown.map((p) => (
              <button
                key={p.id}
                type="button"
                className={styles.tile}
                aria-label={`Add ${p.name}`}
                onClick={() => changeQuantity(p.id, 1)}
              >
                <span className={`mono muted ${styles.sku}`}>{p.sku}</span>
                <span className={styles.tileName}>{p.name}</span>
                <span className={styles.tileFoot}>
                  <span className="mono">{formatMoney(p.price, 2)}</span>
                  <span className={p.stock < lowStockThreshold ? styles.stockLow : 'muted'}>{p.stock} in stock</span>
                </span>
              </button>
            ))}
          </div>
        </section>

        <aside aria-label="Current bill" className={styles.bill}>
          {paid ? (
            <div className={styles.paid}>
              <span className={styles.paidIcon}>
                <Icon name="check" size={32} strokeWidth={2.2} color="var(--brand-text)" />
              </span>
              <h2>Payment received</h2>
              <span className={`mono ${styles.invoiceNo}`}>INV-BR01-2026-004913</span>
              <span className={`mono ${styles.paidTotal}`}>{formatMoney(total, 2)}</span>
              <span className="muted">Paid by {method.toLowerCase()} · receipt sent to printer</span>
              <div className={styles.paidActions}>
                <button type="button" className="btn btn-lg">
                  Reprint
                </button>
                <button type="button" className="btn btn-primary btn-lg" onClick={startNewSale}>
                  New sale
                </button>
              </div>
            </div>
          ) : (
            <>
              <div className={styles.billHead}>
                <div className={styles.billTitle}>
                  <h2>Current bill</h2>
                  <span className="muted">Walk-in customer</span>
                </div>
                <button type="button" className="btn btn-sm">
                  Add customer
                </button>
              </div>

              <div className={styles.lines}>
                {lines.length === 0 && <div className={styles.empty}>Scan an item or tap a product to start.</div>}
                {lines.map(({ product, quantity }) => (
                  <div key={product.id} className={styles.line}>
                    <div className={styles.lineText}>
                      <span className={styles.lineName}>{product.name}</span>
                      <span className={`mono muted ${styles.unit}`}>{formatMoney(product.price, 2)} each</span>
                    </div>
                    <div className={styles.stepper}>
                      <button
                        type="button"
                        aria-label={`Remove one ${product.name}`}
                        onClick={() => changeQuantity(product.id, -1)}
                      >
                        −
                      </button>
                      <span className="mono">{quantity}</span>
                      <button
                        type="button"
                        aria-label={`Add one ${product.name}`}
                        onClick={() => changeQuantity(product.id, 1)}
                      >
                        +
                      </button>
                    </div>
                    <span className={`mono ${styles.lineTotal}`}>{formatMoney(product.price * quantity, 2)}</span>
                  </div>
                ))}
              </div>

              <div className={styles.billFoot}>
                <div className={styles.summary}>
                  <div>
                    <span className="muted">Items</span>
                    <span className="mono">{itemCount}</span>
                  </div>
                  <div>
                    <span className="muted">Discount</span>
                    <span className="mono">{formatMoney(0, 2)}</span>
                  </div>
                  <div className={styles.totalRow}>
                    <span>Total</span>
                    <span className="mono">{formatMoney(total, 2)}</span>
                  </div>
                  <span className={`muted ${styles.taxNote}`}>Prices include tax.</span>
                </div>

                <div role="group" aria-label="Payment method" className={styles.methods}>
                  {paymentMethods.map((m) => (
                    <button key={m} type="button" aria-pressed={method === m} onClick={() => setMethod(m)}>
                      {m}
                    </button>
                  ))}
                </div>

                <div className={styles.chargeRow}>
                  <button type="button" className={`btn ${styles.hold}`} onClick={() => setCart({})}>
                    Hold
                  </button>
                  <button
                    type="button"
                    className={`btn btn-primary ${styles.charge}`}
                    disabled={total === 0}
                    onClick={() => setPaid(true)}
                  >
                    Charge {formatMoney(total, 2)}
                  </button>
                </div>
              </div>
            </>
          )}
        </aside>
      </div>
    </div>
  );
}
