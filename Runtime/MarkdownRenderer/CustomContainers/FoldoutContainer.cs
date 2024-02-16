using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.CustomContainers
{
    [CustomContainer("foldout")]
    public class FoldoutContainer : ICustomContainer
    {
        public CustomContainerType Type => CustomContainerType.Container;
        public (VisualElement topLevel, VisualElement content) BuildContainer(string arguments)
        {
            var fo = new Foldout
            {
                text = arguments ?? ""
            };
            return (fo, fo.contentContainer);
        }

        public VisualElement BuildOpaque(string arguments, VisualElementRenderer renderer, CustomContainer markdownObject)
        {
            throw new System.NotImplementedException();
        }
    }
}