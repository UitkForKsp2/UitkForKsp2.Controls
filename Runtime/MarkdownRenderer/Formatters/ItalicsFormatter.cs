using UnityEngine;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public class ItalicsFormatter : BaseFormatter
    {
        private static ItalicsFormatter _instance;
        public static ItalicsFormatter Instance => _instance ??= new ItalicsFormatter();
        private ItalicsFormatter()
        {
        }

        public override FormatMode Mode => FormatMode.TextLevel;

        public override void BeginApplyingFormatting(TextElement element)
        {
            element.text += "<i>";
        }

        public override void EndApplyingFormatting(TextElement element)
        {
            element.text += "</i>";
        }
    }
}