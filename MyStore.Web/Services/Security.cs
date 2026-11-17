using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using MyStore.Web.Models;

namespace MyStore.Web.Services
{
    public static class PasswordHasher
    {
        const int Iterations = 120000;
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Mật khẩu trống");
            var salt = new byte[16]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var kdf = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                return "PBKDF2$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(kdf.GetBytes(32));
        }
        public static bool Verify(string password, string stored)
        {
            if (password == null || string.IsNullOrWhiteSpace(stored)) return false;
            var parts = stored.Trim().Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
            if (!int.TryParse(parts[1], out int iters) || iters < 100000 || iters > 1000000) return false;
            try
            {
                var expected = Convert.FromBase64String(parts[3]);
                using (var kdf = new Rfc2898DeriveBytes(password, Convert.FromBase64String(parts[2]), iters, HashAlgorithmName.SHA256))
                {
                    var given = kdf.GetBytes(expected.Length); int diff = 0;
                    for (int i = 0; i < expected.Length; i++) diff |= expected[i] ^ given[i];
                    return diff == 0;
                }
            }
            catch (FormatException) { return false; }
        }
    }
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            if (context?.User?.Identity?.IsAuthenticated != true) return false;
            using (var db = new MyStoreEntities())
                return db.Users.Any(x => x.Username == context.User.Identity.Name && x.UserRole == "A");
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext ctx)
        {
            if (ctx.HttpContext.User.Identity.IsAuthenticated)
                ctx.Result = new HttpStatusCodeResult(403,"Tài khoản không có quyền quản trị.");
            else base.HandleUnauthorizedRequest(ctx);
        }
    }
}
