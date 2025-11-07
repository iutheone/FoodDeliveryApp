import type { Restaurant } from '../types'
import Rating from './Rating'
import { Link } from 'react-router-dom'

export default function RestaurantCard({ r }: { r: Restaurant }) {
  return (
    <Link to={`/restaurant/${r.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
      <div className="card">
        <img src={r.image ?? '/src/assets/placeholder.png'} alt={r.name} />
        <div className="meta">
          <h3>{r.name}</h3>
          <p className="small">{r.cuisine} • {r.eta}</p>
          <div style={{ marginTop: 8, display: 'flex', gap: 8, alignItems: 'center' }}>
            <Rating value={r.rating} />
            <div className="small">{r.rating.toFixed(1)}</div>
          </div>
        </div>
        <div className="badge">{r.eta}</div>
      </div>
    </Link>
  )
}
