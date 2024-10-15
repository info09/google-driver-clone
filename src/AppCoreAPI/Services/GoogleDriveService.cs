using AppCoreAPI.Dtos;
using AppCoreAPI.Helpers;
using AppCoreAPI.Services.Interfaces;

namespace AppCoreAPI.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly ILogger<GoogleDriveService> _logger;

        public GoogleDriveService(ILogger<GoogleDriveService> logger)
        {
            _logger = logger;
        }

        public Task<bool> CreateDirectory(string fullPath)
        {
            bool isSuccess = false;
            try
            {
                Directory.CreateDirectory(fullPath);

                isSuccess = true;
                _logger.LogInformation(fullPath + " Directory created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in CreateDirectory");
            }

            return Task.FromResult(isSuccess);
        }

        public Task<bool> DeleteFile(string filePath)
        {
            bool isDeleted = false;
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    isDeleted = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Has error in DeleteFile");
                }
            }
            return Task.FromResult(isDeleted);
        }

        public Task<bool> DeleteFolder(string folderPath)
        {
            bool isDeleted = false;
            try
            {
                Directory.Delete(folderPath, true);
                isDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in DeleteFolder");
            }
            return Task.FromResult(isDeleted);
        }

        public Task<PagedList<FolderOrFile>> GetFileAndFolders(FileOrFolderParams fileOrFolderParams)
        {
            IQueryable<FolderOrFile> list = new FolderOrFile[] { }.AsQueryable();
            try
            {
                var dirs = Directory.GetDirectories(fileOrFolderParams.Path, "*", SearchOption.TopDirectoryOnly);
                foreach (var dir in dirs)
                {
                    var temp = fileOrFolderParams.Path + "\\";
                    var name = dir.Substring(temp.Length);
                    list = list.Concat(new FolderOrFile[] { new FolderOrFile(dir, fileOrFolderParams.Path, name) });
                }

                string[] filePaths = Directory.GetFiles(fileOrFolderParams.Path, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var file in filePaths)
                {
                    var temp = fileOrFolderParams.Path + "\\";
                    var name = file.Substring(temp.Length);
                    list = list.Concat(new FolderOrFile[] { new FolderOrFile(file, fileOrFolderParams.Path, name, false) });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Has error in GetFileAndFolders");
            }
            return Task.FromResult(PagedList<FolderOrFile>.Create(list, fileOrFolderParams.PageNumber, fileOrFolderParams.PageSize));
        }

        public Task<bool> GetFileOrFolder(bool isFolder, string path)
        {
            bool isExist = false;
            try
            {
                if (isFolder)
                {
                    isExist = Directory.Exists(path);
                }
                else
                {
                    isExist = File.Exists(path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in GetFileOrFolder");
            }
            return Task.FromResult(isExist);
        }

        public Task<PagedList<FolderOrFile>> GetFolders(FileOrFolderParams fileOrFolderParams)
        {
            IQueryable<FolderOrFile> list = new List<FolderOrFile>().AsQueryable();

            try
            {
                var dirs = Directory.GetDirectories(fileOrFolderParams.Path, "*", SearchOption.TopDirectoryOnly);
                foreach (var dir in dirs)
                {
                    var temp = fileOrFolderParams.Path + "\\";
                    var name = dir.Substring(temp.Length);
                    list = list.Concat(new FolderOrFile[] { new FolderOrFile(dir, fileOrFolderParams.Path, name) });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in GetFolders");
            }


            return Task.FromResult(PagedList<FolderOrFile>.Create(list, fileOrFolderParams.PageNumber, fileOrFolderParams.PageSize));
        }

        public Task<bool> MoveFolderOrFile(bool isFolder, string source, string dest)
        {
            bool isRename = false;
            try
            {
                if (isFolder)
                    Directory.Move(source, dest);
                else
                    File.Move(source, dest, true);
                isRename = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Has error in MoveFolderOrFile");
            }
            return Task.FromResult(isRename);
        }

        public Task<bool> RenameFile(string source, string destination)
        {
            bool isRename = false;
            try
            {
                File.Move(source, destination);
                isRename = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in RenameFile");
            }
            return Task.FromResult(isRename);
        }

        public Task<bool> RenameFolder(string source, string destination)
        {
            bool isRename = false;
            try
            {
                Directory.Move(source, destination);
                isRename = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Has error in RenameFolder");
            }
            return Task.FromResult(isRename);
        }
    }
}
