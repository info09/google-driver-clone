using AppCoreAPI.Data;
using AppCoreAPI.Data.Entities;
using AppCoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppCoreAPI.Repositories
{
    public class RootFolderRepository : IRootFolderRepository
    {
        private readonly ApplicationDbContext _context;

        public RootFolderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(RootFolder rootFolder)
        {
            _context.RootFolders.Add(rootFolder);
        }

        public async Task<RootFolder> GetRootFolder(int id)
        {
            return await _context.RootFolders.FindAsync(id);
        }

        public async Task<RootFolder> GetRootFolderByUserId(int userId)
        {
            return await _context.RootFolders.FirstOrDefaultAsync(i => i.UserId == userId);
        }
    }
}
