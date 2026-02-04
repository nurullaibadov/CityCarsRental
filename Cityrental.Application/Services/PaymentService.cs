using AutoMapper;
using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.Payment;
using Cityrental.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(
            IRepository<Payment> paymentRepository,
            IRepository<Rental> rentalRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _rentalRepository = rentalRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaymentDto>> ProcessPaymentAsync(
            Guid userId,
            ProcessPaymentDto dto)
        {
            var rental = await _rentalRepository.GetByIdAsync(dto.RentalId);
            if (rental == null)
            {
                return ApiResponse<PaymentDto>.FailureResponse("Rental not found");
            }

            if (rental.UserId != userId)
            {
                return ApiResponse<PaymentDto>.FailureResponse("Unauthorized");
            }

            // Check if payment already exists
            var existingPayment = await _paymentRepository
                .FirstOrDefaultAsync(p => p.RentalId == dto.RentalId);

            if (existingPayment != null)
            {
                return ApiResponse<PaymentDto>.FailureResponse("Payment already processed");
            }

            // Create payment
            var payment = new Payment
            {
                RentalId = rental.Id,
                UserId = userId,
                Amount = rental.TotalAmount,
                PaymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod),
                PaymentStatus = PaymentStatus.Pending
            };

            // Simulate payment processing (replace with real payment gateway)
            var paymentSuccessful = await SimulatePaymentGateway(dto);

            if (paymentSuccessful)
            {
                payment.MarkAsCompleted(Guid.NewGuid().ToString()); // Transaction ID
                rental.Status = RentalStatus.Confirmed;
            }
            else
            {
                payment.MarkAsFailed();
            }

            await _paymentRepository.AddAsync(payment);
            _rentalRepository.Update(rental);
            await _unitOfWork.SaveChangesAsync();

            var paymentDto = _mapper.Map<PaymentDto>(payment);

            return paymentSuccessful
                ? ApiResponse<PaymentDto>.SuccessResponse(paymentDto, "Payment processed successfully")
                : ApiResponse<PaymentDto>.FailureResponse("Payment failed");
        }

        public async Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return ApiResponse<PaymentDto>.FailureResponse("Payment not found");
            }

            var paymentDto = _mapper.Map<PaymentDto>(payment);
            return ApiResponse<PaymentDto>.SuccessResponse(paymentDto);
        }

        public async Task<ApiResponse<List<PaymentDto>>> GetUserPaymentsAsync(Guid userId)
        {
            var payments = (await _paymentRepository.FindAsync(p => p.UserId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
            return ApiResponse<List<PaymentDto>>.SuccessResponse(paymentDtos);
        }

        private async Task<bool> SimulatePaymentGateway(ProcessPaymentDto dto)
        {
            // Simulate payment processing delay
            await Task.Delay(1000);

            // In production, integrate with real payment gateway:
            // - Stripe
            // - PayPal
            // - Local payment provider

            // For now, always return success
            return true;
        }
    }
}
