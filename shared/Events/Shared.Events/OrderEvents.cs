namespace Shared.Events{
  public record OrderPlacedEvent(
    Guid OrderId,
    int UserId,
    decimal Amount,
    DateTime PlacedAt
  );
}