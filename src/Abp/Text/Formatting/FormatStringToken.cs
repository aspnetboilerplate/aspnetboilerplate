namespace Abp.Text.Formatting
{
    internal class FormatStringToken
    {
        public FormatStringToken(string text, FormatStringTokenType type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; private set; }

        public FormatStringTokenType Type { get; private set; }
    }
}