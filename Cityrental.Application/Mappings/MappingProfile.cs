using AutoMapper;
using Cityrental.Application.DTOs.Auth;
using Cityrental.Application.DTOs.Car;
using Cityrental.Application.DTOs.Payment;
using Cityrental.Application.DTOs.Rental;
using Cityrental.Application.DTOs.Review;
using Cityrental.Application.DTOs.User;
using Cityrental.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cityrental.Application.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // Car Mappings
            CreateMap<Car, CarDto>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.TransmissionType, opt => opt.MapFrom(src => src.TransmissionType.ToString()))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType.ToString()))
                .ForMember(dest => dest.DriveType, opt => opt.MapFrom(src => src.DriveType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => ParseJsonArray(src.Features)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => ParseJsonArray(src.ImageUrls)))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<CreateCarDto, Car>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => SerializeToJson(src.Features)))
                .ForMember(dest => dest.TransmissionType, opt => opt.MapFrom(src => Enum.Parse<Domain.Enums.TransmissionType>(src.TransmissionType)))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => Enum.Parse<Domain.Enums.FuelType>(src.FuelType)))
                .ForMember(dest => dest.DriveType, opt => opt.MapFrom(src => Enum.Parse<Domain.Enums.DriveType>(src.DriveType)));

            // Rental Mappings
            CreateMap<Rental, RentalDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.CarBrand, opt => opt.MapFrom(src => src.Car.Brand.Name))
                .ForMember(dest => dest.CarImageUrl, opt => opt.MapFrom(src => src.Car.MainImageUrl))
                .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.PickupLocation.Name))
                .ForMember(dest => dest.ReturnLocation, opt => opt.MapFrom(src => src.ReturnLocation.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateRentalDto, Rental>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.DailyRate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDays, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.DepositAmount, opt => opt.Ignore());

            // Payment Mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

            // Review Mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfileImageUrl));

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CarId, opt => opt.Ignore());
        }

        private static List<string> ParseJsonArray(string? json)
        {
            if (string.IsNullOrEmpty(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string? SerializeToJson(List<string>? list)
        {
            if (list == null || !list.Any()) return null;
            return JsonSerializer.Serialize(list);
        }
    }
}
