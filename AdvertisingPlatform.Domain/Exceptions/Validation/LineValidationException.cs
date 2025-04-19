namespace AdvertisingPlatforms.Domain.Exceptions.Validation
{
    [Serializable]
    internal class LineValidationException : DomainValidationException
    {
        public int LineNumber { get; }

        public LineValidationException()
        {
        }

        public LineValidationException(string? message) : base(message)
        {
        }

        public LineValidationException(
            int lineNumber,
            string? message
        ) : base($"Line {lineNumber}: {message}")
        {
            LineNumber = lineNumber;
        }

        public LineValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}