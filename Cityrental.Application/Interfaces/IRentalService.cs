using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface IRentalService
    {
        Task<ApiResponse<RentalDto>> CreateRentalAsync(Guid userId, CreateRentalDto dto);
        Task<ApiResponse<RentalDto>> GetRentalByIdAsync(Guid id);
        Task<ApiResponse<PaginatedResult<RentalDto>>> GetUserRentalsAsync(Guid userId, PaginationDto pagination);
        Task<ApiResponse<PaginatedResult<RentalDto>>> GetAllRentalsAsync(PaginationDto pagination);
        Task<ApiResponse<RentalDto>> UpdateRentalStatusAsync(Guid id, UpdateRentalStatusDto dto);
        Task<ApiResponse<bool>> CancelRentalAsync(Guid id, Guid userId);
    }
}
