using AutoMapper;
using Cityrental.Application.DTOs.Auth;
using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.User;
using Cityrental.Application.Interfaces;
using Cityrental.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            JwtTokenGenerator jwtTokenGenerator,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // Find user by email
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password");
            }

            // Verify password
            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Account is deactivated");
            }

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            // Update user's refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = _mapper.Map<UserDto>(user)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            // Check if email already exists
            var emailExists = await _userRepository.ExistsAsync(u => u.Email == request.Email);
            if (emailExists)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Email already registered");
            }

            // Check if driver license already exists
            var licenseExists = await _userRepository.ExistsAsync(u => u.DriverLicenseNumber == request.DriverLicenseNumber);
            if (licenseExists)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Driver license already registered");
            }

            // Create new user
            var user = _mapper.Map<User>(request);
            user.PasswordHash = PasswordHasher.HashPassword(request.Password);
            user.Role = UserRole.User;
            user.IsEmailConfirmed = false;
            user.IsActive = true;

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = _mapper.Map<UserDto>(user)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful");
        }

        public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Refresh token expired");
            }

            // Generate new tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = _mapper.Map<UserDto>(user)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }

        public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
        }
    }
}
