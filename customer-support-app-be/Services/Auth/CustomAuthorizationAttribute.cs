using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace customer_support_app.API.Services.Auth
{
    public class CustomAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        // Constructor: Rolleri alıyoruz
        public CustomAuthorizationAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Kullanıcı kimliği doğrulanmış mı?
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Kullanıcının JWT token'ında roller olup olmadığını kontrol et
            var userRoles = context.HttpContext.User.Claims
                                .Where(c => c.Type == "Role") // Burada "Role" claim'ini kullanıyoruz
                                .Select(c => c.Value)
                                .ToList();

            // Eğer roller tanımlıysa, kullanıcının rolü bu rollerden biri olmalı
            if (_roles.Any() && !_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
