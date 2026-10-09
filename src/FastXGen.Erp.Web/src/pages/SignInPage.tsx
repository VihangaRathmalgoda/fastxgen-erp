import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Icon } from '../components/Icon';
import styles from './SignInPage.module.css';

type Mode = 'email' | 'pin';

const pinLength = 4;
const highlights = [
  'Barcode-first POS with split payments',
  'Stock, purchasing and transfers per branch',
  'Every change recorded: who, what and when',
];

export function SignInPage() {
  const [mode, setMode] = useState<Mode>('email');

  return (
    <div className={styles.page}>
      <section aria-label="About FastXGen ERP" className={styles.hero}>
        <div className={styles.brand}>
          <span className={styles.logo}>FX</span>
          <span>FastXGen ERP</span>
        </div>
        <div className={styles.pitch}>
          <h1>Every branch, every till, one system.</h1>
          <p>
            Fast checkout at the counter, live stock across branches, and reports you can trust, for shops of every
            size.
          </p>
        </div>
        <ul className={styles.highlights}>
          {highlights.map((text) => (
            <li key={text}>
              <Icon name="check" strokeWidth={2} color="var(--brand-check)" />
              {text}
            </li>
          ))}
        </ul>
      </section>

      <main className={styles.formArea}>
        <div className={styles.panel}>
          <div className={styles.panelHead}>
            <h2>Sign in</h2>
            <span className="muted">Choose how you work today.</span>
          </div>

          <div role="tablist" aria-label="Sign-in method" className={`segmented ${styles.tabs}`}>
            <button type="button" role="tab" aria-selected={mode === 'email'} onClick={() => setMode('email')}>
              Staff &amp; owners
            </button>
            <button type="button" role="tab" aria-selected={mode === 'pin'} onClick={() => setMode('pin')}>
              Cashier PIN
            </button>
          </div>

          {mode === 'email' ? <EmailForm /> : <PinPad />}
        </div>
      </main>
    </div>
  );
}

function EmailForm() {
  return (
    <form className={styles.form} onSubmit={(event) => event.preventDefault()}>
      <div className="field-group">
        <label className="field-label" htmlFor="login-email">
          Email
        </label>
        <input id="login-email" className="field" type="email" autoComplete="username" placeholder="you@yourshop.com" />
      </div>
      <div className="field-group">
        <div className={styles.passwordHead}>
          <label className="field-label" htmlFor="login-password">
            Password
          </label>
          <a href="#">Forgot password?</a>
        </div>
        <input id="login-password" className="field" type="password" autoComplete="current-password" />
      </div>
      <label className={styles.remember}>
        <input type="checkbox" />
        Keep me signed in on this computer
      </label>
      <Link to="/" className="btn btn-primary btn-lg btn-block">
        Sign in
      </Link>
      <span className={`muted ${styles.note}`}>Owners and admins confirm with an authenticator code.</span>
    </form>
  );
}

function PinPad() {
  const [pin, setPin] = useState('');
  const press = (digit: string) => setPin((current) => (current.length < pinLength ? current + digit : current));
  const keys = [
    ...['1', '2', '3', '4', '5', '6', '7', '8', '9'].map((d) => ({ text: d, label: `Digit ${d}`, onPress: () => press(d) })),
    { text: 'C', label: 'Clear PIN', onPress: () => setPin('') },
    { text: '0', label: 'Digit 0', onPress: () => press('0') },
    { text: '⌫', label: 'Delete last digit', onPress: () => setPin((current) => current.slice(0, -1)) },
  ];

  return (
    <div className={styles.form}>
      <div className={styles.till}>
        <Icon name="monitor" color="var(--brand-text)" />
        <span className={styles.tillText}>
          <strong>Main Street · Counter 2</strong>
          <span className="muted">Registered till</span>
        </span>
      </div>
      <div className={styles.dots} aria-live="polite" aria-label={`${pin.length} of ${pinLength} digits entered`}>
        {Array.from({ length: pinLength }, (_, i) => (
          <span key={i} className={i < pin.length ? `${styles.dot} ${styles.dotFilled}` : styles.dot} />
        ))}
      </div>
      <div className={styles.keypad}>
        {keys.map((key) => (
          <button key={key.label} type="button" className={styles.key} aria-label={key.label} onClick={key.onPress}>
            {key.text}
          </button>
        ))}
      </div>
      {pin.length === pinLength ? (
        <Link to="/pos" className="btn btn-primary btn-lg btn-block">
          Start shift
        </Link>
      ) : (
        <span className={styles.pinHint}>Enter your 4-digit PIN</span>
      )}
    </div>
  );
}
