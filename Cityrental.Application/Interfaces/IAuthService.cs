using Cityrental.Application.DTOs.Auth;
using Cityrental.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse<bool>> LogoutAsync(Guid userId);
    }
}
