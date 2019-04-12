using AutoMapper;
using OnePay_Backend.DTO;
using OnePay_Backend.Models;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<OnePayTransaction, TransactionDTO>();
            CreateMap<TransactionDTO, OnePayTransaction>();
            CreateMap<BusinessProductDTO, BusinessProduct>();
            CreateMap<BusinessProduct, BusinessProductDTO>();
            CreateMap<UserAccountDTO, UserAccount>();
            CreateMap<UserAccount, UserAccountDTO>();
        }
    }
}