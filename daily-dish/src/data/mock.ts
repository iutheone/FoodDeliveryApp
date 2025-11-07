import type { Restaurant } from '../types'

const restaurants: Restaurant[] = [
  {
    id: 'r1',
    name: "Mama's Kitchen",
    cuisine: 'Indian',
    rating: 4.5,
    eta: '25-35 min',
    image: '/src/assets/Res1.jpeg',
    menu: [
      { id: 'm1', name: 'Butter Chicken', price: 9.5, description: 'Creamy tomato gravy', veg: false },
      { id: 'm2', name: 'Paneer Tikka', price: 7.0, description: 'Smoky grilled paneer', veg: true },
      { id: 'm3', name: 'Naan', price: 1.5 }
    ]
  },
  {
    id: 'r2',
    name: 'Pasta House',
    cuisine: 'Italian',
    rating: 4.3,
    eta: '20-30 min',
    image: '/src/assets/Res2.jpeg',
    menu: [
      { id: 'm4', name: 'Spaghetti Carbonara', price: 11.5, description: 'Classic carbonara' },
      { id: 'm5', name: 'Margherita Pizza', price: 10.0, description: 'Fresh basil & mozzarella' }
    ]
  },
  {
    id: 'r3',
    name: 'Green Bowl',
    cuisine: 'Healthy',
    rating: 4.7,
    eta: '15-25 min',
    image: '/src/assets/Res3.jpeg',
    menu: [
      { id: 'm6', name: 'Quinoa Salad', price: 8.0, description: 'Mixed greens' },
      { id: 'm7', name: 'Smoothie Bowl', price: 6.5 }
    ]
  }
]

export default restaurants
