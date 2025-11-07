import { useMemo } from 'react';
import restaurants from '../data/mock';
import RestaurantCard from '../components/RestaurantCard';

function Home({query}:{query: string}) {
  const filtered = useMemo(()=>{
    const q= query.trim().toLowerCase();
    if(!q)return restaurants;
    return restaurants.filter(t=> t.name.toLowerCase().includes(q) || t.cuisine.toLowerCase().includes(q) || t.menu.some(m=> m.name.toLowerCase().includes(q)));

  },[query])
  return (
    <div className='app'>
      <h2>Popular restaurants</h2>
      <div className="list">
       {filtered.map(r=> <RestaurantCard r={r}/>)}
        {filtered.length == 0 && <div className='empty'>No restaurat found</div>}
      </div>
    </div>
  )
}

export default Home
