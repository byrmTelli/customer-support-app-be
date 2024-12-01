using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace customer_support_app.SERVICE.Authorization
{
    public class UserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string UserID()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst("UserID")?.Value ?? "";
        }
        
        public string UserName()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst("Username")?.Value ?? "";
        }

        public string Role()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst("Role")?.Value ?? "";
        }

        public string Email()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst("Email")?.Value ?? "";
        }
    }
}