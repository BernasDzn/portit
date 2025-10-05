namespace Api.Application.Exceptions
{
    public class UnknownPhysicalResourceType : Exception
    {
        public UnknownPhysicalResourceType()
        {
        }

        public UnknownPhysicalResourceType(string message)
            : base(message)
        {
        }

        public UnknownPhysicalResourceType(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}