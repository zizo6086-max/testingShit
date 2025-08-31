namespace Domain.Constants;

public static class FileConstants
{
    public static class Messages
    {
        public const string FileUploadSuccessful = "File uploaded successfully";
        public const string FileDeleteSuccessful = "File deleted successfully";
        public const string InvalidFileType = "Invalid file type";
        public const string FileTooLarge = "File size exceeds maximum allowed size";
        public const string NoFileProvided = "No file provided";
    }
    
    public static class Directories
    {
        public const string Users = "Users";
        public const string Uploads = "uploads";
    }
}
