using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    [PublicAPI]
    public class UitkMarkdownRendererAttribute : Attribute
    {
    }
}