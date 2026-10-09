import { useState } from 'react';
import { Icon } from '../components/Icon';
import { formatMoney, formatSignedMoney } from '../data/money';
import { branches, ruleSimulation } from '../data/sampleData';
import styles from './ReportingRulePage.module.css';

// Rule lifecycle (ADR 0009): draft, validate, simulate, publish.
const lifecycle = ['Draft', 'Validate', 'Simulate', 'Publish'] as const;
const stage = { validated: 2, simulated: 3, published: 4 } as const;

export function ReportingRulePage() {
  const [currentStage, setCurrentStage] = useState<number>(stage.validated);
  const simulated = currentStage >= stage.simulated;
  const published = currentStage >= stage.published;
  const canPublish = currentStage === stage.simulated;

  const days = ruleSimulation.days.map((d, i) => {
    const carriedIn = i === 0 ? ruleSimulation.carriedIntoFirstDay : ruleSimulation.days[i - 1].movedOut;
    const reported = d.original - d.movedOut + carriedIn;
    return { day: d.day, original: d.original, reported, difference: reported - d.original };
  });
  const weekTotal = ruleSimulation.days.reduce((sum, d) => sum + d.original, 0);

  const history = [{ what: 'Draft created and validated by Owner', when: '9 Oct 2026, 10:12' }];
  if (simulated) {
    history.unshift({ what: 'Simulated on 5 to 11 Oct by Owner', when: 'Just now' });
  }
  if (published) {
    history.unshift({ what: 'Version 1 published by Owner', when: 'Just now' });
  }

  return (
    <>
      <div className="page-head">
        <div className="page-head-text">
          <span className={`muted ${styles.breadcrumb}`}>Reporting rules / Main Street</span>
          <h1>Next-day reporting after cutoff</h1>
        </div>
        <span className={`pill ${published ? 'pill-ok' : 'pill-neutral'} ${styles.status}`}>
          {published ? 'Published · version 1' : 'Draft'}
        </span>
      </div>

      <ol aria-label="Rule lifecycle" className={styles.steps}>
        {lifecycle.map((label, i) => {
          const done = i < currentStage;
          const current = i === currentStage;
          return (
            <li
              key={label}
              className={current ? `${styles.step} ${styles.stepCurrent}` : styles.step}
              aria-current={current ? 'step' : undefined}
            >
              <span className={done || current ? `${styles.stepDot} ${styles.stepDotOn}` : styles.stepDot}>
                {done ? '✓' : i + 1}
              </span>
              <span className={done || current ? styles.stepLabelOn : 'muted'}>{label}</span>
            </li>
          );
        })}
      </ol>

      <div className={`row ${styles.columns}`}>
        <section aria-label="Rule settings" className={`card ${styles.settings}`}>
          <h2 className="card-title">Rule</h2>
          <div className="field-group">
            <label className="field-label" htmlFor="rule-scope">
              Applies to
            </label>
            <select id="rule-scope" className="field">
              <option>Main Street</option>
              <option>All branches</option>
              {branches
                .filter((b) => b !== 'Main Street')
                .map((b) => (
                  <option key={b}>{b}</option>
                ))}
            </select>
          </div>
          <div className="field-group">
            <span className="field-label">When</span>
            <div className={styles.when}>
              <span>a sale is completed after</span>
              <label className="sr-only" htmlFor="rule-cutoff">
                Cutoff time
              </label>
              <input id="rule-cutoff" className={`field ${styles.time}`} type="time" defaultValue="14:00" />
            </div>
          </div>
          <div className="field-group">
            <label className="field-label" htmlFor="rule-action">
              Then
            </label>
            <select id="rule-action" className="field">
              <option>Report it on the next business day</option>
              <option>Exclude it from the selected report</option>
            </select>
          </div>
          <div className="field-group">
            <label className="field-label" htmlFor="rule-from">
              Effective from
            </label>
            <input id="rule-from" className="field" type="date" defaultValue="2026-10-01" />
          </div>
          <div className={styles.info}>
            <Icon name="info" />
            <span>
              Original sales are never changed. The rule only sets the date used in reports, and every report shows
              both totals. Ask your accountant to confirm tax treatment before publishing.
            </span>
          </div>
          <div className={styles.ruleActions}>
            <button
              type="button"
              className="btn btn-lg btn-outline-brand"
              onClick={() => setCurrentStage((s) => Math.max(s, stage.simulated))}
            >
              Simulate on last week
            </button>
            <button
              type="button"
              className="btn btn-lg btn-primary"
              aria-disabled={!canPublish}
              onClick={() => canPublish && setCurrentStage(stage.published)}
            >
              Publish rule
            </button>
          </div>
        </section>

        <section aria-label="Simulation result" className={`card ${styles.simulation}`}>
          <h2 className="card-title">Simulation</h2>
          {simulated ? (
            <>
              <div className={styles.figures}>
                <Figure label="Sales checked" value={ruleSimulation.salesChecked.toLocaleString('en-US')} />
                <Figure label="Moved to next day" value={ruleSimulation.salesMoved.toLocaleString('en-US')} />
                <Figure label="Week total (unchanged)" value={formatMoney(weekTotal)} />
              </div>
              <div className="table-scroll">
                <table className="table" style={{ minWidth: 520 }}>
                  <thead>
                    <tr>
                      <th>Day</th>
                      <th className="num">Original total</th>
                      <th className="num">Reported total</th>
                      <th className="num">Difference</th>
                    </tr>
                  </thead>
                  <tbody>
                    {days.map((d) => (
                      <tr key={d.day}>
                        <td className={styles.day}>{d.day}</td>
                        <td className="num">{formatMoney(d.original)}</td>
                        <td className="num">{formatMoney(d.reported)}</td>
                        <td
                          className="num"
                          style={{
                            color:
                              d.difference < 0 ? 'var(--danger)' : d.difference > 0 ? 'var(--brand-text)' : 'var(--muted)',
                          }}
                        >
                          {formatSignedMoney(d.difference)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          ) : (
            <div className={styles.placeholder}>
              Run a simulation to see which sales would move and how the daily totals change. Nothing is saved.
            </div>
          )}

          <div className={styles.history}>
            <h3>History</h3>
            {history.map((h) => (
              <div key={h.what} className={styles.historyRow}>
                <span>{h.what}</span>
                <span className="muted">{h.when}</span>
              </div>
            ))}
          </div>
        </section>
      </div>
    </>
  );
}

function Figure({ label, value }: { label: string; value: string }) {
  return (
    <div className={styles.figure}>
      <span className="muted">{label}</span>
      <span className="mono">{value}</span>
    </div>
  );
}
