namespace Shayou.Engine.Foundations.Input
{
    public sealed class ActionInputRequest : InputRequest
    {
        private readonly HashSet<string> allowedActionKeys;

        public ActionInputRequest(string key, IEnumerable<string> allowedActionKeys)
            : base(key)
        {
            this.allowedActionKeys = new HashSet<string>(allowedActionKeys, StringComparer.Ordinal);
        }

        public IReadOnlyCollection<string> AllowedActionKeys => allowedActionKeys;

        public override InputCheckResult CheckInput(InputSubmission submission)
        {
            if (!allowedActionKeys.Contains(submission.ActionKey))
            {
                return InputCheckResult.Invalid(
                    $"Input '{submission.ActionKey}' is not allowed for request '{Key}'.");
            }

            if (submission.Selections.Count > 0)
            {
                return InputCheckResult.Invalid(
                    $"Input '{submission.ActionKey}' does not accept selections.");
            }

            return InputCheckResult.Valid();
        }
    }
}
