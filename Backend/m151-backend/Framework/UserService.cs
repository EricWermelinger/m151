using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace m151_backend.Framework
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AuthorizationM151 _authorization = new();

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserGuid()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            string guid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return guid != null ? Guid.Parse(guid) : null;
        }
    }
}
