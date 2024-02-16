using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using UnityEngine;


namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.InlineRenderers
{
    [UitkMarkdownRenderer]
    public class CodeInlineRenderer : MarkdownObjectRenderer<VisualElementRenderer,CodeInline>
    {
        protected override void Write(VisualElementRenderer renderer, CodeInline obj)
        {
            using var handle = renderer.PushCode();
            SerialLogger.Log($"Content: `{obj.Content}`");
            renderer.WriteText(obj.Content,false);
        }
    }
}