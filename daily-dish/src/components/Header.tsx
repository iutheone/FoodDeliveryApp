import { Link } from 'react-router-dom'
import { useCart } from '../context/CartContext'

export default function Header({ query, setQuery }: { query: string; setQuery: (s: string) => void }) {
  const cart = useCart()
  const count = cart.items.reduce((s, i) => s + i.qty, 0)
  return (
    <header className="header app">
      <div className="brand">
        <div className="logo">FD</div>
        <div>
          <h2 style={{ margin: 0 }}>FoodDelivery</h2>
          <div className="small">Fast & tasty</div>
        </div>
      </div>

      <div className="controls">
        <input className="search" value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Search restaurants or dishes..." />
        <Link to="/checkout" className="badge">Checkout</Link>
        <Link to="/cart" className="badge"> {count} items</Link>
      </div>
    </header>
  )
}
