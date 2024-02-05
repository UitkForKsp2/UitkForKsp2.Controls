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

        public override bool ApplyToButtonTopLevel => false;

        public override void BeginApplyingFormatting(TextElement element, bool linkMode)
        {
            if (linkMode)
            {
                element.text += "<b>";
            }
            else
            {
                element.AddToClassList("md-bold");
            }
        }

        public override void EndApplyingFormatting(TextElement element, bool linkMode)
        {
            if (linkMode)
            {
                element.text += "</b>";
            }
        }
    }
}