using System.Globalization;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.Formatters
{
    public class HeadingFormatter : BaseFormatter
    {
        private static readonly HeadingFormatter H1 = new(1);
        private static readonly HeadingFormatter H2 = new(2);
        private static readonly HeadingFormatter H3 = new(3);
        private static readonly HeadingFormatter H4 = new(4);
        private static readonly HeadingFormatter H5 = new(5);
        private static readonly HeadingFormatter H6 = new(5);
        public static HeadingFormatter GetFormatter(int headingSize) {
            switch (headingSize)
            {
                case 1:
                    return H1;
                case 2:
                    return H2;
                case 3:
                    return H3;
                case 4:
                    return H4;
                case 5:
                    return H5;
                case 6:
                    return H6;
                default:
                    return H6;
            }
        }

        private int _headingSize;
        private HeadingFormatter(int headingSize)
        {
            _headingSize = headingSize;
        }

        public override FormatMode Mode => FormatMode.ElementLevel;

        public override void BeginApplyingFormatting(TextElement element)
        {
            element.AddToClassList($"h{_headingSize.ToString(CultureInfo.InvariantCulture)}");
        }

        public override void EndApplyingFormatting(TextElement element)
        {
        }
    }
}