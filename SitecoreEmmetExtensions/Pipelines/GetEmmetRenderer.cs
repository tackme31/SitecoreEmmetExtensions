using Sitecore.Data;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using SitecoreEmmetExtensions.Renderers;

namespace SitecoreEmmetExtensions.Pipelines
{
    public class GetEmmetRenderer : GetRendererProcessor
    {
        public override void Process(GetRendererArgs args)
        {
            if (args.Result != null)
            {
                return;
            }

            var templateName = args.Rendering?.RenderingItem?.InnerItem.TemplateName;
            if (templateName != "Emmet Rendering")
            {
                return;
            }

            args.Result = new EmmetRenderer()
            {
                Rendering = args.Rendering
            };
        }
    }
}
