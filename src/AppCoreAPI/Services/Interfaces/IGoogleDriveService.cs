using AppCoreAPI.Dtos;
using AppCoreAPI.Helpers;

namespace AppCoreAPI.Services.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<bool> DeleteFile(string filePath);
        Task<bool> DeleteFolder(string folderPath);
        Task<bool> RenameFolder(string source, string destination);
        Task<bool> RenameFile(string source, string destination);
        Task<PagedList<FolderOrFile>> GetFileAndFolders(FileOrFolderParams fileOrFolderParams);
        Task<PagedList<FolderOrFile>> GetFolders(FileOrFolderParams fileOrFolderParams);
        Task<bool> CreateDirectory(string fullPath);
        Task<bool> GetFileOrFolder(bool isFolder, string path);
        Task<bool> MoveFolderOrFile(bool isFolder, string source, string dest);
    }
}
