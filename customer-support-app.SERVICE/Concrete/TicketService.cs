using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;

namespace customer_support_app.SERVICE.Concrete
{
    public class TicketService : ITicketService
    {
        private readonly ITicketDal _ticketDal;
        private readonly IMapper _mapper;
        public TicketService(ITicketDal ticketDal,IMapper mapper)
        {
            _ticketDal = ticketDal;
            _mapper = mapper;
        }
        public async Task<IDataResult<List<TicketViewModel>>> GetTicketsOfUser(int id)
        {
            try
            {
                var tickets = await _ticketDal.GetTicketsOfUser(id);

                if(!String.IsNullOrEmpty(tickets.Message))
                {
                    return new ErrorDataResult<List<TicketViewModel>>(tickets.Message,tickets.Code);
                }

                return new SuccessDataResult<List<TicketViewModel>>(_mapper.Map<List<TicketViewModel>>(tickets.Data),StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<TicketViewModel>>("Something went wrong. Please check the application logs.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> CreateTicket(CreateTicketRequestModel model)
        {
            try
            {
                var result = await _ticketDal.AddAsync(_mapper.Map<Ticket>(model));

                if (result == null)
                {
                    return new ErrorResult("Something went wrong while creating entity.",StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult("Entity created successfully.",StatusCodes.Status201Created);
            }
            catch(Exception ex)
            {
                return new ErrorResult("Error occured while creating entity.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<TicketViewModel>> UpdateTicket(UpdateTicketRequestModel model)
        {
            try
            {
                var result = await _ticketDal.UpdateTicket(model);

                if (!String.IsNullOrEmpty(result.Message))
                {
                    return new ErrorDataResult<TicketViewModel>(result.Message, result.Code);
                }

                return new SuccessDataResult<TicketViewModel>(_mapper.Map<TicketViewModel>(result.Data), result.Code);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        } 
        public async Task<IDataResult<TicketViewModel>> GetTicketById(int id)
        {
            try
            {
                var response = await _ticketDal.GetTickedById(id);

                if(!String.IsNullOrEmpty(response.Message))
                {
                    return new ErrorDataResult<TicketViewModel>(response.Message, response.Code);
                }

                return new SuccessDataResult<TicketViewModel>(_mapper.Map<TicketViewModel>(response.Data),response.Code);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong while fetching data. Please check the logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> DeleteTicket(int id)
        {
            try
            {
                var isTicketExist = await _ticketDal.GetTickedById(id);

                if(!String.IsNullOrEmpty(isTicketExist.Message))
                {
                    return new ErrorResult(isTicketExist.Message, isTicketExist.Code);
                }

                await _ticketDal.DeleteAsync(isTicketExist.Data, "Dummy",false);

                return new SuccessResult("Deleted successfully.",StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
