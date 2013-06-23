using System;
using System.Linq;
using JetBrains.Annotations;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Services;
using Rijkshuisstijl.UrlProtector.Models;

namespace Rijkshuisstijl.UrlProtector.Services {
    public class CachedUrlProtectorRules : ICachedUrlProtectorRules {
        private readonly IRepository<DashboardFilterRecord> _dashboardFilterRecords;
        private readonly IRepository<UrlFilterRecord> _urlFilterRecords;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IClock _clock;

        public const string SignalUpdateUrlFilterRecordsTrigger = "SignalUpdateUrlProtectorFilterRecords";

        public CachedUrlProtectorRules(IRepository<DashboardFilterRecord> dashboardFilterRecords, IRepository<UrlFilterRecord> urlFilterRecords, ICacheManager cacheManager, ISignals signals, IClock clock) {
            _dashboardFilterRecords = dashboardFilterRecords;
            _urlFilterRecords = urlFilterRecords;
            _cacheManager = cacheManager;
            _signals = signals;
            _clock = clock;
        }

        public IQueryable<UrlFilterRecord> UrlFilterRecords {
            get { return UrlProtectorCacheStore.UrlFilterRecords; }

        }

        public IQueryable<DashboardFilterRecord> DashboardFilterRecords
        {
            get { return UrlProtectorCacheStore.DashboardFilterRecords; }
        }

        private UrlProtectorCacheStore UrlProtectorCacheStore {
            get {
                var protectorRecordCache = _cacheManager.Get("Rijkshuisstijl.UrlProtector.UrlFilterRecords", ctx => {
                    var newCache = new UrlProtectorCacheStore();
                    ctx.Monitor(_clock.When(TimeSpan.FromMinutes(60)));
                    ctx.Monitor(_signals.When(SignalUpdateUrlFilterRecordsTrigger));

                    var dashboardrecords = (from record in _dashboardFilterRecords.Table
                                            select record).ToList().AsQueryable();

                    var urlfilterrecords = (from record in _urlFilterRecords.Table
                                            select record).ToList().AsQueryable();


                    newCache.DashboardFilterRecords = dashboardrecords;
                    newCache.UrlFilterRecords = urlfilterrecords;
                    return newCache;
                });
                return protectorRecordCache;

            }
        }

    }
}