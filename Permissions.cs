using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Rijkshuisstijl.UrlProtector {
    public class Permissions : IPermissionProvider {

        public static readonly Permission ConfigureUrlProtector = new Permission {
            Description = "Show and edit the settings for the Url protector",
            Name = "Configure Url protector"
        };
        public Feature Feature { get; set; }
        public IEnumerable<Permission> GetPermissions() {
            return new[] {ConfigureUrlProtector};
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {
                        ConfigureUrlProtector
                    }
                }
            };
        }
    }
}