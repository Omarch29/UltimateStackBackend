using AutoMapper;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;

namespace REPM.Application.Mapers;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        // Aggregates
        CreateMap<User, UserDto>();
        CreateMap<Lease, LeaseDto>();
        CreateMap<Payment, PaymentDto>();
        CreateMap<Property, PropertyDto>();
        CreateMap<Property, PropertyWithLeasesDto>();
        
        // Value Objects    
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<Money, MoneyDto>().ReverseMap();
        CreateMap<DateRange, DateRangeDto>().ReverseMap();
        
        // Update DTO
        CreateMap<UserforUpdateDto, User>();
    }
}