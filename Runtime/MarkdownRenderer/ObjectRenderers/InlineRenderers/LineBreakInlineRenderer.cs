using System;
using Markdig.Extensions.DefinitionLists;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.InlineRenderers
{
    [UitkMarkdownRenderer]
    public class LineBreakInlineRenderer : MarkdownObjectRenderer<VisualElementRenderer,LineBreakInline>
    {
        protected override void Write(VisualElementRenderer renderer, LineBreakInline obj)
        {
            SerialLogger.Log("Line break!");
            if (obj.IsHard)
            {
                renderer.HardLineBreak();
            }
            else
            {
                renderer.WriteText(" ");
            }
        }
    }
}