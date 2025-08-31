namespace Domain.Constants;

public static class AuthConstants
{
    public static class Claims
    {
        public const string ExternalProvider = "ExternalProvider";
        public const string GoogleId = "GoogleId";
    }
    
    public static class Providers
    {
        public const string Google = "Google";
    }
    
    public static class Messages
    {
        public const string InvalidCredentials = "Invalid Credentials!";
        public const string UserNotFound = "User not found";
        public const string EmailAlreadyExists = "Email already exists!";
        public const string UsernameAlreadyExists = "Username already exists!";
        public const string UserRegisteredSuccessfully = "Successfully registered new user!";
        public const string LoginSuccessful = "Login successful";
        public const string TokenRefreshedSuccessfully = "Successfully Refreshed Token";
        public const string EmailAlreadyVerified = "Email already verified";
        public const string EmailVerificationSent = "Email verification link sent to your email";
    }
    
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Seller = "Seller";
    }
}
