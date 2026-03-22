namespace Shayou.Engine.Foundations.Input
{
    public sealed class InputCheckResult
    {
        private InputCheckResult(bool isValid, string? errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; }

        public string? ErrorMessage { get; }

        public static InputCheckResult Valid()
        {
            return new InputCheckResult(true, null);
        }

        public static InputCheckResult Invalid(string errorMessage)
        {
            return new InputCheckResult(false, errorMessage);
        }
    }
}
