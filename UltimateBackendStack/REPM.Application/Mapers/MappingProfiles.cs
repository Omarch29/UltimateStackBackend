using AutoMapper;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;

namespace REPM.Application.Mapers;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<Address, AddressDto>();
        CreateMap<Money, MoneyDto>();
        CreateMap<Lease, LeaseDto>();
        CreateMap<Payment, PaymentDto>();
        CreateMap<Property, PropertyDto>();
        CreateMap<Property, PropertyWithLeasesDto>();
        
    }
}