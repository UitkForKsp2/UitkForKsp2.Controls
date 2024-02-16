using System.Globalization;
using Markdig.Renderers;
using Markdig.Syntax;

// ReSharper disable once CheckNamespace
namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class HeadingBlockRenderer : MarkdownObjectRenderer<VisualElementRenderer, HeadingBlock>
    {
        protected override void Write(VisualElementRenderer renderer, HeadingBlock obj)
        {
            using var block = renderer.PushBlock();
            using var headingHandle = renderer.PushHeading(obj.Level);
            renderer.WriteInlines(obj);
        }
    }
}