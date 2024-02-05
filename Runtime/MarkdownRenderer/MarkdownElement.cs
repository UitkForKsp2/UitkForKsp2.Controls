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
            _pipeline = new MarkdownPipelineBuilder().UsePipeTables().UseGridTables().UseAutoLinks()
                .UseTaskLists().UseListExtras().Build();
            style.flexDirection = FlexDirection.Column;
        }

        public MarkdownElement(string markdown, string rootPath) : this()
        {
            _markdown = markdown;
            Build();
        }
        
        public new class UxmlFactory : UxmlFactory<MarkdownElement>
        {
        }
        
        
        private string _markdown = string.Empty;

        public string Markdown { get => _markdown;
            set
            {
                _markdown = value;
                Build();
            }
        }
        
        public void Build()
        {
            // First we clear our children
            Clear();
            var document = Markdig.Markdown.Parse(Markdown, _pipeline);
            VisualElementRenderer.RenderInto(this, document);
        }
    }
}
