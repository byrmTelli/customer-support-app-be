using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.Role
{
    public class RoleProfile:Profile
    {
        public RoleProfile()
        {
            CreateMap<AppRole, RoleViewModel>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name));
        }
    }
}
