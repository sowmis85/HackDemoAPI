namespace HackDemoAPI.Common
{
    public class RegValidation
    {
        public const string EventIdRequired ="Event Id is required.";

        public const string AttendeeNameRequired ="Attendee name is required.";

        public const string AttendeeEmailRequired ="Attendee email is required.";

        public const string InvalidEmail ="Invalid attendee email.";

        public const string InvalidQuantity = "Quantity must be greater than zero.";

        public const string EventNotOpenForRegistration = "Event is not open for registration.";

        public const string AlreadyRegisteredForEvent = "Attendee is already registered for this event.";

        public const string RegistrationNotFound = "Registration Not Found";

        public const string RegistrationAlreadyCancelled = "Registration already cancelled.";

        public const string RegistrationCancelledSuccessfully = "Registration cancelled successfully.";
    }
}
