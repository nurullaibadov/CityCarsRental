using Cityrental.Application.DTOs.Car;
using Cityrental.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface ICarService
    {
        Task<ApiResponse<PaginatedResult<CarDto>>> GetCarsAsync(CarFilterDto filter, PaginationDto pagination);
        Task<ApiResponse<CarDto>> GetCarByIdAsync(Guid id);
        Task<ApiResponse<CarDto>> CreateCarAsync(CreateCarDto dto);
        Task<ApiResponse<CarDto>> UpdateCarAsync(Guid id, UpdateCarDto dto);
        Task<ApiResponse<bool>> DeleteCarAsync(Guid id);
        Task<ApiResponse<List<CarDto>>> GetAvailableCarsAsync(DateTime pickupDate, DateTime returnDate);
    }
}
