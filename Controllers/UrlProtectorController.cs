using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Rijkshuisstijl.UrlProtector.Filters;
using Rijkshuisstijl.UrlProtector.Models;
using Rijkshuisstijl.UrlProtector.Services;
using Rijkshuisstijl.UrlProtector.ViewModels;
using Rijkshuisstijl.UrlProtector.ViewModels.UrlProtector;

namespace Rijkshuisstijl.UrlProtector.Controllers {
    [Admin]
    public class UrlProtectorController : Controller {
        private readonly IAuthorizer _authorizer;
        private readonly ISignals _signals;
        private readonly ICachedUrlProtectorRules _cachedUrlProtectorRules;
        private readonly IRepository<DashboardFilterRecord> _dashboardFilterRecords;
        private readonly IRepository<FilteredRequestRecord> _filteredRequestRecords;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UrlFilterRecord> _urlFilterRecords;
        private readonly IWorkContextAccessor _workContextAccessor;

        public UrlProtectorController(
            ICachedUrlProtectorRules cachedUrlProtectorRules,
            IRepository<DashboardFilterRecord> dashboardFilterRecords,
            IRepository<UrlFilterRecord> urlFilterRecords,
            IRepository<FilteredRequestRecord> filteredRequestRecords,
            IOrchardServices orchardServices,
            IWorkContextAccessor workContextAccessor,
            IAuthorizer authorizer,
            ISignals signals) {
            _cachedUrlProtectorRules = cachedUrlProtectorRules;
            _dashboardFilterRecords = dashboardFilterRecords;
            _urlFilterRecords = urlFilterRecords;
            _filteredRequestRecords = filteredRequestRecords;
            _orchardServices = orchardServices;
            _workContextAccessor = workContextAccessor;
            _authorizer = authorizer;
            _signals = signals;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpGet]
        public ActionResult Show() {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }
            var filter = new UrlFilter(_workContextAccessor,_filteredRequestRecords, _cachedUrlProtectorRules);

            var viewModel = new ShowViewModel(_cachedUrlProtectorRules.DashboardFilterRecords, _cachedUrlProtectorRules.UrlFilterRecords, _filteredRequestRecords.Table, filter.UserHostAddress, filter.UserAgent);


            //var urlFilterModelView = new UrlFilterModelView(_cachedUrlProtectorRules.DashboardFilterRecords, _cachedUrlProtectorRules.UrlFilterRecords, _filteredRequestRecords.Table, filter.UserHostAddress, filter.UserAgent);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult NewFilter() {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            //Get the last content item record id
            int lastId = (from urlfilter in _urlFilterRecords.Table
                          orderby urlfilter.Id descending
                          select urlfilter.Id).FirstOrDefault();

            int newId = lastId + 1;
            var filterUrl = new UrlFilterRecord {Id = newId, UrlPriority = 50};
            _signals.Trigger(CachedUrlProtectorRules.SignalUpdateUrlFilterRecordsTrigger);
            var viewModel = new EditUrlViewModel {
                UrlFilterRecord = filterUrl
            };

            return View("EditUrl", viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int urlFilterId) {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            UrlFilterRecord urFilter = _urlFilterRecords.Get(urlFilterId);
            if (urFilter == null) {
                return new HttpNotFoundResult("Could not find the urlFilter with id " + urlFilterId);
            }
            _urlFilterRecords.Delete(urFilter);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The urlfilter with pattern {0} has been deleted", urFilter.UrlPattern));
            _signals.Trigger(CachedUrlProtectorRules.SignalUpdateUrlFilterRecordsTrigger);
            return RedirectToAction("Show");
        }

        [HttpGet]
        public ActionResult Edit(int urlFilterid) {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            UrlFilterRecord urlFilter = _urlFilterRecords.Get(urlFilterid);
            if (urlFilter == null) {
                return new HttpNotFoundResult("Could not find the url filter with id " + urlFilterid);
            }
            var viewModel = new EditUrlViewModel {
                UrlFilterRecord = urlFilter
            };

            return View("EditUrl", viewModel);
        }


        [HttpPost, ActionName("Edit")]
        public ActionResult SaveFilter(EditUrlViewModel viewModel) {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            //Validate the values for valid regex patterns if a value is supplied
            if (!String.IsNullOrEmpty(viewModel.UrlFilterRecord.UrlPattern) && !IsValidRegex(viewModel.UrlFilterRecord.UrlPattern))
            {
                ModelState.AddModelError(String.Empty, "Url pattern is not a valid regex expression");
            }

            if (!String.IsNullOrEmpty(viewModel.UrlFilterRecord.UserHostAddressPattern) && !IsValidRegex(viewModel.UrlFilterRecord.UserHostAddressPattern))
            {
                ModelState.AddModelError(String.Empty, "Userhostaddress pattern is not a valid regex expression");
            }

            if (!String.IsNullOrEmpty(viewModel.UrlFilterRecord.UserAgentPattern) && !IsValidRegex(viewModel.UrlFilterRecord.UserAgentPattern))
            {
                ModelState.AddModelError(String.Empty, "Useragent pattern is not a valid regex expression");
            }

            if (viewModel.UrlFilterRecord.UrlPriority < 1 || viewModel.UrlFilterRecord.UrlPriority > 99)
            {
                ModelState.AddModelError(String.Empty, "Priority must be between 1 and 99");
            }

            if (!ModelState.IsValid) {
                return View("EditUrl", viewModel);
            }


            _urlFilterRecords.Update(viewModel.UrlFilterRecord);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The url filter with pattern {0} has been saved", viewModel.UrlFilterRecord.UrlPattern));
            _signals.Trigger(CachedUrlProtectorRules.SignalUpdateUrlFilterRecordsTrigger);
            return RedirectToAction("Show");
        }

        /// <summary>
        ///     Check if pattern is a valid Regex expression by creating the regex and catching the exception if it occures
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [NonAction]
        private static Boolean IsValidRegex(String pattern) {
            bool returnValue = false;
            try {
                new Regex(pattern);
                returnValue = true;
            }
            catch (ArgumentException) {}
            return returnValue;
        }

        [HttpGet]
        public ActionResult EditDashboard() {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            //Only one item for the dashboard (always id = 1)
            var dashboardFilterRecord = _dashboardFilterRecords.Get(1);
            if (dashboardFilterRecord == null) {
                return new HttpNotFoundResult("Could not find the dashboard filter");
            }

            var viewModel = new EditDashboardViewModel {DashboardFilterRecord = dashboardFilterRecord};
            return View("EditDashboard", viewModel);
        }

        [HttpPost, ActionName("EditDashboard")]
        public ActionResult SaveDashboardFilter(EditDashboardViewModel viewModel) {
            if (!_authorizer.Authorize(Permissions.ConfigureUrlProtector, T("Not authorized to manage settings"))) {
                return new HttpUnauthorizedResult();
            }

            //Validate the values for valid regex patterns if a value is supplied

            if (!String.IsNullOrEmpty(viewModel.DashboardFilterRecord.UserHostAddressPattern) && !IsValidRegex(viewModel.DashboardFilterRecord.UserHostAddressPattern)) {
                ModelState.AddModelError(String.Empty, "Userhostaddress pattern is not a valid regex expression");
            }

            if (!String.IsNullOrEmpty(viewModel.DashboardFilterRecord.UserAgentPattern) && !IsValidRegex(viewModel.DashboardFilterRecord.UserAgentPattern)) {
                ModelState.AddModelError(String.Empty, "Useragent pattern is not a valid regex expression");
            }

            if (!ModelState.IsValid) {
               
                return View("EditDashboard", viewModel);
            }


            _dashboardFilterRecords.Update(viewModel.DashboardFilterRecord);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The dashboard filter has been saved"));
            _signals.Trigger(CachedUrlProtectorRules.SignalUpdateUrlFilterRecordsTrigger);
            return RedirectToAction("Show");
        }
    }
}