using AutoMapper;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Profiles.Comment
{
    public class CommentProfile:Profile
    {
        public CommentProfile()
        {
            CreateMap<AddCommentToTicketRequestModel, CORE.DBModels.Comment>()
                .ForMember(dest => dest.Message, src => src.MapFrom(rm => rm.Message))
                .ForMember(dest => dest.CreatorId, src => src.MapFrom(rm => rm.CreatorId))
                .ForMember(dest => dest.TicketId, src => src.MapFrom(rm => rm.TicketId));

            CreateMap<UpdateCommentRequestModel, CORE.DBModels.Comment>()
                .ForMember(dest => dest.Id, src => src.MapFrom(rm => rm.Id))
                .ForMember(dest => dest.Message, src => src.MapFrom(rm => rm.Message));

            CreateMap<CORE.DBModels.Comment,CommentViewModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(rm => rm.Id))
                .ForMember(dest => dest.Message, src => src.MapFrom(rm => rm.Message))
                .ForMember(dest => dest.CreatedAt, src => src.MapFrom(rm => rm.CreatedAt))
                .ForMember(dest => dest.Creator, src => src.MapFrom(rm => rm.Creator));
        }
    }
}
