import { NavLink, Outlet } from 'react-router-dom';
import { Icon, type IconName } from './Icon';
import styles from './AppShell.module.css';

interface NavItem {
  label: string;
  icon: IconName;
  to?: string;
}

const navItems: NavItem[] = [
  { label: 'Dashboard', icon: 'dashboard', to: '/' },
  { label: 'POS', icon: 'cart', to: '/pos' },
  { label: 'Sales', icon: 'receipt' },
  { label: 'Products & stock', icon: 'box', to: '/products' },
  { label: 'Purchasing', icon: 'truck' },
  { label: 'Reports', icon: 'chart' },
  { label: 'Reporting rules', icon: 'sliders', to: '/rules' },
  { label: 'Users & roles', icon: 'users' },
];

export function AppShell() {
  return (
    <div className={styles.shell}>
      <nav aria-label="Main menu" className={styles.nav}>
        <div className={styles.brand}>
          <span className={styles.logo}>FX</span>
          <span className={styles.brandText}>
            <span className={styles.product}>FastXGen ERP</span>
            <span className={styles.tenant}>ABC Traders</span>
          </span>
        </div>

        <div className={styles.links}>
          {navItems.map((item) =>
            item.to ? (
              <NavLink
                key={item.label}
                to={item.to}
                end={item.to === '/'}
                className={({ isActive }) => (isActive ? `${styles.link} ${styles.active}` : styles.link)}
              >
                <Icon name={item.icon} />
                {item.label}
              </NavLink>
            ) : (
              <a key={item.label} href="#" className={styles.link} aria-disabled="true" title="Coming soon">
                <Icon name={item.icon} />
                {item.label}
              </a>
            ),
          )}
        </div>

        <div className={styles.user}>
          <span className={styles.avatar}>OW</span>
          <span className={styles.brandText}>
            <span className={styles.userName}>Owner</span>
            <span className={styles.tenant}>All branches</span>
          </span>
        </div>
      </nav>

      <main className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
}
