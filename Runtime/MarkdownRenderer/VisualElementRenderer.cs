using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using UitkForKsp2.Controls.MarkdownRenderer.Formatters;
using UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer
{
    [PublicAPI]
    public class VisualElementRenderer : RendererBase
    {
        
        private List<BlockInfo> _blockStack = new();

        public BlockInfo CurrentBlock => _blockStack.Last();
        
        private VisualElementRenderer()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes<UitkMarkdownRendererAttribute>().Any())
                {
                    ObjectRenderers.Add((IMarkdownObjectRenderer)Activator.CreateInstance(type));
                }
            }
        }

        [CanBeNull] private static VisualElementRenderer _renderer;
        public static VisualElementRenderer Renderer => _renderer ??= new VisualElementRenderer();

    
        
        [PublicAPI]
        public class BlockInfo : IDisposable
        {
            
            
            public VisualElement MainBlock;


            private List<BaseFormatter> _formatters = new();
            private bool _inLink;
            private VisualElement _currentContainerElement;
            private TextElement _currentTextElement;
            private bool _wasDirtied = true;
            public BlockInfo(VisualElement mainBlock)
            {
                MainBlock = mainBlock;
            }

            
            public void Dispose()
            {
                Renderer.PopBlock();
                for (var i = _formatters.Count-1; i >= 0; i--)
                {
                    if (_currentTextElement != null)
                    {
                        _formatters[i].EndApplyingFormatting(_currentTextElement, _inLink);
                    }
                }
            }

            public FormatterHandle PushFormatter(BaseFormatter formatter)
            {
                if (!_inLink)
                {
                    _wasDirtied = true;
                    _currentTextElement = null;
                }
                else
                {
                    formatter.BeginApplyingFormatting(_currentTextElement, true);
                }

                _formatters.Add(formatter);
                SerialLogger.Log($"Pushing Formatter: {formatter.GetType()}");
                return new FormatterHandle(this);
            }

            public void PopFormatter()
            {
                var last = _formatters.Last(); 
                SerialLogger.Log($"Popping Formatter: {last.GetType()} {_inLink}");
                if (!_inLink)
                {
                    _wasDirtied = true;
                    _currentTextElement = null;
                }
                else if (_currentTextElement != null)
                {
                    last.EndApplyingFormatting(_currentTextElement, true);
                }

                _formatters.RemoveAt(_formatters.Count - 1);
            }

            private void EnsureTextElement()
            {
                if (!_wasDirtied) return;
                _wasDirtied = false;
                if (_currentContainerElement == null)
                {
                    _currentContainerElement = new VisualElement
                    {
                        style =
                        {
                            flexWrap = Wrap.Wrap,
                            flexDirection = FlexDirection.Row
                        }
                    };
                    MainBlock.Add(_currentContainerElement);
                }

                _currentTextElement = new Label
                {
                    enableRichText = true
                };
                foreach (var formatter in _formatters)
                {
                    formatter.BeginApplyingFormatting(_currentTextElement, false);
                }
                _currentContainerElement.Add(_currentTextElement);
            }
            private void WriteToken(string token)
            {
                EnsureTextElement();
                // _currentTextElement.text += token;
                _currentTextElement.text += token;
                if (!token.EndsWith(' ')) return;
                _currentTextElement = null;
                _wasDirtied = true;
            }
            
            private List<string> Tokenize(string text)
            {
                List<string> tokens = new();
                int idx;
                while ((idx = text.IndexOf(' ')) != -1)
                {
                    var token = text[..(idx + 1)];
                    text = text[(idx + 1)..];
                    tokens.Add(token);
                }

                if (!string.IsNullOrEmpty(text))
                {
                    tokens.Add(text);
                }

                return tokens;
            }
            
            public void RenderText(string text, bool split=true)
            {
                SerialLogger.Log($"RenderText: `{text}` (dirtied: {_wasDirtied})");
                if (!_inLink)
                {
                    if (split)
                    {
                        foreach (var token in Tokenize(text))
                        {
                            WriteToken(token);
                        }
                    }
                    else
                    {
                        EnsureTextElement();
                        _currentTextElement.text += text;
                        _currentTextElement = null;
                        _wasDirtied = true;
                    }
                }
                else
                {
                    _currentTextElement.text += text;
                }
            }

            public void StartLink(string linkAddress)
            {
                if (_currentContainerElement == null)
                {
                    _currentContainerElement = new VisualElement
                    {
                        style =
                        {
                            flexWrap = Wrap.Wrap,
                            flexDirection = FlexDirection.Row
                        }
                    };
                    MainBlock.Add(_currentContainerElement);
                }
                _currentTextElement = new Button
                {
                    enableRichText = true,
                };
                _currentTextElement.AddToClassList("link");
                ((Button)_currentTextElement).clicked += () => MarkdownApi.HandleLink(linkAddress);
                foreach (var formatter in _formatters)
                {
                    formatter.BeginApplyingFormatting(_currentTextElement, !formatter.ApplyToButtonTopLevel);
                }
                _currentContainerElement.Add(_currentTextElement);
                

                _wasDirtied = false;
                _inLink = true;
            }

            public void EndLink()
            {
                for (var i = _formatters.Count-1; i >= 0; i--)
                {
                    if (_currentTextElement != null)
                    {
                        _formatters[i].EndApplyingFormatting(_currentTextElement, _inLink);
                    }
                }
                _wasDirtied = true;
                _currentTextElement = null;
                _inLink = false;
            }

            public void Newline()
            {
                _wasDirtied = true;
                _currentContainerElement = null;
                _currentTextElement = null;
            }

            public void AddImage(string imageAddress, string altText="Image")
            {
                
                _wasDirtied = true;
                _currentContainerElement = null;
                _currentTextElement = null;
                var imageContainer = new Image
                {
                    scaleMode = ScaleMode.ScaleToFit
                };
                MainBlock.Add(imageContainer);
                MarkdownApi.HandleImage(imageAddress, imageContainer);
            }

            public void AddChild(BlockInfo blockInfo)
            {
                MainBlock.Add(blockInfo.MainBlock);
            }
        }
        
        public class FormatterHandle : IDisposable
        {
            public BlockInfo BlockInfo;

            public FormatterHandle(BlockInfo blockInfo)
            {
                BlockInfo = blockInfo;
            }

            public void Dispose()
            {
                BlockInfo.PopFormatter();
            }
        }
    
        public static void RenderInto(VisualElement element, MarkdownObject markdownObject)
        {
            Renderer.ResetRoot(element);
            Renderer.Render(markdownObject);
        }
        public override object Render(MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return _blockStack.First();
        }

        private void ResetRoot(VisualElement root)
        {
            _blockStack.Clear();
            _blockStack.Add(new BlockInfo(root));
        }

        public BlockInfo PushBlock(params string[] classes)
        {
            var newBlock = new VisualElement();
            foreach (var @class in classes)
            {
                newBlock.AddToClassList(@class);
            }
            var newInfo = new BlockInfo(newBlock);
            CurrentBlock.AddChild(newInfo);
            _blockStack.Add(newInfo);
            return newInfo;
        }

        public void PopBlock()
        {
            // _blockStack.RemoveAt(_blockStack.Count - 1);
            _blockStack.RemoveAt(_blockStack.Count - 1);
        }

        public void WriteInlines(LeafBlock block)
        {
            var inline = block.Inline as Inline;
            while (inline != null)
            {
                Write(inline);
                inline = inline.NextSibling;
            }
        }
        
        public void WriteTextRaw(string text, bool split=true)
        {
            CurrentBlock.RenderText(text,split);
        }
        public void WriteText(string text, bool split=true)
        {
            WriteTextRaw(text.Replace($"<", "<\u2000B"),split);
        }

        public FormatterHandle PushHeading(int level) =>
            CurrentBlock.PushFormatter(HeadingFormatter.GetFormatter(level));

        public FormatterHandle PushParagraph() => CurrentBlock.PushFormatter(ParagraphFormatter.Instance);
        public FormatterHandle PushItalic() => CurrentBlock.PushFormatter(ItalicsFormatter.Instance);
        public FormatterHandle PushBold() => CurrentBlock.PushFormatter(BoldFormatter.Instance);
        public FormatterHandle PushCode() => CurrentBlock.PushFormatter(CodeFormatter.Instance);
        public void StartLink(string address) => CurrentBlock.StartLink(address);
        public void EndLink() => CurrentBlock.EndLink();
        public void Image(string path, string alt="Image") => CurrentBlock.AddImage(path, alt);
    }
}