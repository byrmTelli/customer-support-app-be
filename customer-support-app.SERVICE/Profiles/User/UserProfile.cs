using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.User
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserViewModel>()
                .ForMember(dest => dest.FullName, src => src.MapFrom(x => $"{x.Name} {x.Surname}"));

            CreateMap<RegisterUserRequestModel, AppUser>()
                .ForMember(dest => dest.UserName, src => src.MapFrom(x => x.Username))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
                .ForMember(dest => dest.Surname, src => src.MapFrom(x => x.Surname))
                .ForMember(dest => dest.Email, src => src.MapFrom(x => x.Email))
                .ForMember(dest => dest.Adress,src => src.MapFrom(x => x.Address))
                .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.PhoneNumber));
        }
    }
}
