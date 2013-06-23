using System.Collections.Generic;
using System.Linq;
using Orchard;
using Rijkshuisstijl.UrlProtector.Models;

namespace Rijkshuisstijl.UrlProtector.Services {
    public interface ICachedUrlProtectorRules : IDependency {
        IQueryable<UrlFilterRecord> UrlFilterRecords { get; }
        IQueryable<DashboardFilterRecord> DashboardFilterRecords { get; }
    }
}