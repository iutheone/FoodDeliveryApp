import { useParams } from 'react-router-dom'
import restaurants from '../data/mock'
import MenuItemCard from '../components/MenuItemCard'
import CartDrawer from '../components/CartDrawer'

export default function RestaurantPage() {
  const { id } = useParams()
  const r = restaurants.find(x => x.id === id)
  if (!r) return <div className="empty">Restaurant not found</div>

  return (
    <div className="grid app">
      <div>
        <div className="card" style={{ alignItems: 'flex-start' }}>
          <img src={r.image} alt={r.name} style={{ width: 160, height: 110, objectFit: 'cover' }} />
          <div className="meta">
            <h2 style={{ marginBottom: 6 }}>{r.name}</h2>
            <div className="small">{r.cuisine} • {r.eta}</div>
            <p style={{ marginTop: 8 }}>{r.menu.length} items</p>
          </div>
        </div>

        <div className="menu">
          {r.menu.map(m => <MenuItemCard key={m.id} item={m} restaurantId={r.id} />)}
        </div>
      </div>

      <div>
        <CartDrawer />
      </div>
    </div>
  )
}
