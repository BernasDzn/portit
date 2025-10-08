namespace Api.Application.Exceptions
{
    public class OutdatedDecisionException : Exception
    {
        public OutdatedDecisionException() : base("The decision is outdated and cannot be applied.")
        {
        }

        public OutdatedDecisionException(string message) : base(message)
        {
        }

        public OutdatedDecisionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}