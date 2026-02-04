using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentDto>> ProcessPaymentAsync(Guid userId, ProcessPaymentDto dto);
        Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(Guid id);
        Task<ApiResponse<List<PaymentDto>>> GetUserPaymentsAsync(Guid userId);
    }
}
