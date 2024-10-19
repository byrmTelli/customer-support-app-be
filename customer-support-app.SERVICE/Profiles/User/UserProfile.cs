using AutoMapper;
using customer_support_app.CORE.DBModels;
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
        }
    }
}
