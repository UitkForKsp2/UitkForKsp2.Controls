using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public class ParagraphFormatter : BaseFormatter
    {
        private static ParagraphFormatter _instance;
        public static ParagraphFormatter Instance => _instance ??= new ParagraphFormatter();
        private ParagraphFormatter()
        {
        }

        public override FormatMode Mode => FormatMode.ElementLevel;

        public override void BeginApplyingFormatting(TextElement element)
        {
            element.AddToClassList("p");
        }

        public override void EndApplyingFormatting(TextElement element)
        {
        }
    }
}