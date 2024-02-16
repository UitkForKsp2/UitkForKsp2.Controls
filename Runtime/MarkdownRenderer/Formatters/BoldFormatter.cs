using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public class BoldFormatter : BaseFormatter
    {
        private static BoldFormatter _instance;
        public static BoldFormatter Instance => _instance ??= new BoldFormatter();
        private BoldFormatter()
        {
        }

        public override FormatMode Mode => FormatMode.TextLevel;

        public override void BeginApplyingFormatting(TextElement element)
        {
            element.text += "<b>";
        }

        public override void EndApplyingFormatting(TextElement element)
        {
            element.text += "</b>";
        }
    }
}