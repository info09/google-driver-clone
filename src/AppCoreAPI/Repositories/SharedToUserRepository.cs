using AppCoreAPI.Data;
using AppCoreAPI.Data.Entities;
using AppCoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppCoreAPI.Repositories
{
    public class SharedToUserRepository : ISharedToUserRepository
    {
        private readonly ApplicationDbContext _context;

        public SharedToUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(SharedToUser entity)
        {
            _context.SharedToUsers.Add(entity);
        }

        public async Task<SharedToUser> GetSharedToUserById(int id)
        {
            return await _context.SharedToUsers.FindAsync(id);
        }

        public async Task<SharedToUser> GetSharedToUserByUrl(string url, string username)
        {
            //dung FirstOrDefaultAsync vi tra ve co the la 1 list. giong nhau chi khac moi SharedUsername
            return await _context.SharedToUsers.FirstOrDefaultAsync(x => (x.Url == url && x.SharedUserName == username) || (x.Url == url && x.OwnerUserName == username));
        }
    }
}
