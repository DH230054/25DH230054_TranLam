using System;
using System.Linq;
using System.Web;
using MyStore.Web.Models;

namespace MyStore.Web.Services
{
    /// <summary>
    /// Builds the shared navigation menu in the normal page request.
    /// Avoids a nested MVC child request from Html.Action in _CustomerLayout.
    /// </summary>
    public static class NavigationMenuService
    {
        public static PersonalMenuVM Build(HttpContextBase context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var identity = context.User == null ? null : context.User.Identity;
            var loggedIn = identity != null && identity.IsAuthenticated;
            var userName = loggedIn ? identity.Name : null;

            // Don't create or reset the cart. Display the current session's quantity.
            var session = context.Session;
            var cart = session == null ? null : session["MyStore.Cart"] as Cart;
            var result = new PersonalMenuVM
            {
                IsLoggedIn = loggedIn,
                Username = userName,
                CartCount = cart == null ? 0 : cart.Count,
                IsAdmin = false
            };

            if (loggedIn && !String.IsNullOrWhiteSpace(userName))
            {
                using (var db = new MyStoreEntities())
                {
                    result.IsAdmin = db.Users.Any(x => x.Username == userName && x.UserRole == "A");
                }
            }
            return result;
        }
    }
}
