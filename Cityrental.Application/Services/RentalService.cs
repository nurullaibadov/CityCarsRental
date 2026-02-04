using AutoMapper;
using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.Rental;
using Cityrental.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IRepository<Car> _carRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RentalService(
            IRepository<Rental> rentalRepository,
            IRepository<Car> carRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RentalDto>> CreateRentalAsync(Guid userId, CreateRentalDto dto)
        {
            // Check if car exists and is available
            var car = await _carRepository.GetByIdAsync(dto.CarId);
            if (car == null)
            {
                return ApiResponse<RentalDto>.FailureResponse("Car not found");
            }

            if (!car.IsAvailableForRental())
            {
                throw new CarNotAvailableException(car.Id);
            }

            // Create rental
            var rental = _mapper.Map<Rental>(dto);
            rental.UserId = userId;
            rental.DailyRate = car.DailyRate;
            rental.CalculateTotalAmount();
            rental.DepositAmount = rental.TotalAmount * 0.2m; // 20% deposit
            rental.Status = RentalStatus.Pending;

            await _rentalRepository.AddAsync(rental);

            // Update car status
            car.Status = CarStatus.Rented;
            _carRepository.Update(car);

            await _unitOfWork.SaveChangesAsync();

            var rentalDto = _mapper.Map<RentalDto>(rental);
            return ApiResponse<RentalDto>.SuccessResponse(rentalDto, "Rental created successfully");
        }

        public async Task<ApiResponse<RentalDto>> GetRentalByIdAsync(Guid id)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
            {
                return ApiResponse<RentalDto>.FailureResponse("Rental not found");
            }

            var rentalDto = _mapper.Map<RentalDto>(rental);
            return ApiResponse<RentalDto>.SuccessResponse(rentalDto);
        }

        public async Task<ApiResponse<PaginatedResult<RentalDto>>> GetUserRentalsAsync(
            Guid userId,
            PaginationDto pagination)
        {
            var query = (await _rentalRepository.FindAsync(r => r.UserId == userId))
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = query.Count();
            var items = query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToList();

            var rentalDtos = _mapper.Map<List<RentalDto>>(items);

            var result = new PaginatedResult<RentalDto>
            {
                Items = rentalDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return ApiResponse<PaginatedResult<RentalDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<PaginatedResult<RentalDto>>> GetAllRentalsAsync(
            PaginationDto pagination)
        {
            var allRentals = (await _rentalRepository.GetAllAsync())
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = allRentals.Count();
            var items = allRentals
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToList();

            var rentalDtos = _mapper.Map<List<RentalDto>>(items);

            var result = new PaginatedResult<RentalDto>
            {
                Items = rentalDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return ApiResponse<PaginatedResult<RentalDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<RentalDto>> UpdateRentalStatusAsync(
            Guid id,
            UpdateRentalStatusDto dto)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
            {
                return ApiResponse<RentalDto>.FailureResponse("Rental not found");
            }

            var newStatus = Enum.Parse<RentalStatus>(dto.Status);
            rental.Status = newStatus;

            if (newStatus == RentalStatus.Completed && dto.ActualReturnDate.HasValue)
            {
                rental.ActualReturnDate = dto.ActualReturnDate.Value;

                // Make car available again
                var car = await _carRepository.GetByIdAsync(rental.CarId);
                if (car != null)
                {
                    car.Status = CarStatus.Available;
                    _carRepository.Update(car);
                }
            }

            _rentalRepository.Update(rental);
            await _unitOfWork.SaveChangesAsync();

            var rentalDto = _mapper.Map<RentalDto>(rental);
            return ApiResponse<RentalDto>.SuccessResponse(rentalDto, "Rental status updated");
        }

        public async Task<ApiResponse<bool>> CancelRentalAsync(Guid id, Guid userId)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
            {
                return ApiResponse<bool>.FailureResponse("Rental not found");
            }

            if (rental.UserId != userId)
            {
                return ApiResponse<bool>.FailureResponse("Unauthorized");
            }

            if (rental.Status != RentalStatus.Pending && rental.Status != RentalStatus.Confirmed)
            {
                return ApiResponse<bool>.FailureResponse("Cannot cancel rental in current status");
            }

            rental.Status = RentalStatus.Cancelled;
            _rentalRepository.Update(rental);

            // Make car available again
            var car = await _carRepository.GetByIdAsync(rental.CarId);
            if (car != null)
            {
                car.Status = CarStatus.Available;
                _carRepository.Update(car);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Rental cancelled successfully");
        }
    }
}
