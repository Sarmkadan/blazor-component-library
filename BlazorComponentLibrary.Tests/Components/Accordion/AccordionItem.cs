using Microsoft.AspNetCore.Components;

namespace BlazorComponentLibrary.Tests.Components.Accordion
{
    public class AccordionItem : ComponentBase
    {
        [Parameter]
        public bool IsInitiallyExpanded { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public bool IsExpanded { get; private set; }

        public void SetExpanded(bool expanded)
        {
            IsExpanded = expanded;
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", IsExpanded ? "expanded" : string.Empty);
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
