using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using UitkForKsp2.Controls.MarkdownRenderer.CustomContainers;
using UnityEngine.UIElements;

namespace UitkForKsp2.Controls.MarkdownRenderer.ObjectRenderers.BlockRenderers
{
    [UitkMarkdownRenderer]
    public class CustomContainerRenderer : MarkdownObjectRenderer<VisualElementRenderer, CustomContainer>
    {
        private Dictionary<string, ICustomContainer> _containers = new();

        public CustomContainerRenderer()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute(typeof(CustomContainerAttribute)) is CustomContainerAttribute attr)
                    {
                        _containers[attr.ContainerType] = (ICustomContainer)Activator.CreateInstance(type);
                    }
                }
            }
        }
        
        protected override void Write(VisualElementRenderer renderer, CustomContainer obj)
        {
            if (_containers.TryGetValue(obj.Info ?? "", out var container))
            {
                
                switch (container.Type)
                {
                    case CustomContainerType.Container:
                    {
                        var (block, content) = container.BuildContainer(obj.Arguments);
                        renderer.CurrentBlock.MainBlock.Add(block);
                        using var handle = renderer.AddBlock(content);
                        renderer.WriteChildren(obj);
                        break;
                    }
                    case CustomContainerType.Opaque:
                    {
                        var content = container.BuildOpaque(obj.Arguments, renderer, obj);
                        renderer.CurrentBlock.MainBlock.Add(content);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                using var block = renderer.PushBlock(obj.Info);
                renderer.WriteChildren(obj);
            }
            // if (obj.Info == "foldout")
            // {
            //     var foldout = new Foldout
            //     {
            //         text = obj.Arguments ?? ""
            //     };
            //     renderer.CurrentBlock.MainBlock.Add(foldout);
            //     renderer.AddBlock(foldout.contentContainer);
            //     renderer.WriteChildren(obj);
            // }
            // else
            // {
            //     using var block = renderer.PushBlock(obj.Info);
            //     renderer.WriteChildren(obj);
            // }
        }
    }
}