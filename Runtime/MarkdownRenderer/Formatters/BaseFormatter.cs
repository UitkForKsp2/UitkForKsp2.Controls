using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public abstract class BaseFormatter
    {
        public enum FormatMode
        {
            ElementLevel,
            TextLevel
        }

        public abstract FormatMode Mode { get; }
        public abstract void BeginApplyingFormatting(TextElement element);

        public abstract void EndApplyingFormatting(TextElement element);
    }
}