namespace Shared.Events{
  public record OrderPlacedEvent(
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    DateTime PlacedAt
  );
}