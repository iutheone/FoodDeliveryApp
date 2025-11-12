using System;

namespace SharedEvents.Models
{
    public class Order
    {
        public int OrderId {get;set;}
        public string Status { get; set; } = "Pending";
        public int RestaurantId{get;set;}
        public int Amount{get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderMenuMapping{
        public int Id{get;set;}
        public int MenuId{get;set;}
        public int OrderId{get;set;}
    }

    public class Restaurant 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Cuisine { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Image { get; set; } = string.Empty;
        public int Eta { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class RestaurantMenuMapping
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int MenuId { get; set; }
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool Veg { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}