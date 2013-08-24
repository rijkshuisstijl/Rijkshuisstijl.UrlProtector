#region

using System;
using System.Linq;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Services;
using Rijkshuisstijl.UrlProtector.Models;

#endregion

namespace Rijkshuisstijl.UrlProtector.Services
{
    public class CachedUrlProtectorRules : ICachedUrlProtectorRules
    {
        public const string SignalUpdateUrlFilterRecordsTrigger = "SignalUpdateUrlProtectorFilterRecords";
        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;
        private readonly IRepository<DashboardFilterRecord> _dashboardFilterRecords;
        private readonly ISignals _signals;
        private readonly IRepository<UrlFilterRecord> _urlFilterRecords;

        public CachedUrlProtectorRules(IRepository<DashboardFilterRecord> dashboardFilterRecords, IRepository<UrlFilterRecord> urlFilterRecords, ICacheManager cacheManager, ISignals signals, IClock clock)
        {
            _dashboardFilterRecords = dashboardFilterRecords;
            _urlFilterRecords = urlFilterRecords;
            _cacheManager = cacheManager;
            _signals = signals;
            _clock = clock;
        }

        private UrlProtectorCacheStore UrlProtectorCacheStore
        {
            get
            {
                UrlProtectorCacheStore protectorRecordCache = _cacheManager.Get("Rijkshuisstijl.UrlProtector.UrlFilterRecords", ctx =>
                {
                    UrlProtectorCacheStore newCache = new UrlProtectorCacheStore();
                    ctx.Monitor(_clock.When(TimeSpan.FromMinutes(60)));
                    ctx.Monitor(_signals.When(SignalUpdateUrlFilterRecordsTrigger));

                    IQueryable<DashboardFilterRecord> dashboardrecords = (from record in _dashboardFilterRecords.Table
                        select record).ToList().AsQueryable();

                    IQueryable<UrlFilterRecord> urlfilterrecords = (from record in _urlFilterRecords.Table
                        select record).ToList().AsQueryable();


                    newCache.DashboardFilterRecords = dashboardrecords;
                    newCache.UrlFilterRecords = urlfilterrecords;
                    return newCache;
                });
                return protectorRecordCache;
            }
        }

        public IQueryable<UrlFilterRecord> UrlFilterRecords
        {
            get { return UrlProtectorCacheStore.UrlFilterRecords; }
        }

        public IQueryable<DashboardFilterRecord> DashboardFilterRecords
        {
            get { return UrlProtectorCacheStore.DashboardFilterRecords; }
        }
    }
}