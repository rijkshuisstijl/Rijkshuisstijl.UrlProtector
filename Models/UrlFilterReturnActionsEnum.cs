#region

using System.ComponentModel;

#endregion

namespace Rijkshuisstijl.UrlProtector.Models
{
    public enum UrlFilterReturnActionsEnum
    {
        [Description("No action")] NoAction = 0,
        [Description("Page not found")] NotFound = 1,
        [Description("Access denied")] AccessDenied = 2,
        [Description("In maintenance")] InMaintenance = 3,
        [Description("Redirect")] Redirect = 4
    }
}