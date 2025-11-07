import { useCart, useCartDispatch } from '../context/CartContext'
import { useNavigate } from 'react-router-dom'

export default function Checkout() {
  const cart = useCart()
  const dispatch = useCartDispatch()
  const navigate = useNavigate()
  const subtotal = cart.items.reduce((s, i) => s + i.qty * i.item.price, 0)

  function placeOrder() {
    // Demo: clear cart and go to a simple confirmation
    dispatch({ type: 'CLEAR' })
    alert('Order placed! 🎉')
    navigate('/')
  }

  if (cart.items.length === 0) return <div className="app empty">No items in cart</div>

  return (
    <div className="app">
      <h2>Checkout</h2>
      <div className="card">
        <div>
          {cart.items.map(i => (
            <div key={i.item.id} style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
              <div>
                <div style={{ fontWeight: 700 }}>{i.item.name} × {i.qty}</div>
                <div className="small">${(i.item.price * i.qty).toFixed(2)}</div>
              </div>
              <div>${(i.item.price * i.qty).toFixed(2)}</div>
            </div>
          ))}
        </div>
        <div style={{ marginTop: 12 }}>
          <div className="small">Subtotal</div>
          <div style={{ fontWeight: 800 }}>${subtotal.toFixed(2)}</div>
        </div>
        <div style={{ marginTop: 12 }}>
          <button className="checkout-btn" onClick={placeOrder}>Place Order</button>
        </div>
      </div>
    </div>
  )
}
