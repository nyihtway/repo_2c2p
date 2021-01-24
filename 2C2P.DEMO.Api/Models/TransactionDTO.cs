using _2C2P.DEMO.Domain.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using AutoMapper;
using Newtonsoft.Json;

namespace _2C2P.DEMO.Api.Models
{
    public class TransactionDTO : IMapFrom<Transaction>
    {
        public string Id { get; set; }
        public string Payment { get; set; }
        public string Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest=> dest.Id, act=> act.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Payment, act => act.MapFrom(src => string.Concat(src.Amount, " " , src.CurrencyCode)))
                .ForAllMembers(opt => opt.Condition((source, dest, sourceMember, destMember) => (sourceMember != null)));
        }
    }
}
