using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public abstract class BaseFormatter
    {
        public abstract bool ApplyToButtonTopLevel { get; }
        public abstract void BeginApplyingFormatting(TextElement element, bool linkMode);

        public abstract void EndApplyingFormatting(TextElement element, bool linkMode);
    }
}