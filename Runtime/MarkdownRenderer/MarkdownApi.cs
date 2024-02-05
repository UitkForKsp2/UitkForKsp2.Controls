using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer
{
    public static class MarkdownApi
    {
        /// <summary>
        /// A delegate for an image handler, imagePath is the part passed after the protocol (e.g. https://)
        /// </summary>
        public delegate void ImageHandler(string imagePath, Image target);

        private static readonly Dictionary<string,ImageHandler> MarkdownImageHandlers = new();

        public static void RegisterMarkdownImageHandler(string protocol, ImageHandler handler)
        {
            MarkdownImageHandlers[protocol] = handler;
        }

        [CanBeNull]
        public static void HandleImage(string imagePath, Image target)
        {
            // While it's not handling
            target.image = Texture2D.blackTexture; // TODO: Add a loading texture
            var protocolEnd = imagePath.IndexOf("://", StringComparison.Ordinal);
            if (protocolEnd == -1) return;
            var protocol = imagePath[..protocolEnd];
            if (MarkdownImageHandlers.TryGetValue(protocol, out var handler))
            {
                handler(imagePath[(protocolEnd + 3)..], target);
            }
        }
        
        /// <summary>
        /// A delegate for a link handler, action is the part passed after the protocol (e.g. https://)
        /// </summary>
        public delegate void LinkHandler(string action);

        private static readonly Dictionary<string, LinkHandler> MarkdownLinkHandlers = new()
        {
            ["https"] = action =>
            {
                Application.OpenURL($"https://{action}");
            },
        };
        
        public static void RegisterMarkdownLinkHandler(string protocol, LinkHandler handler)
        {
            MarkdownLinkHandlers[protocol] = handler;
        }

        public static void HandleLink(string link)
        {
            var protocolEnd = link.IndexOf("://", StringComparison.Ordinal);
            if (protocolEnd == -1) return;
            var protocol = link[..protocolEnd];
            if (MarkdownLinkHandlers.TryGetValue(protocol, out var handler))
            {
                handler(link[(protocolEnd + 3)..]);
            }
        }
        
        
        // The parsing of this is going to be "fun"
        public delegate string SyntaxHighlighter(string block);

        private static readonly Dictionary<string, SyntaxHighlighter> MarkdownSyntaxHighlighters = new();

        public static void RegisterSyntaxHighlighter(string language, SyntaxHighlighter highlighter)
        {
            MarkdownSyntaxHighlighters[language] = highlighter;
        }

        /// <summary>
        /// Highlights a block of code based off of the language
        /// </summary>
        /// <param name="language">The language for the code block (e.g. scss)</param>
        /// <param name="input">The sanitized input (`&lt;` characters replaced)</param>
        /// <returns>A rich text form with syntax highlighting tags</returns>
        public static string Highlight(string language, string input)
        {
            return MarkdownSyntaxHighlighters.TryGetValue(language, out var highlighter) ? highlighter(input) : input;
        }
    }
}