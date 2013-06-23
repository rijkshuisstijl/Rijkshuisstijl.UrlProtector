using Orchard;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Rijkshuisstijl.UrlProtector {
    public class AdminMenu : Component, INavigationProvider {

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .Add(item => item
                                 .Caption(T("Url Protector"))
                                 .Position("8.0")
                                 .Action("Show", "UrlProtector", new {area = "Rijkshuisstijl.UrlProtector"})
                                 .Permission(Permissions.ConfigureUrlProtector));
        }
    }

}