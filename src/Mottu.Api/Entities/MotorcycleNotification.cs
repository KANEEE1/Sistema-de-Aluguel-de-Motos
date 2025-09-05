using System;

namespace Mottu.Api.Entities
{
    public class MotorcycleNotification
    {
        public Guid Id { get; set; }
        public Guid MotorcycleId { get; set; }
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Plate { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string NotificationType { get; set; } = string.Empty;
    }
}
