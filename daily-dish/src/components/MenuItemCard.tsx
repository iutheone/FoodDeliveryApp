import type { MenuItem } from '../types'
import { useCartDispatch } from '../context/CartContext'

export default function MenuItemCard({ item, restaurantId }: { item: MenuItem; restaurantId: string }) {
  const dispatch = useCartDispatch()
  return (
    <div className="menu-item">
      <div className="left">
        <img src={item.image} alt={item.name} />
        <div>
          <div style={{ fontWeight: 700 }}>{item.name}</div>
          <div className="small">{item.description ?? ''}</div>
        </div>
      </div>
      <div style={{ textAlign: 'right' }}>
        <div style={{ fontWeight: 700 }}>Rs.{item.price.toFixed(2)}</div>
        <button className="btn" onClick={() => dispatch({ type: 'ADD', item, restaurantId })}>Add</button>
      </div>
    </div>
  )
}
