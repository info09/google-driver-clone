namespace AppCoreAPI.Helpers
{
    public class FileOrFolderParams : PaginationParams
    {
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
