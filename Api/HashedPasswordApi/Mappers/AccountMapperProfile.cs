using AutoMapper;
using HashedPassword.Data;
using HashedPasswordApi.Models;

namespace HashedPasswordApi.Mappers
{
    public class AccountMapperProfile : Profile
    {
        public AccountMapperProfile()
        {
            CreateMap<AccountDto, Account>();
        }
    }
}