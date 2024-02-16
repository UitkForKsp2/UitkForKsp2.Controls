using System.Globalization;
using Markdig.Renderers;
using Markdig.Syntax;


namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class ParagraphBlockRenderer : MarkdownObjectRenderer<VisualElementRenderer, ParagraphBlock>
    {
        protected override void Write(VisualElementRenderer renderer, ParagraphBlock obj)
        {
            using var block = renderer.PushBlock();
            using var paragraphHandle = renderer.PushParagraph();
            renderer.WriteInlines(obj);
        }
    }
}