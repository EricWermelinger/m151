using System.Security.Claims;

namespace m151_backend.Framework
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserGuid()
        {
            string guid = null;
            if (_httpContextAccessor.HttpContext != null)
            {
                guid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            return guid != null ? Guid.Parse(guid) : null;
        }
    }
}
