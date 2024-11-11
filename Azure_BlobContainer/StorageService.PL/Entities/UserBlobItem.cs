namespace StorageService.PL.Entities
{
    public class UserBlobItem
    {
        public string fileName = null!;
        public string FileName
        {
            get => fileName;

            set {
                if (value.Contains('/'))
                {
                    var index = value.LastIndexOf('/');
                    fileName = value.Substring(index+1).ToLower();
                }
                else
                {
                    fileName = value.ToLower();
                }
            }
        }
        public string filePath { get; set; } = null!;
    }
}
