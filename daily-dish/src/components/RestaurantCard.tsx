import type { Restaurant } from '../types'
import Rating from './Rating'
import { Link } from 'react-router-dom'

export default function RestaurantCard({ r }: { r: Restaurant }) {
  return (
    <Link to={`/restaurant/${r.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
      <div className="card">
        <img src={r.image ?? '/src/assets/placeholder.png'} alt={r.name} />
        <div className="meta" style={{padding:'2px'}}>
          <div style={{display: 'flex', justifyContent: 'space-between', gap:'10px', alignItems: 'center', padding:'10px 0'}}>
            <h3 style={{fontSize:'19px', margin:'auto 0'}}>{r.name}</h3>
            <Rating value={r.rating} />
          </div>
          <p className="small">{r.cuisine} • {r.eta}</p>
          <div style={{ marginTop: 8, display: 'flex', gap: 8, alignItems: 'center' }}>
          </div>
        </div>
        <div className="badge">{r.location}</div>
      </div>
    </Link>
  )
}
