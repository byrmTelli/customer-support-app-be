using AutoMapper;
using customer_support_app.CORE.ViewModels.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.LogActivity
{
    public class LogActivityProfile:Profile
    {
        public LogActivityProfile()
        {
            CreateMap<CORE.DBModels.ActivityLog, LogActivityViewModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Description))
                .ForMember(dest => dest.CreatedAt, src => src.MapFrom(x => x.CreatedAt));
        }
    }
}
