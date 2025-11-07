import { useState } from 'react'
import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import Restaurant from './pages/Restaurant'
import Checkout from './pages/Checkout'
import Header from './components/header'

function App() {
  const [query, setQuery] = useState('')
  return (
    <>
      <Header query={query} setQuery={setQuery} />
      <Routes>
      <Route path="/" element={<Home query={query} />} />
        <Route path="/restaurant/:id" element={<Restaurant />} />
        <Route path="/checkout" element={<Checkout />} />
        <Route path="*" element={<div className="app empty">Not found</div>} />
      </Routes>
    </>
  )
}

export default App
