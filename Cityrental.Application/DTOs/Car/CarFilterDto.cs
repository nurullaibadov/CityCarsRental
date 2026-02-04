using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Car
{
    public class CarFilterDto
    {
        public Guid? BrandId { get; set; }
        public Guid? CategoryId { get; set; }
        public string? TransmissionType { get; set; }
        public string? FuelType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinSeats { get; set; }
        public int? MinYear { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } // "price", "year", "rating"
        public bool SortDescending { get; set; }
    }
}
