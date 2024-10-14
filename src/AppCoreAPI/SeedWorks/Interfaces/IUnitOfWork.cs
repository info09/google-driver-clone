namespace AppCoreAPI.SeedWorks.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> Complete();
        bool HasChanges();
    }
}
