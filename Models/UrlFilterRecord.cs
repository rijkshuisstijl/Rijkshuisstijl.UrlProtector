#region

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Rijkshuisstijl.UrlProtector.Models
{
    public class UrlFilterRecord
    {
        public virtual Int32 Id { get; set; }

        [Required(ErrorMessage = "Url pattern is required")]
        public virtual String UrlPattern { get; set; }

        [Required(ErrorMessage = "Priority is required (1-99)")]
        public virtual Int32 UrlPriority { get; set; }

        [Required(ErrorMessage = "Userhostaddress pattern is required")]
        public virtual String UserHostAddressPattern { get; set; }

        [Required(ErrorMessage = "Useragent pattern is required")]
        public virtual String UserAgentPattern { get; set; }

        public virtual String Description { get; set; }
        public virtual Boolean ForceSsl { get; set; }
        [Required(ErrorMessage = "Failure action must be selected")]
        [Range(0,4,ErrorMessage = "Failure action is invalid")]
        public virtual Int32 FailureAction { get; set; }
        public virtual String RedirectTo { get; set; }
    }
}