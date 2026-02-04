using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Rental
{
    public class RentalDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid CarId { get; set; }
        public string CarModel { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string? CarImageUrl { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string ReturnLocation { get; set; } = string.Empty;
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public int TotalDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
