using Markdig.Renderers;
using Markdig.Syntax.Inlines;


namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.InlineRenderers
{
    [UitkMarkdownRenderer]
    public class LinkInlineRenderer : MarkdownObjectRenderer<VisualElementRenderer,LinkInline>
    {
        protected override void Write(VisualElementRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                renderer.Image(obj.Url);
            }
            else
            {
                renderer.StartLink(obj.Url);
                renderer.WriteChildren(obj);
                renderer.EndLink();
            }
        }
    }
}