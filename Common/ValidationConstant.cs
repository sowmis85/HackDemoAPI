using System.Runtime.CompilerServices;

namespace HackDemoAPI.Common
{
    public static class ValidationConstant
    {
        public const string JWTConfigValidation = "JWT configuration is not complete. Please check appsettings.json";
        public const string UserRegistrationSuccess = "User registered successfully!";
        public const string InvalidCrendentials = "Invalid login credentials.";
        public const string InvalidRefreshToken = "Invalid refresh token.";
        public const string RefreshTokenExpired = "Refresh token expired.";
        public const string MissingRefreshToken = "Refresh token is missing.";
        public const string EventCreated = "Event created successfully.";
        public const string EventUpdated = "Event updated successfully.";
        public const string EventDeleted = "Event deleted successfully.";
        public const string AlreadyProcessed = "Already Processed";
        public const string Reserved = "Reserved";
        public const string Released = "Released";
        public const string EventNotFound = "Event not found";
        public const string NoSeatsAvailable = "No Seats Available";
        public const string ReservationNotFound = "Reservation Not Found";
        public const string WorkflowNotFound = "Workflow Not Found";
        public const string InternalServerError = "Internal Server Error";
        public const string EmailExistsError = "Email already exists";
        public const string UnExpectedError = "An unexpected error occurred during login";
        public const string LoginSuccessMessage= "Login successful";
        public const string InvalidLoginRequest = "Invalid login request";
        public const string UnExprectedRefreshError = "An unexpected error occurred during token refresh";
        public const string TokenRefreshSuccess = "Token refreshed successfully";
        public const string UnExpectedRegistrationError = "An unexpected error occurred during registration";
        public const string ValidationError = "Validation failed";
        public const string EmailRequired = "Email is required";
        public const string NewPasswordRequired = "New password is required";
        public const string PasswordUpdateSuccess = "Password updated successfully";
        public const string EmailIdNotFound = "Email Id not found";

        //Error codes
        public static readonly string RELEASE = "RELEASE";
        public static readonly string RESERVE = "RESERVE";
        public static readonly string UNAUTHORIZED = "UNAUTHORIZED";
        public static readonly string VALIDATION_ERROR = "VALIDATION_ERROR";
        public static readonly string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public static readonly string EMAIL_EXISTS= "EMAIL_EXISTS";
        public static readonly string  INVALID_REFRESH_TOKEN= "INVALID_REFRESH_TOKEN";
        public static readonly string MISSING_REFRESH_TOKEN = "MISSING_REFRESH_TOKEN";
        public static readonly string EMAIL_NOT_FOUND = "EMAIL_NOT_FOUND";
    }
}
