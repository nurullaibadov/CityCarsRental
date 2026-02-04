using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Payment
{
    public class ProcessPaymentDto
    {
        public Guid RentalId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
    }
}
