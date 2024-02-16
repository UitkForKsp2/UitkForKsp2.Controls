using JetBrains.Annotations;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.CustomContainers
{
    public interface ICustomContainer
    {
        public CustomContainerType Type { get; }

        public (VisualElement topLevel, VisualElement content) BuildContainer([CanBeNull] string arguments);

        public VisualElement BuildOpaque(string arguments, VisualElementRenderer renderer,
            CustomContainer markdownObject);

    }
}