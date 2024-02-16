using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public class CodeFormatter : BaseFormatter
    {
        private static CodeFormatter _instance;
        public static CodeFormatter Instance => _instance ??= new CodeFormatter();
        private CodeFormatter()
        {
        }

        public override FormatMode Mode => FormatMode.ElementLevel;
        public override void BeginApplyingFormatting(TextElement element)
        {
            element.AddToClassList("md-code");
        }

        public override void EndApplyingFormatting(TextElement element)
        {
        }
    }
}