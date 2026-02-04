using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Car
{
    public class UpdateCarDto
    {
        public Guid BrandId { get; set; }
        public Guid CategoryId { get; set; }
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string TransmissionType { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string DriveType { get; set; } = string.Empty;
        public int Seats { get; set; }
        public int Doors { get; set; }
        public decimal DailyRate { get; set; }
        public int Mileage { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string>? Features { get; set; }
        public string? Description { get; set; }
    }
}
