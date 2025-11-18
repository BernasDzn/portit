namespace Api.Application.Exceptions
{
    public class NotificationIsPermanentException : Exception
    {
        public NotificationIsPermanentException() : base("The notification is marked as permanent and cannot be modified or deleted.")
        {
        }

        public NotificationIsPermanentException(string message) : base(message)
        {
        }

        public NotificationIsPermanentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}