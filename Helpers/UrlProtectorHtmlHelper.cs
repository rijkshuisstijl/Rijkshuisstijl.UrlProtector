#region

using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Localization;
using Rijkshuisstijl.UrlProtector.Models;

#endregion

namespace Rijkshuisstijl.UrlProtector.Helpers
{
    public static class UrlProtectorHtmlHelper
    {
        public static MvcHtmlString CreateFilteredRequestTable(this HtmlHelper helper, IQueryable<FilteredRequestRecord> filteredRequestRecords, Localizer T)
        {
            StringBuilder sb = new StringBuilder();

            if (filteredRequestRecords == null || !filteredRequestRecords.Any())
            {
                sb.AppendLine(T("No requests are filtered yet.").Text);
            }
            else
            {
                sb.AppendLine("<table><tr>");
                sb.AppendFormat("<th>{0}</th>", T("Request time"));
                sb.AppendFormat("<th>{0}</th>", T("Url"));
                sb.AppendFormat("<th>{0}</th>", T("User Host Address"));
                sb.AppendFormat("<th>{0}</th>", T("User Agent"));
                sb.AppendLine("</tr>");

                foreach (FilteredRequestRecord filteredRequest in filteredRequestRecords.OrderByDescending(r => r.RequestTime))
                {
                    sb.AppendLine("<tr>");
                    sb.AppendFormat("<td>{0}</td>", filteredRequest.RequestTime);
                    sb.AppendFormat("<td>{0}</td>", filteredRequest.Url);
                    sb.AppendFormat("<td>{0}</td>", filteredRequest.UserHostAddress);
                    sb.AppendFormat("<td>{0}</td>", filteredRequest.UserAgent);
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
            }
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString CreateUserInfoTable(this HtmlHelper helper, string currentUserHostAddress, string currentUserAgent, Localizer T)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table id='userinfo'><tr>");
            sb.AppendLine("");
            sb.AppendFormat("<td>{0}</td><td>{1}</td>", T("Current user host address:"), currentUserHostAddress);
            sb.AppendFormat("<td class='currentagent'>{0}</td><td>{1}</td>", T("Current user agent:"), currentUserAgent);
            sb.AppendLine("</tr> </table>");
            return new MvcHtmlString(sb.ToString());
        }
    }
}