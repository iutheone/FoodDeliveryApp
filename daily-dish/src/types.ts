export type iMenuItem = {
  id: string,
  name: string,
  description?: string,
  price: number,
  image?: string,
  veg?: boolean
}


export type Restaurant ={
  id: string,
  name: string,
  cuisine: string,
  rating: number,
  image?: string,
  eta : string,
  location: string,
  menu : MenuItem[]
}


export type CartItem = {
  item: MenuItem
  qty: number
  restaurantId: string
}