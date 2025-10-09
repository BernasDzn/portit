namespace Api.Application.Exceptions
{
    public class InvalidRepresentativeException : Exception
    {
        public InvalidRepresentativeException() : base("The provided representative does not represent the vessel owner.")
        {
        }

        public InvalidRepresentativeException(string message) : base(message)
        {
        }

        public InvalidRepresentativeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}