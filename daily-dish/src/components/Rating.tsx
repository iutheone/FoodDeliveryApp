export default function Rating({ value }: { value: number }) {
  return <div style={{ background: '#eef', padding: '4px 8px', borderRadius: 8, fontWeight: 700 }}>{value.toFixed(1)}</div>
}