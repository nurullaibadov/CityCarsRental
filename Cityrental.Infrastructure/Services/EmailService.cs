using Cityrental.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // TODO: Implement with MailKit or SendGrid
            await Task.CompletedTask;
        }

        public async Task SendWelcomeEmailAsync(string to, string firstName)
        {
            var subject = "Welcome to CityCar Azerbaijan!";
            var body = $"Hello {firstName},\n\nWelcome to CityCar Azerbaijan!";
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendRentalConfirmationEmailAsync(string to, string firstName, Guid rentalId)
        {
            var subject = "Rental Confirmation";
            var body = $"Hello {firstName},\n\nYour rental (ID: {rentalId}) has been confirmed!";
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPaymentReceiptEmailAsync(string to, string firstName, decimal amount)
        {
            var subject = "Payment Receipt";
            var body = $"Hello {firstName},\n\nPayment of ${amount} received successfully!";
            await SendEmailAsync(to, subject, body);
        }
    }
}
