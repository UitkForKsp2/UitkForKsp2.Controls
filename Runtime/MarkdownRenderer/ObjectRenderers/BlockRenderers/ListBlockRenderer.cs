using Markdig.Renderers;
using Markdig.Syntax;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class ListBlockRenderer : MarkdownObjectRenderer<VisualElementRenderer, ListBlock>
    {
        private static string GetNextInOrder(char style, string current)
        {
            switch (style)
            {
                case '1':
                    return $"{int.Parse(current)+1}";
                default:
                    return $"{(char)(current[0]+1)}";
            }
        }

        private static string GetLast(char style, string start, int count)
        {
            switch (style)
            {
                case '1':
                    return $"{int.Parse(start) + (count - 1)}";
                default:
                    return $"{(char)(start[0]+(count)-1)}";
            }
        }

        private static void RenderContainer(VisualElementRenderer renderer, ListItemBlock block)
        {
            using var containerBlock = renderer.PushBlock();
            containerBlock.MainBlock.style.flexDirection = FlexDirection.Column;
            containerBlock.MainBlock.style.flexGrow = new StyleFloat(1);
            containerBlock.MainBlock.style.flexShrink = new StyleFloat(0f);
            renderer.WriteChildren(block);
        }

        private static string LeftPad(string input, int expectedLength)
        {
            while (input.Length < expectedLength)
            {
                input = " " + input;
            }
            return input;
        }
        protected override void Write(VisualElementRenderer renderer, ListBlock obj)
        {
            var header = obj.IsOrdered ? obj.OrderedStart : $"{obj.BulletType}";
            var lastLength = obj.IsOrdered ? GetLast(obj.BulletType,header,obj.Count).Length : 0;
            var width = lastLength + 20;
            foreach (var block1 in obj)
            {
                using var listItemBlock = renderer.PushBlock();
                listItemBlock.MainBlock.style.flexDirection = FlexDirection.Row;
                listItemBlock.MainBlock.style.flexWrap = Wrap.NoWrap;
                var bulletBlock = new VisualElement
                {
                    style =
                    {
                        minWidth = new StyleLength(width),
                        maxWidth = new StyleLength(width),
                        flexDirection = FlexDirection.Column
                    }
                };
                listItemBlock.MainBlock.Add(bulletBlock);
                var bulletText = new TextElement
                {
                    enableRichText = false
                };
                bulletText.AddToClassList("list-bullet");
                if (obj.IsOrdered)
                {
                    bulletText.text = $"{LeftPad(header,lastLength)}{obj.OrderedDelimiter}";
                    header = GetNextInOrder(obj.BulletType, header);
                }
                else
                {
                    bulletText.text = header;
                }
                bulletBlock.Add(bulletText);
                var item = (ListItemBlock)block1;
                RenderContainer(renderer,item);
            }
        }
    }
}