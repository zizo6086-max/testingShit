namespace Domain.Constants;

public static class ValidationConstants
{
    public static class Username
    {
        public const string Pattern = "^[a-zA-Z][a-zA-Z0-9_]{2,19}$";
        public const string ErrorMessage = "Username must start with a letter and be 3-20 characters long, containing only letters, numbers, and underscores.";
    }
    
    public static class Password
    {
        public const string Pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";
        public const string ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
    }
    
    public static class Email
    {
        public const string ErrorMessage = "Please provide a valid email address";
    }
}
