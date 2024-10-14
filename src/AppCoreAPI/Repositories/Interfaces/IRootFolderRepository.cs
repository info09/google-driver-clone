using AppCoreAPI.Data.Entities;

namespace AppCoreAPI.Repositories.Interfaces
{
    public interface IRootFolderRepository
    {
        void Add(RootFolder rootFolder);
        Task<RootFolder> GetRootFolder(int id);
        Task<RootFolder> GetRootFolderByUserId(int userId);
    }
}
