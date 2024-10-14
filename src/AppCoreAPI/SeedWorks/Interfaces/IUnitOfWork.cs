using AppCoreAPI.Repositories.Interfaces;

namespace AppCoreAPI.SeedWorks.Interfaces
{
    public interface IUnitOfWork
    {
        IRootFolderRepository RootFolderRepository { get; }
        ISharedToUserRepository SharedToUserRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
