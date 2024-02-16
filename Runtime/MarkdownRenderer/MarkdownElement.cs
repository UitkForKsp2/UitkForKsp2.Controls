using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Markdig;
// using Markdig.Extensions.EmphasisExtras;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace UitkForKsp2.Controls.MarkdownRenderer
{
    [PublicAPI]
    public class MarkdownElement : VisualElement
    {

        private MarkdownPipeline _pipeline;
        
        public MarkdownElement()
        {
            _pipeline = new MarkdownPipelineBuilder().UseAutoLinks()
                .UseTaskLists().UseListExtras().UseCustomContainers().Build();
            style.flexDirection = FlexDirection.Column;
        }

        public MarkdownElement(string text, string rootPath) : this()
        {
            _text = text;
            Build();
        }
        
        public new class UxmlFactory : UxmlFactory<MarkdownElement,UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _markdown = new()
            {
                name = "text",
            };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                Label le;
                base.Init(ve, bag, cc);
                var ate = ve as MarkdownElement;

                ate!.text = _markdown.GetValueFromBag(bag, cc);
            }
        }
        
        
        private string _text = string.Empty;
        
        // Disable this because of the UI builder
        // ReSharper disable once InconsistentNaming
        public string text { get => _text;
            set
            {
                _text = value;
                Build();
            }
        }
        
        public void Build()
        {
            // First we clear our children
            Clear();
            var document = Markdown.Parse(text, _pipeline);
            VisualElementRenderer.RenderInto(this, document);
        }
    }
}
