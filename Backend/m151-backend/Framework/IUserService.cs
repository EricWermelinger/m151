using m151_backend.Entities;

namespace m151_backend.Framework
{
    public interface IUserService
    {
        public Task<User?> GetUser();
        public Guid? GetUserGuid();
    }
}
