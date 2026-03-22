namespace Shayou.Engine.Foundations.Input
{
    public sealed class InputSubmission
    {
        public string ActionKey { get; init; } = string.Empty;

        public IReadOnlyList<SelectionRef> Selections { get; init; } = Array.Empty<SelectionRef>();
    }
}
