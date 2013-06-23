using System;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.UrlProtector.Models
{
    public class DashboardFilterRecord
    {
        public virtual Int32 Id { get; set; }
        [Required(ErrorMessage = "Userhostaddress pattern is required")]
        public virtual String UserHostAddressPattern { get; set; }
        [Required(ErrorMessage = "Useragent pattern is required")]
        public virtual String UserAgentPattern { get; set; }
        public virtual Boolean ReturnStatusNotFound { get; set; }
        public virtual Boolean ForceSsl { get; set; }
    }
}

