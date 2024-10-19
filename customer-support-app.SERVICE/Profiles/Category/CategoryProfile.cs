using AutoMapper;
using customer_support_app.CORE.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.Category
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<CORE.DBModels.Category, CategoryViewModel>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name))
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id))
                .ReverseMap();
        }
    }
}
