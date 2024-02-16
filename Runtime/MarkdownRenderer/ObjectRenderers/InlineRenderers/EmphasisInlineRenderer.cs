using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using UnityEngine;

namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.InlineRenderers
{
    [UitkMarkdownRenderer]
    public class EmphasisInlineRenderer : MarkdownObjectRenderer<VisualElementRenderer,EmphasisInline>
    {
        protected override void Write(VisualElementRenderer renderer, EmphasisInline obj)
        {
            var inline = obj.FirstChild;

            using var handle = obj.DelimiterCount == 1 ? renderer.PushItalic() : renderer.PushBold();
            while (inline != null)
            {
                renderer.Write(inline);
                if (inline == obj.LastChild) break;
                inline = inline.NextSibling;
            }
        }
    }
}