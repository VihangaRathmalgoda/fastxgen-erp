import { branches } from '../data/sampleData';

export function BranchSelect({ id }: { id: string }) {
  return (
    <>
      <label className="sr-only" htmlFor={id}>
        Branch
      </label>
      <select id={id} className="field" style={{ width: 'auto', borderColor: 'var(--line-field)' }}>
        <option>All branches</option>
        {branches.map((b) => (
          <option key={b}>{b}</option>
        ))}
      </select>
    </>
  );
}
