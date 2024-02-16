using System;
using JetBrains.Annotations;

namespace UitkForKsp2.Controls.MarkdownRenderer.CustomContainers
{
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(ICustomContainer))]
    public class CustomContainerAttribute : Attribute
    {
        public string ContainerType;
        public CustomContainerAttribute(string containerType)
        {
            ContainerType = containerType;
        }
    }
}