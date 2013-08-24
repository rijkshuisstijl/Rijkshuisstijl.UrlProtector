#region

using System.Linq;

#endregion

namespace Rijkshuisstijl.UrlProtector.Models
{
    public class UrlProtectorCacheStore
    {
        public IQueryable<UrlFilterRecord> UrlFilterRecords { get; set; }
        public IQueryable<DashboardFilterRecord> DashboardFilterRecords { get; set; }
    }
}