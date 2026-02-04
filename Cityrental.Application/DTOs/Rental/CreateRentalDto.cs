using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Rental
{
    public class CreateRentalDto
    {
        public Guid CarId { get; set; }
        public Guid PickupLocationId { get; set; }
        public Guid ReturnLocationId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? Notes { get; set; }
    }
}
