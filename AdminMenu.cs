#region

using Orchard;
using Orchard.UI.Navigation;

#endregion

namespace Rijkshuisstijl.UrlProtector
{
    public class AdminMenu : Component, INavigationProvider
    {
        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Rijkshuisstijl"), "2", LinkSubMenu);
        }

        private void LinkSubMenu(NavigationBuilder menu)
        {
            menu.Add(item => item
                .Caption(T("Url Protector"))
                .Position("8.0")
                .Action("Show", "UrlProtector", new {area = "Rijkshuisstijl.UrlProtector"})
                .Permission(Permissions.ConfigureUrlProtector));
        }
    }
}