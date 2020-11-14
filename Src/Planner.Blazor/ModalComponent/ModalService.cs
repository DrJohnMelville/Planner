using System;
using Microsoft.AspNetCore.Components;

namespace Planner.Blazor.ModalComponent
{
    public class ModalService
    {
        public event Action<string, RenderFragment>? OnShow;
        public event Action? OnClose;
        

        public void Show<T>(string title, params (string Name, object Value)[] parameters) 
            where T : ComponentBase => Show(title, typeof(T), parameters);
        public void Show(string title, Type contentType, params (string Name, object Value)[] parameters)
        {
            if (contentType.BaseType != typeof(ComponentBase))
            {
                throw new ArgumentException($"{contentType.FullName} must be a Blazor Component");
            }

            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, contentType);
                int i = 2;
                foreach (var parameter in parameters)
                {
                    x.AddAttribute(i++, parameter.Name, parameter.Value);
                }
                x.CloseComponent();
            });

            OnShow?.Invoke(title, content);
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}