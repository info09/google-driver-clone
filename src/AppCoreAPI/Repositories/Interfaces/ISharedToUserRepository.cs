using AppCoreAPI.Data.Entities;

namespace AppCoreAPI.Repositories.Interfaces
{
    public interface ISharedToUserRepository
    {
        void Add(SharedToUser entity);
        Task<SharedToUser> GetSharedToUserById(int id);
        Task<SharedToUser> GetSharedToUserByUrl(string url, string username);
    }
}
