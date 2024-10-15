namespace AppCoreAPI.Dtos
{
    public class FolderOrFile
    {
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }

        public FolderOrFile(string fullPath, string path, string name, bool isFolder = true)
        {
            Path = path;
            FullPath = fullPath;
            Name = name;
            IsFolder = isFolder;
        }
    }

    public class FolderOrFileRename : FolderOrFile
    {
        public FolderOrFileRename(string fullPath, string path, string name, bool isFolder = true) : base(fullPath, path, name, isFolder)
        {
        }
        public string NewName { get; set; }
        public string OldFullPath { get; set; }
    }
}
