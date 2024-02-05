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

        public override bool ApplyToButtonTopLevel => true;

        public override void BeginApplyingFormatting(TextElement element, bool linkMode)
        {
            if (linkMode)
            {
            }
            else
            {
                element.AddToClassList("p");
            }
        }

        public override void EndApplyingFormatting(TextElement element, bool linkMode)
        {
        }
    }
}