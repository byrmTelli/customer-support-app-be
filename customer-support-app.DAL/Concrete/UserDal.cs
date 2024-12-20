﻿using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using customer_support_app.CORE.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_support_app.CORE.Constants;
using customer_support_app.CORE.Utilities;

namespace customer_support_app.DAL.Concrete
{
    public class UserDal: EfEntityRepositoryBase<AppUser, AppDbContext>,IUserDal
    {
        private readonly AppDbContext _context;
        public UserDal(AppDbContext context):base(context) 
        {
            _context = context;
        }
        public async Task<IResult> ApproveUser(int id)
        {
            try
            {
                var isUserExist = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (isUserExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }
                isUserExist.IsApproved = true;
                await _context.SaveChangesAsync();

                return new SuccessResult("User approved successfully.", StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorResult("Error occured.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<UserProfileForAdminPanelViewModel>> GetUserProfileForAdminPanelAsync(int id)
        {
            try
            {
                var isUserExistQuery = from userRoles in _context.UserRoles
                                  join user in _context.Users on userRoles.UserId equals user.Id
                                  join role in _context.Roles on userRoles.RoleId equals role.Id
                                       where user.Id == id
                                  select new UserProfileForAdminPanelViewModel
                                  {
                                      Id = user.Id,
                                      Username = user.UserName,
                                      FullName = $"{user.Name} {user.Surname}",
                                      Email = user.Email,
                                      PhoneNumber = user.PhoneNumber,
                                      Adress = user.Adress,
                                      Role = new RoleViewModel
                                      {
                                          Name = role.Name
                                      },
                                      CreatedAt = user.CreatedAt,
                                      ProfileImage = ImageHelper.ConvertImageToBase64String(user.ProfileImage),
                                      IsApproved = user.IsApproved
                                  };

                var isUserExist = await isUserExistQuery.FirstOrDefaultAsync();
                if(isUserExist == null)
                {
                    return new ErrorDataResult<UserProfileForAdminPanelViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                return new SuccessDataResult<UserProfileForAdminPanelViewModel>(isUserExist, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<UserProfileForAdminPanelViewModel>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<CustomerProfileViewModel>>> GetCustomersForAdminPanelAsync()
        {
            try
            {
                var helpdesksWithRolesQuery = from userRoles in _context.UserRoles
                                              join user in _context.Users on userRoles.UserId equals user.Id
                                              join role in _context.Roles on userRoles.RoleId equals role.Id
                                              where role.Name == RoleTypes.Customer
                                              select new CustomerProfileViewModel
                                              {
                                                  Id = user.Id,
                                                  Username = user.UserName,
                                                  FullName = $"{user.Name} {user.Surname}",
                                                  Email = user.Email,
                                                  PhoneNumber = user.PhoneNumber,
                                                  Adress = user.Adress,
                                                  Role = new RoleViewModel
                                                  {
                                                      Name = role.Name
                                                  },
                                                  CreatedAt = user.CreatedAt,
                                                  IsApproved = user.IsApproved ? "Approved": "Not Approved"
                                              };

                var helpdesksWithRoles = await helpdesksWithRolesQuery.ToListAsync();

                return new SuccessDataResult<List<CustomerProfileViewModel>>(helpdesksWithRoles, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerProfileViewModel>>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<UserProfileViewModel>>> GetHelpDesksForAdminPanelAsync()
        {
            try
            {
                var helpdesksWithRolesQuery = from userRoles in _context.UserRoles
                                              join user in _context.Users on userRoles.UserId equals user.Id
                                              join role in _context.Roles on userRoles.RoleId equals role.Id
                                              where role.Name == RoleTypes.Helpdesk
                                              select new UserProfileViewModel 
                                              {
                                                Id = user.Id,
                                                Username = user.UserName,
                                                FullName = $"{user.Name} {user.Surname}",
                                                Email = user.Email,
                                                PhoneNumber = user.PhoneNumber,
                                                Adress = user.Adress,
                                                Role = new RoleViewModel 
                                                { 
                                                    Name = role.Name
                                                }
                                              };

                var helpdesksWithRoles = await helpdesksWithRolesQuery.ToListAsync();  

                return new SuccessDataResult<List<UserProfileViewModel>>(helpdesksWithRoles,StatusCodes.Status200OK);
            }
            catch (Exception ex) 
            {
                return new ErrorDataResult<List<UserProfileViewModel>>("Something went wrong.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<HelpdeskViewModel>>> GetHelpdesksAsync()
        {
            try
            {
                var helpdesksQuery = (from userRoles in _context.UserRoles
                             join user in _context.Users on userRoles.UserId equals user.Id
                             join role in _context.Roles on userRoles.RoleId equals role.Id
                             where role.Name == RoleTypes.Helpdesk
                             select new HelpdeskViewModel
                             {
                                 Id = user.Id,
                                 FullName = $"{user.Name} {user.Surname}",
                                 Role = new RoleViewModel { Name = role.Name}
                             });


                var helpdesks = await helpdesksQuery.ToListAsync();

                return new SuccessDataResult<List<HelpdeskViewModel>>(helpdesks,StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<HelpdeskViewModel>>("Something went wrong please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
