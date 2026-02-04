using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetProfileAsync(Guid userId);
        Task<ApiResponse<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<ApiResponse<PaginatedResult<UserDto>>> GetAllUsersAsync(PaginationDto pagination);
        Task<ApiResponse<bool>> DeactivateUserAsync(Guid userId);
    }
}
