import React, { createContext, useContext, useReducer } from 'react'
import type { CartItem, MenuItem } from '../types'

type State = {
  items: CartItem[]
}

type Action =
  | { type: 'ADD'; item: MenuItem; restaurantId: string }
  | { type: 'REMOVE'; itemId: string; restaurantId: string }
  | { type: 'INCREMENT'; itemId: string; restaurantId: string }
  | { type: 'DECREMENT'; itemId: string; restaurantId: string }
  | { type: 'CLEAR' }

const initialState: State = { items: [] }

function reducer(state: State, action: Action): State {
  switch (action.type) {
    case 'ADD': {
      const idx = state.items.findIndex(
        i => i.item.id === action.item.id && i.restaurantId === action.restaurantId
      )
      if (idx >= 0) {
        const items = [...state.items]
        items[idx] = { ...items[idx], qty: items[idx].qty + 1 }
        return { items }
      }
      return { items: [...state.items, { item: action.item, qty: 1, restaurantId: action.restaurantId }] }
    }
    case 'INCREMENT': {
      return {
        items: state.items.map(i =>
          i.item.id === action.itemId && i.restaurantId === action.restaurantId ? { ...i, qty: i.qty + 1 } : i
        )
      }
    }
    case 'DECREMENT': {
      return {
        items: state.items
          .map(i =>
            i.item.id === action.itemId && i.restaurantId === action.restaurantId ? { ...i, qty: i.qty - 1 } : i
          )
          .filter(i => i.qty > 0)
      }
    }
    case 'REMOVE': {
      return { items: state.items.filter(i => !(i.item.id === action.itemId && i.restaurantId === action.restaurantId)) }
    }
    case 'CLEAR': {
      return { items: [] }
    }
    default:
      return state
  }
}

const CartStateContext = createContext<State | undefined>(undefined)
const CartDispatchContext = createContext<React.Dispatch<Action> | undefined>(undefined)

export const CartProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(reducer, initialState)
  return (
    <CartStateContext.Provider value={state}>
      <CartDispatchContext.Provider value={dispatch}>{children}</CartDispatchContext.Provider>
    </CartStateContext.Provider>
  )
}

export function useCart() {
  const s = useContext(CartStateContext)
  if (!s) throw new Error('useCart must be used within CartProvider')
  return s
}

export function useCartDispatch() {
  const d = useContext(CartDispatchContext)
  if (!d) throw new Error('useCartDispatch must be used within CartProvider')
  return d
}
