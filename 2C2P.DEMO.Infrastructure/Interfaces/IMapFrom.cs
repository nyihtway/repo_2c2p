using AutoMapper;

namespace _2C2P.DEMO.Infrastructure.Interfaces
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
