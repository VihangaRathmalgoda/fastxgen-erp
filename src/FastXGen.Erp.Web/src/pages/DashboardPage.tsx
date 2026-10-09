import { useState } from 'react';
import { Link } from 'react-router-dom';
import { BranchSelect } from '../components/BranchSelect';
import { Icon } from '../components/Icon';
import { formatMoney } from '../data/money';
import { dashboard } from '../data/sampleData';
import styles from './DashboardPage.module.css';

const toneClass = { ok: 'pill-ok', warn: 'pill-warn', neutral: 'pill-neutral' } as const;
const dotColor = { ok: 'var(--brand)', warn: 'var(--warn-dot)', neutral: 'var(--muted)' } as const;

export function DashboardPage() {
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const [allRead, setAllRead] = useState(false);
  const [approved, setApproved] = useState<ReadonlySet<string>>(new Set());

  const unread = allRead ? 0 : dashboard.initialUnread;
  const weekMax = Math.max(...dashboard.week.map((d) => d.sales));
  const weekTotal = dashboard.week.reduce((sum, d) => sum + d.sales, 0);

  return (
    <>
      <header className={styles.toolbar}>
        <div className={`search ${styles.search}`}>
          <label className="sr-only" htmlFor="dash-search">
            Search
          </label>
          <input id="dash-search" className="field" type="search" placeholder="Search products, invoices, customers" />
          <Icon name="search" />
        </div>
        <div className={styles.toolbarRight}>
          <BranchSelect id="dash-branch" />
          <button
            type="button"
            className={styles.bell}
            aria-label={unread > 0 ? `Notifications, ${unread} unread` : 'Notifications'}
            aria-expanded={notificationsOpen}
            onClick={() => setNotificationsOpen((open) => !open)}
          >
            <Icon name="bell" />
            {unread > 0 && <span className={styles.badge}>{unread}</span>}
          </button>
        </div>
      </header>

      {notificationsOpen && (
        <section aria-label="Notifications" className={styles.notifications}>
          <div className={styles.notificationsHead}>
            <h2>Notifications</h2>
            <button type="button" className={styles.linkButton} onClick={() => setAllRead(true)}>
              Mark all read
            </button>
          </div>
          {dashboard.notifications.map((n, i) => (
            <div
              key={n.title}
              className={styles.notification}
              style={{ background: !allRead && i < dashboard.initialUnread ? 'var(--brand-tint)' : undefined }}
            >
              <span className={styles.notificationDot} style={{ background: dotColor[n.tone] }} />
              <div className={styles.stack}>
                <strong>{n.title}</strong>
                <span className="muted">{n.body}</span>
                <span className={`muted ${styles.small}`}>{n.time}</span>
              </div>
            </div>
          ))}
        </section>
      )}

      <div className="page-head">
        <div className="page-head-text">
          <h1>Dashboard</h1>
          <span className="muted">All branches · Today · Sample data</span>
        </div>
        <Link to="/pos" className="btn btn-primary">
          Open POS
        </Link>
      </div>

      <section aria-label="Key figures" className={styles.kpis}>
        {dashboard.kpis.map((kpi) => (
          <div key={kpi.label} className={`card ${styles.kpi}`}>
            <span className={`muted ${styles.kpiLabel}`}>{kpi.label}</span>
            <span className={`mono ${styles.kpiValue}`}>
              {kpi.isMoney ? formatMoney(kpi.value) : kpi.value.toLocaleString('en-US')}
            </span>
            <span className={`pill ${toneClass[kpi.tone]}`}>{kpi.trend}</span>
          </div>
        ))}
      </section>

      <div className="row">
        <section aria-label="Sales this week" className="card grow-2">
          <div className={styles.cardHead}>
            <h2 className="card-title">Sales this week</h2>
            <span className="mono muted">{formatMoney(weekTotal)}</span>
          </div>
          <div className={styles.chart}>
            {dashboard.week.map((d, i) => (
              <div key={d.day} className={styles.barColumn}>
                <span className={`mono muted ${styles.barValue}`}>{Math.round(d.sales / 1000)}k</span>
                <div
                  className={styles.bar}
                  style={{
                    height: `${Math.round((d.sales / weekMax) * 100)}%`,
                    background: i === dashboard.week.length - 1 ? 'var(--brand)' : 'var(--brand-chart)',
                  }}
                />
              </div>
            ))}
          </div>
          <div className={styles.chartAxis}>
            {dashboard.week.map((d) => (
              <span key={d.day} className="muted">
                {d.day}
              </span>
            ))}
          </div>
        </section>

        <section aria-label="Pending approvals" className="card grow-1">
          <h2 className="card-title">Waiting for your approval</h2>
          {dashboard.approvals.map((a) => (
            <div key={a.id} className={styles.approval}>
              <div className={styles.stack}>
                <strong>{a.title}</strong>
                <span className={`muted ${styles.small}`}>{a.meta}</span>
              </div>
              {approved.has(a.id) ? (
                <span className="pill pill-ok">Approved</span>
              ) : (
                <button
                  type="button"
                  className="btn btn-sm btn-outline-brand"
                  onClick={() => setApproved((current) => new Set(current).add(a.id))}
                >
                  Approve
                </button>
              )}
            </div>
          ))}
        </section>
      </div>

      <div className="row">
        <section aria-label="Branch comparison" className="card grow-2">
          <h2 className="card-title">Branches today</h2>
          <div className="table-scroll">
            <table className="table" style={{ minWidth: 480 }}>
              <thead>
                <tr>
                  <th>Branch</th>
                  <th className="num">Sales</th>
                  <th className="num">Bills</th>
                  <th className="num">vs last week</th>
                </tr>
              </thead>
              <tbody>
                {dashboard.branchesToday.map((b) => (
                  <tr key={b.name}>
                    <td className={styles.strongCell}>{b.name}</td>
                    <td className="num">{formatMoney(b.sales)}</td>
                    <td className="num">{b.bills}</td>
                    <td
                      className={styles.change}
                      style={{ color: b.change < 0 ? 'var(--danger)' : 'var(--brand-text)' }}
                    >
                      {b.change < 0 ? '−' : '+'}
                      {Math.abs(b.change).toFixed(1)}%
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        <section aria-label="Low stock" className="card grow-1">
          <div className={styles.cardHead}>
            <h2 className="card-title">Low stock</h2>
            <Link to="/products" className={styles.viewAll}>
              View all
            </Link>
          </div>
          {dashboard.lowStock.map((p) => (
            <div key={p.name} className={styles.lowStockRow}>
              <div className={styles.stack}>
                <span className={styles.strongCell}>{p.name}</span>
                <span className={`muted ${styles.small}`}>
                  {p.branch} · reorder at {p.reorderAt}
                </span>
              </div>
              <span className={`pill pill-warn mono ${styles.qty}`}>{p.qty} left</span>
            </div>
          ))}
        </section>
      </div>
    </>
  );
}
