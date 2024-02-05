using Markdig.Renderers;
using Markdig.Syntax.Inlines;

// ReSharper disable once CheckNamespace
namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.InlineRenderers
{
    [UitkMarkdownRenderer]
    public class LiteralInlineRenderer : MarkdownObjectRenderer<VisualElementRenderer,LiteralInline>
    {
        protected override void Write(VisualElementRenderer renderer, LiteralInline obj)
        {
            renderer.WriteText(obj.Content.ToString());
        }
    }
}