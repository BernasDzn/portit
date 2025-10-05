namespace Api.Application.Exceptions
{
    public class StorageFullException : Exception
    {
        public StorageFullException()
        {
        }

        public StorageFullException(string message)
            : base(message)
        {
        }

        public StorageFullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}