using AutoMapper;
using Cityrental.Application.DTOs.Car;
using Cityrental.Application.DTOs.Common;
using Cityrental.Application.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Services
{
    public class CarService : ICarService
    {
        private readonly IRepository<Car> _carRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CarService(
            IRepository<Car> carRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _carRepository = carRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResult<CarDto>>> GetCarsAsync(
            CarFilterDto filter,
            PaginationDto pagination)
        {
            var query = ((DbSet<Car>)_carRepository.GetAllAsync().Result.GetType())
                .AsQueryable()
                .Include(c => c.Brand)
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .Where(c => c.IsActive);

            // Apply filters
            if (filter.BrandId.HasValue)
                query = query.Where(c => c.BrandId == filter.BrandId.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(c => c.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrEmpty(filter.TransmissionType))
                query = query.Where(c => c.TransmissionType.ToString() == filter.TransmissionType);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(c => c.FuelType.ToString() == filter.FuelType);

            if (filter.MinPrice.HasValue)
                query = query.Where(c => c.DailyRate >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(c => c.DailyRate <= filter.MaxPrice.Value);

            if (filter.MinSeats.HasValue)
                query = query.Where(c => c.Seats >= filter.MinSeats.Value);

            if (filter.MinYear.HasValue)
                query = query.Where(c => c.Year >= filter.MinYear.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(c =>
                    c.Model.ToLower().Contains(searchTerm) ||
                    c.Brand.Name.ToLower().Contains(searchTerm) ||
                    c.Category.Name.ToLower().Contains(searchTerm));
            }

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortDescending
                    ? query.OrderByDescending(c => c.DailyRate)
                    : query.OrderBy(c => c.DailyRate),
                "year" => filter.SortDescending
                    ? query.OrderByDescending(c => c.Year)
                    : query.OrderBy(c => c.Year),
                "rating" => filter.SortDescending
                    ? query.OrderByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0)
                    : query.OrderBy(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var carDtos = _mapper.Map<List<CarDto>>(items);

            var result = new PaginatedResult<CarDto>
            {
                Items = carDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return ApiResponse<PaginatedResult<CarDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<CarDto>> GetCarByIdAsync(Guid id)
        {
            var car = await _carRepository
                .FindAsync(c => c.Id == id)
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (car == null)
            {
                return ApiResponse<CarDto>.FailureResponse("Car not found");
            }

            var carDto = _mapper.Map<CarDto>(car);
            return ApiResponse<CarDto>.SuccessResponse(carDto);
        }

        public async Task<ApiResponse<CarDto>> CreateCarAsync(CreateCarDto dto)
        {
            // Validate license plate uniqueness
            var exists = await _carRepository.ExistsAsync(c => c.LicensePlate == dto.LicensePlate);
            if (exists)
            {
                return ApiResponse<CarDto>.FailureResponse("License plate already exists");
            }

            var car = _mapper.Map<Car>(dto);
            car.Status = CarStatus.Available;
            car.IsActive = true;

            await _carRepository.AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            var carDto = _mapper.Map<CarDto>(car);
            return ApiResponse<CarDto>.SuccessResponse(carDto, "Car created successfully");
        }

        public async Task<ApiResponse<CarDto>> UpdateCarAsync(Guid id, UpdateCarDto dto)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return ApiResponse<CarDto>.FailureResponse("Car not found");
            }

            // Update properties
            car.BrandId = dto.BrandId;
            car.CategoryId = dto.CategoryId;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.Color = dto.Color;
            car.TransmissionType = Enum.Parse<TransmissionType>(dto.TransmissionType);
            car.FuelType = Enum.Parse<FuelType>(dto.FuelType);
            car.DriveType = Enum.Parse<DriveType>(dto.DriveType);
            car.Seats = dto.Seats;
            car.Doors = dto.Doors;
            car.DailyRate = dto.DailyRate;
            car.Mileage = dto.Mileage;
            car.Status = Enum.Parse<CarStatus>(dto.Status);
            car.Description = dto.Description;

            _carRepository.Update(car);
            await _unitOfWork.SaveChangesAsync();

            var carDto = _mapper.Map<CarDto>(car);
            return ApiResponse<CarDto>.SuccessResponse(carDto, "Car updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteCarAsync(Guid id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return ApiResponse<bool>.FailureResponse("Car not found");
            }

            car.IsActive = false;
            _carRepository.Update(car);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Car deleted successfully");
        }

        public async Task<ApiResponse<List<CarDto>>> GetAvailableCarsAsync(
            DateTime pickupDate,
            DateTime returnDate)
        {
            var cars = await _carRepository
                .FindAsync(c => c.Status == CarStatus.Available && c.IsActive);

            var carDtos = _mapper.Map<List<CarDto>>(cars);
            return ApiResponse<List<CarDto>>.SuccessResponse(carDtos);
        }
    }
}
