using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendWelcomeEmailAsync(string to, string firstName);
        Task SendRentalConfirmationEmailAsync(string to, string firstName, Guid rentalId);
        Task SendPaymentReceiptEmailAsync(string to, string firstName, decimal amount);
    }
}
