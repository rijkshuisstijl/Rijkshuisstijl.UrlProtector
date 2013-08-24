#region

using System;

#endregion

namespace Rijkshuisstijl.UrlProtector.Models
{
    public class FilteredRequestRecord
    {
        public virtual Int32 Id { get; set; }
        public virtual DateTime RequestTime { get; set; }
        public virtual String Url { get; set; }
        public virtual String UserAgent { get; set; }
        public virtual String UserHostAddress { get; set; }
    }
}