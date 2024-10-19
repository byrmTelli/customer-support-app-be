using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Enums;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.ViewModels.Ticket;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.Ticket
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<CORE.DBModels.Ticket, TicketViewModel>()
                .ForMember(dest => dest.Title, src => src.MapFrom(x => x.Title))
                .ForMember(dest => dest.Content, src => src.MapFrom(x => x.Content))
                .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status))
                .ForMember(dest => dest.Creator, src => src.MapFrom(x => x.Creator))
                .ForMember(dest => dest.CreatedAt, src => src.MapFrom(x => x.CreatedAt))
                .ReverseMap();

            CreateMap<CreateTicketRequestModel, CORE.DBModels.Ticket>()
                .ForMember(dest => dest.Title, src => src.MapFrom(x => x.Title))
                .ForMember(dest => dest.Content, src => src.MapFrom(x => x.Content))
                .ForMember(dest => dest.CreatorId, src => src.MapFrom(x => x.CreatorId))
                .ForMember(dest => dest.CategoryId, src => src.MapFrom(x => x.CategoryId));

        }
    }
}
