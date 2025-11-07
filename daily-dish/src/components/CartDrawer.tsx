import { useCart, useCartDispatch } from '../context/CartContext'
import type { CartItem } from '../types'
import { Link } from 'react-router-dom'

export default function CartDrawer() {
  const cart = useCart()
  const dispatch = useCartDispatch()
  const subtotal = cart.items.reduce((s, i) => s + i.qty * i.item.price, 0)

  if (cart.items.length === 0) return <div className="cart-drawer"><div className="empty">Your cart is empty</div></div>

  return (
    <div className="cart-drawer">
      <h3>Cart</h3>
      <div className="cart-list">
        {cart.items.map((c: CartItem) => (
          <div key={c.item.id} className="cart-row">
            <div>
              <div style={{ fontWeight: 700 }}>{c.item.name}</div>
              <div className="small">${c.item.price.toFixed(2)} • {c.qty} qty</div>
            </div>
            <div style={{ display: 'flex', gap: 6 }}>
              <button onClick={() => dispatch({ type: 'DECREMENT', itemId: c.item.id, restaurantId: c.restaurantId })}>-</button>
              <button onClick={() => dispatch({ type: 'INCREMENT', itemId: c.item.id, restaurantId: c.restaurantId })}>+</button>
              <button onClick={() => dispatch({ type: 'REMOVE', itemId: c.item.id, restaurantId: c.restaurantId })}>remove</button>
            </div>
          </div>
        ))}
      </div>
      <div style={{ marginTop: 12 }}>
        <div className="small">Subtotal</div>
        <div style={{ fontWeight: 800, fontSize: 18 }}>${subtotal.toFixed(2)}</div>
        <Link to="/checkout"><button className="checkout-btn">Proceed to checkout</button></Link>
      </div>
    </div>
  )
}
