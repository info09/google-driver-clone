namespace AppCoreAPI.Dtos
{
    public class SharedToUserAddDto
    {
        public string OwnerUserName { get; set; }
        public string[] SharedUserName { get; set; }
        public string FullPath { get; set; }
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public bool IsFolder { get; set; }
        public string Name { get; set; }
    }
}
