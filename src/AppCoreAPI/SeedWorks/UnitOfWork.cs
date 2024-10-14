using AppCoreAPI.Data;
using AppCoreAPI.SeedWorks.Interfaces;

namespace AppCoreAPI.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
