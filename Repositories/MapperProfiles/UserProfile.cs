using Auth.Models;
using AutoMapper;

namespace Auth.Repositories.MapperProfiles {
    public class UserProfile : Profile {
        public UserProfile() {
            CreateMap<User, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((src, dst) => dst.Password = BCrypt.Net.BCrypt.HashPassword(src.Password))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}