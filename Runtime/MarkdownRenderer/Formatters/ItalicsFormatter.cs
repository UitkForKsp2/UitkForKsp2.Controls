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

        public override bool ApplyToButtonTopLevel => false;

        public override void BeginApplyingFormatting(TextElement element, bool linkMode)
        {
            if (linkMode)
            {
                element.text += "<i>";
            }
            else
            {
                element.AddToClassList("md-italic");
            }
        }

        public override void EndApplyingFormatting(TextElement element, bool linkMode)
        {
            if (linkMode)
            {
                element.text += "</i>";
            }
        }
    }
}