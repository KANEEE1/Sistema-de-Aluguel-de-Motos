namespace RentalSystem.Api.Contracts
{
    public record MotorcycleCreatedEvent
    {
        public Guid Id { get; init; }
        public int Year { get; init; }
        public string Model { get; init; } = string.Empty;
        public string Plate { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }
}