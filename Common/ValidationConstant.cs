namespace HackDemoAPI.Common
{
    public static class ValidationConstant
    {
        public const string JWTConfigValidation = "JWT configuration is not complete. Please check appsettings.json";
        public const string? UserRegistrationSuccess = "User registered successfully!";
        public const string? InvalidCrendentials = "Invalid credentials.";
        public const string? InvalidRefreshToken = "Invalid refresh token.";
        public const string? RefreshTokenExpired = "Refresh token expired.";
        public const string? EventCreated = "Event created successfully.";
        public const string? EventUpdated = "Event updated successfully.";
        public const string? EventDeleted = "Event deleted successfully.";
        public const string AlreadyProcessed = "Already Processed";
        public const string Reserved = "Reserved";
        public const string Released = "Released";
        public const string EventNotFound = "Event not found";
        public const string NoSeatsAvailable = "No Seats Available";
        public const string ReservationNotFound = "Reservation Not Found";
       

        public static readonly string RELEASE = "RELEASE";
        public static readonly string RESERVE = "RESERVE";
    }
}
