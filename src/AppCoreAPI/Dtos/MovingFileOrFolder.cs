namespace AppCoreAPI.Dtos
{
    public class MovingFileOrFolder
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool IsFolder { get; set; }
    }
}
