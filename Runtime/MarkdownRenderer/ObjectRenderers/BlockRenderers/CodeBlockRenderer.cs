using System.Text;
using Markdig.Renderers;
using Markdig.Syntax;
using UnityEngine;
using UnityEngine.UIElements;


namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class CodeBlockRenderer : MarkdownObjectRenderer<VisualElementRenderer, CodeBlock>
    {
        
        protected override void Write(VisualElementRenderer renderer, CodeBlock obj)
        {
            using var block = renderer.PushBlock("md-code-block");
            var scrollChild = new ScrollView
            {
                mode = ScrollViewMode.Horizontal,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalScrollerVisibility = ScrollerVisibility.Auto
            };
            scrollChild.AddToClassList("md-code-scroll");
            block.MainBlock.Add(scrollChild);
            var textChild = new Label
            {
                enableRichText = true
            };
            textChild.AddToClassList("p");
            scrollChild.Add(textChild);
            var lines = renderer.GetRawLines(obj);
            var highlighted = lines;
            var fenced = obj as FencedCodeBlock;
            if (fenced?.Info != null && fenced.Info.Length != 0)
            {
                var language = fenced.Info;
                highlighted = MarkdownApi.Highlight(language, lines);
            }

            textChild.text = highlighted;
            var copyContainer = new VisualElement();
            copyContainer.AddToClassList("md-code-label-container");
            block.MainBlock.Add(copyContainer);
            var copyButton = new Button
            {
                text = "Copy" // TODO: Localize this.
            };
            copyButton.clicked += () => GUIUtility.systemCopyBuffer = lines;
            copyButton.AddToClassList("link");
            copyButton.AddToClassList("md-copy-button");
            copyContainer.Add(copyButton);
        }
    }
}