using AppCoreAPI.Dtos;
using AppCoreAPI.Errors;
using AppCoreAPI.Extensions;
using AppCoreAPI.Helpers;
using AppCoreAPI.SeedWorks.Interfaces;
using AppCoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;

namespace AppCoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleDriverController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDriveService _googleDriveService;

        public GoogleDriverController(IUnitOfWork unitOfWork, IGoogleDriveService googleDriveService)
        {
            _googleDriveService = googleDriveService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetFileOrFolder([FromQuery] FileOrFolderParams fParams)
        {
            var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolderByUserId(User.GetUserId());
            if (rootFolder == null) return NotFound(new ApiResponse(404, "rootFolder not found"));
            if (string.IsNullOrEmpty(fParams.Path))
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
                fParams.Path = rootPath;
            }

            var listItem = await _googleDriveService.GetFileAndFolders(fParams);
            Response.AddPaginationHeader(listItem.CurrentPage, listItem.PageSize, listItem.TotalCount, listItem.TotalPages);

            return Ok(listItem);
        }

        [HttpGet("get-root-folder")]
        public async Task<ActionResult> GetRootFolder()
        {
            var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolderByUserId(User.GetUserId());
            if (rootFolder == null) return NotFound(new ApiResponse(404, "rootFolder not found"));

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
            return Ok(new FolderOrFile(rootPath, "\\", "My drive"));
        }

        [HttpGet("get-folders")]
        public async Task<ActionResult> GetFolders([FromQuery] FileOrFolderParams fParams)
        {
            var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolderByUserId(User.GetUserId());
            if (rootFolder == null) return NotFound("Root Folder not found");
            if (string.IsNullOrEmpty(fParams.Path))
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
                fParams.Path = rootPath;
            }

            var listItem = await _googleDriveService.GetFolders(fParams);
            Response.AddPaginationHeader(listItem.CurrentPage, listItem.PageSize, listItem.TotalCount, listItem.TotalPages);

            return Ok(listItem);
        }

        [HttpPost("new-folder")]
        public async Task<ActionResult> NewFolder(string parentPath, string name)
        {
            //tao thu muc ngoai folder root
            if (string.IsNullOrEmpty(parentPath))
            {
                var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolderByUserId(User.GetUserId());
                if (rootFolder == null) return NotFound(new ApiResponse(404, "rootFolder not found"));

                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
                string fullPath = rootPath + "\\" + name;

                bool createFolder = await _googleDriveService.CreateDirectory(fullPath);
                if (createFolder) return Ok(new FolderOrFile(fullPath, rootPath, name));
                else
                    return BadRequest(new ApiResponse(400, "Error create Directory"));
            }
            else
            {
                //tao trong thu muc con
                string fullPath = parentPath + "\\" + name;
                bool createFolder = await _googleDriveService.CreateDirectory(fullPath);
                if (createFolder) return Ok(new FolderOrFile(fullPath, parentPath, name));
                else
                    return BadRequest(new ApiResponse(400, "Error create Directory"));
            }
        }

        [HttpPut("move-file-folder")]
        public async Task<ActionResult> MoveFolderOrFile(MovingFileOrFolder fileOrFolder)
        {
            bool isSuccess = await _googleDriveService.MoveFolderOrFile(fileOrFolder.IsFolder, fileOrFolder.Source, fileOrFolder.Destination);
            if (isSuccess)
                return Ok(new { source = fileOrFolder.Source });
            return BadRequest(new ApiResponse(400, "Error in move flie or folder"));
        }

        [HttpPut("rename")]
        public async Task<ActionResult> RenameFileOrFolder(FolderOrFileRename item)
        {
            string source = item.FullPath;
            string dest = item.Path + "\\" + item.NewName;
            bool isRename;
            if (item.IsFolder)
            {
                isRename = await _googleDriveService.RenameFolder(source, dest);
            }
            else
            {
                isRename = await _googleDriveService.RenameFile(source, dest);
            }
            if (isRename)
            {
                var fileFolderRename = new FolderOrFileRename(item.Path + "\\" + item.NewName, item.Path, item.NewName, item.IsFolder);
                fileFolderRename.OldFullPath = item.FullPath;

                return Ok(fileFolderRename);
            }
            else
                return BadRequest(new ApiResponse(400, "Can not rename file or folder"));
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(string item)
        {
            var tempOb = JsonConvert.DeserializeObject<FolderOrFile>(item);
            var isExsit = await _googleDriveService.GetFileOrFolder(tempOb.IsFolder, tempOb.FullPath);
            if (isExsit)
            {
                bool temp;

                if (tempOb.IsFolder)
                {
                    temp = await _googleDriveService.DeleteFolder(tempOb.FullPath);
                }
                else
                {
                    temp = await _googleDriveService.DeleteFile(tempOb.FullPath);
                }
                if (temp) return Ok(tempOb); //xoa thanh cong
                else return BadRequest(new ApiResponse(400, "Error while delete file or folder"));
            }
            return BadRequest(new ApiResponse(400, "File or folder not exist!"));
        }

        private const long MaxFileSize = 10L * 1024L * 1024L * 1024L; // 10GB

        [HttpPost("Upload")]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult> Upload(string parentPath)
        {
            string UploadFolder = "";
            var formCollection = await Request.ReadFormAsync();
            var files = formCollection.Files;

            if (files.Count == 0)
                return BadRequest(new ApiResponse(400, "No file uploaded"));

            var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolderByUserId(User.GetUserId());
            if (rootFolder == null) return NotFound(new ApiResponse(404, "rootFolder not found"));

            if (string.IsNullOrEmpty(parentPath))
            {
                UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
            }
            else
            {
                UploadFolder = parentPath;
            }

            var file = files[0];
            string UploadPath = Path.Combine(UploadFolder, file.Name);
            using (var temp = new FileStream(UploadPath, FileMode.Create))
            {
                await file.CopyToAsync(temp);
            }

            //var connections = await _presenceTracker.GetConnectionsForUser(User.Identity.Name);
            //await _presenceHub.Clients.Clients(connections).SendAsync("OnUploadFileSuccess", "Up load file success");

            return Ok(new FolderOrFile(UploadPath, parentPath, file.Name, false));
        }


        [HttpGet(("download"))]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> Download([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), filePath);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
