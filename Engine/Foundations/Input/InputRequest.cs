namespace Shayou.Engine.Foundations.Input
{
    public abstract class InputRequest
    {
        protected InputRequest(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public abstract InputCheckResult CheckInput(InputSubmission submission);
    }
}
