using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.WEB.Helpers
{


    public static class PaginationHelper
    {
        public static IHtmlContent GeneratePaginationLinks(
            IUrlHelper urlHelper,
            int totalPages,
            int currentPage,
            string action,
            object routeValues = null)
        {
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");

            ul.InnerHtml.AppendHtml(CreatePageLink(urlHelper, action, routeValues, 1, "<<", currentPage == 1, currentPage));

            ul.InnerHtml.AppendHtml(CreatePageLink(urlHelper, action, routeValues, currentPage - 1, "<", currentPage == 1, currentPage));

            for (var i = 1; i <= totalPages; i++)
            {
                ul.InnerHtml.AppendHtml(CreatePageLink(urlHelper, action, routeValues, i, i.ToString(), i == currentPage, currentPage));
            }

            ul.InnerHtml.AppendHtml(CreatePageLink(urlHelper, action, routeValues, currentPage + 1, ">", currentPage == totalPages, currentPage));

            ul.InnerHtml.AppendHtml(CreatePageLink(urlHelper, action, routeValues, totalPages, ">>", currentPage == totalPages, currentPage));

            var writer = new System.IO.StringWriter();
            ul.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

        private static TagBuilder CreatePageLink(
            IUrlHelper urlHelper,
            string action,
            object routeValues,
            int pageNumber,
            string text,
            bool isDisabled,
            int currentPage)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (isDisabled)
            {
                li.AddCssClass("disabled");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.InnerHtml.AppendHtml(text);

            if (!isDisabled)
            {
                var newRouteValues = new RouteValueDictionary(routeValues)
                {
                    ["PageNumber"] = pageNumber
                };
                a.Attributes["href"] = urlHelper.Action(action, newRouteValues);
            }

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}