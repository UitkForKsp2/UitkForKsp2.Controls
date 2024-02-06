using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
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

        private int _indentLevel = 0;
        
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
                if (_currentTextElement == null) return;
                for (var i = _formatters.Count-1; i >= 0; i--)
                {
                    _formatters[i].EndApplyingFormatting(_currentTextElement);
                }
            }

            public FormatterHandle PushFormatter(BaseFormatter formatter)
            {
                if (!_inLink && formatter.Mode == BaseFormatter.FormatMode.ElementLevel)
                {
                    Dirty();
                }
                if (formatter.Mode == BaseFormatter.FormatMode.TextLevel &&_currentTextElement != null) {
                    formatter.BeginApplyingFormatting(_currentTextElement);
                }

                _formatters.Add(formatter);
                return new FormatterHandle(this);
            }

            public void PopFormatter()
            {
                var last = _formatters.Last();
                if (_currentTextElement != null)
                {
                    last.EndApplyingFormatting(_currentTextElement);
                }
                _formatters.RemoveAt(_formatters.Count - 1);
                if (!_inLink && last.Mode == BaseFormatter.FormatMode.ElementLevel)
                {
                    Dirty();
                }
            }

            public void Dirty()
            {
                _wasDirtied = true;
                if (_currentTextElement != null)
                {
                    for (var i = _formatters.Count-1; i >= 0; i--)
                    {
                        _formatters[i].EndApplyingFormatting(_currentTextElement);
                    }
                }
                _currentTextElement = null;
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
                _currentTextElement.AddToClassList("md-text");
                foreach (var formatter in _formatters)
                {
                    formatter.BeginApplyingFormatting(_currentTextElement);
                }
                _currentContainerElement.Add(_currentTextElement);
            }

            private static bool EndsWithBreakChar(string text) => text.EndsWith(" ");
            private void WriteToken(string token)
            {
                EnsureTextElement();
                _currentTextElement.text += token;
                if (EndsWithBreakChar(token)) Dirty();
            }
            
            
            private static int IndexOfBreakChar(string text)
            {
                for (var i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                    {
                        return i;
                    }
                }
                return -1;
            }
            
            private List<string> Tokenize(string text)
            {
                List<string> tokens = new();
                int idx;
                while ((idx = IndexOfBreakChar(text)) != -1)
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
            
            public void RenderText(string text, bool split)
            {
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
                    formatter.BeginApplyingFormatting(_currentTextElement);
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
                        _formatters[i].EndApplyingFormatting(_currentTextElement);
                    }
                }
                Dirty();
                _inLink = false;
            }

            public void Newline()
            {
                Dirty();
                _currentContainerElement = null;
            }

            public void AddImage(string imageAddress, string altText="Image")
            {
                
                Dirty();
                _currentContainerElement = null;
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
            _indentLevel = 0;
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

        public BlockInfo AddBlock(VisualElement block, bool addAsChild=false)
        {
            var newInfo = new BlockInfo(block);
            if (addAsChild) CurrentBlock.AddChild(newInfo);
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
        
        public void WriteTextRaw(string text, bool split = true)
        {
            CurrentBlock.RenderText(text,split);
        }
        public void WriteText(string text, bool split = true)
        {
            WriteTextRaw(text.Replace($"<", "<\u200B"),split);
        }

        public void Dirty()
        {
            CurrentBlock.Dirty();
        }

        public void HardLineBreak()
        {
            CurrentBlock.Newline();
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

        public IndentationHandle Indent()
        {
            return new IndentationHandle(++_indentLevel);
        }
        
        public class IndentationHandle : IDisposable
        {
            public IndentationHandle(int indentation)
            {
                Indentation = indentation;
            }

            public int Indentation { get; }

            public void Dispose()
            {
                Renderer._indentLevel -= 1;
            }
        }
        
        public string GetRawLines(LeafBlock block)
        {
            var lines = block.Lines;
            var slices = lines.Lines;
            StringBuilder sb = new();
            for (var i = 0; i < lines.Count; i++)
            {
                sb.Append(slices[i].ToString());
                if (i != lines.Count - 1) sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}