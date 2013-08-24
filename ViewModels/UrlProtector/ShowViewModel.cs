#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Rijkshuisstijl.UrlProtector.Models;

#endregion

namespace Rijkshuisstijl.UrlProtector.ViewModels.UrlProtector
{
    public class ShowViewModel
    {
        public ShowViewModel(IQueryable<DashboardFilterRecord> dashboardFilterRecords, IQueryable<UrlFilterRecord> urlFilterRecords, IQueryable<FilteredRequestRecord> filteredRequestRecords, string currentHostAddress, string currentUserAgent)
        {
            DashboardFilterRecords = dashboardFilterRecords;
            UrlFilterRecords = urlFilterRecords;
            FilteredRequestRecords = filteredRequestRecords;
            CurrentUserHostAddress = currentHostAddress;
            CurrentUserAgent = currentUserAgent;
        }

        public IQueryable<UrlFilterRecord> UrlFilterRecords { get; private set; }
        public IQueryable<FilteredRequestRecord> FilteredRequestRecords { get; private set; }
        public IQueryable<DashboardFilterRecord> DashboardFilterRecords { get; private set; }
        public string CurrentUserHostAddress { get; private set; }
        public string CurrentUserAgent { get; private set; }

        public List<String> MatchedUserHostAddressPatterns
        {
            get
            {
                List<string> matches = new List<String>();
                if (String.IsNullOrEmpty(CurrentUserHostAddress))
                {
                    return matches;
                }

                foreach (DashboardFilterRecord dashboardFilterRecord in DashboardFilterRecords)
                {
                    Regex pattern = new Regex(dashboardFilterRecord.UserHostAddressPattern, RegexOptions.IgnoreCase);
                    if (pattern.IsMatch(CurrentUserHostAddress))
                    {
                        matches.Add(dashboardFilterRecord.UserHostAddressPattern);
                    }
                }

                foreach (UrlFilterRecord urlFilterRecord in UrlFilterRecords)
                {
                    Regex pattern = new Regex(urlFilterRecord.UserHostAddressPattern, RegexOptions.IgnoreCase);
                    if (pattern.IsMatch(CurrentUserHostAddress))
                    {
                        matches.Add(urlFilterRecord.UserHostAddressPattern);
                    }
                }

                return matches;
            }
        }

        public List<String> MatchedUserAgentPatterns
        {
            get
            {
                List<string> matches = new List<String>();
                if (String.IsNullOrEmpty(CurrentUserAgent))
                {
                    return matches;
                }

                foreach (UrlFilterRecord urlFilterRecord in UrlFilterRecords)
                {
                    Regex pattern = new Regex(urlFilterRecord.UserAgentPattern.ToLower());
                    if (pattern.IsMatch(CurrentUserAgent.ToLower()))
                    {
                        matches.Add(urlFilterRecord.UserAgentPattern);
                    }
                }

                foreach (DashboardFilterRecord dashboardFilterRecord in DashboardFilterRecords)
                {
                    Regex pattern = new Regex(dashboardFilterRecord.UserAgentPattern.ToLower());
                    if (pattern.IsMatch(CurrentUserAgent.ToLower()))
                    {
                        matches.Add(dashboardFilterRecord.UserAgentPattern);
                    }
                }

                return matches;
            }
        }
    }
}