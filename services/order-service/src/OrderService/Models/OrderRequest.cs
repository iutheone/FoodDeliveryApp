namespace OrderService.Models{
  public class OrderRequest{
    public string CustomerName { get; set; } = string.Empty;
    public string Restaurant { get; set; } = string.Empty;
    public decimal Amount { get; set; }
  }
}