using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Rental
{

    public class UpdateRentalStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime? ActualReturnDate { get; set; }
    }
}
