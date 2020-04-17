using Sitecore.Mvc.Common;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using SitecoreEmmetExtensions.Extensions;
using System.IO;
using System.Web.Mvc;

namespace SitecoreEmmetExtensions.Renderers
{
    public class EmmetRenderer : Renderer
    {
        public Rendering Rendering { get; set; }

        public override void Render(TextWriter writer)
        {
            var abbreviation = Rendering?.RenderingItem?.InnerItem["Abbreviation"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                return;
            }

            var sitecoreHelper = GetSitecoreHelper();
            var result = sitecoreHelper.RenderEmmetAbbreviation(abbreviation);
            writer.Write(result);
        }

        protected virtual SitecoreHelper GetSitecoreHelper()
        {
            var current = ContextService.Get().GetCurrent<ViewContext>();
            var viewDataContainer = new ViewDataContainer(current.ViewData);
            var htmlHelper = new HtmlHelper(current, viewDataContainer);
            return new SitecoreHelper(htmlHelper);
        }
    }
}
