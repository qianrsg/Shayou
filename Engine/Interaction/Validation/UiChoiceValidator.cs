namespace Shayou.Engine.Interaction.Validation
{
    public class UiChoiceValidator
    {
        public string Key { get; set; }

        public UiChoiceValidator(string key)
        {
            Key = key;
        }

        public virtual bool IsChoosable()
        {
            return true;
        }

        public virtual bool IsConfirmable()
        {
            return true;
        }
    }
}
