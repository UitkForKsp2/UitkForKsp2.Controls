using Markdig.Renderers;
using Markdig.Syntax;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class QuoteBlockRenderer : MarkdownObjectRenderer<VisualElementRenderer,QuoteBlock>
    {
        private static void SetupQuoteHeader(VisualElement block)
        {
            block.style.flexDirection = FlexDirection.Row;
            var blockA = new VisualElement();
            blockA.AddToClassList("quote-outside");
            block.Add(blockA);
            var blockB = new VisualElement();
            blockB.AddToClassList("quote-middle");
            block.Add(blockB);
            var blockC = new VisualElement();
            blockC.AddToClassList("quote-outside");
            block.Add(blockC);
        }

        private static void WriteInternals(VisualElementRenderer renderer, QuoteBlock obj)
        {
            using var innerBlock = renderer.PushBlock();
            innerBlock.MainBlock.style.flexGrow = 1;
            renderer.WriteChildren(obj);
        }
        protected override void Write(VisualElementRenderer renderer, QuoteBlock obj)
        {
            using var outerBlock = renderer.PushBlock();
            SetupQuoteHeader(outerBlock.MainBlock);
            WriteInternals(renderer, obj);
        }
    }
}