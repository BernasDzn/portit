namespace Api.Application.Exceptions
{
    public class UnknownPhysicalResourceTypeException : Exception
    {
        public UnknownPhysicalResourceTypeException()
        {
        }

        public UnknownPhysicalResourceTypeException(string message)
            : base(message)
        {
        }

        public UnknownPhysicalResourceTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}