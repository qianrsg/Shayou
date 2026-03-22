namespace Shayou.Engine.Foundations.Input
{
    public interface IInputService
    {
        string? CurrentContextKey { get; }

        InputSubmission WaitForInput(InputRequest request);
    }
}
