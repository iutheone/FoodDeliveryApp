namespace Shared.Events{
  public record OrderPlacedEvent(
    int OrderId,
    int Amount,
    DateTime PlacedAt
  );


  // public class MenuItem{
  //       public int Id{get;set;} 
  //       public string Name{get;set;}
  //       public string Description{get;set;}
  //       public int Price{get;set;}
  //       public string Image{get;set;}
  //       public bool Veg{get;set;}
  //       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  // }   
}