namespace NexTaskScheduler.WindowsService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IOptionsSnapshot<Identify> auth;

        public AuthController(IOptionsSnapshot<Identify> auth)
        {
            this.auth = auth;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var isValid = IsValidUser(username, password);

            if (isValid)
            {
                await DoCheckIn(username);
                return Redirect("~/hangfire");
            }

            ViewBag.IsInvalid = !isValid;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Login");
        }

        private async Task DoCheckIn(string username)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.NameIdentifier, username)
                        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private bool IsValidUser(string username, string password)
        {
            bool isValid = false;
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var encryptedPassword = Cryptography.EncryptPassword(password);
                isValid = username.Equals(auth.Value.UserName, StringComparison.OrdinalIgnoreCase) &&
                   encryptedPassword.Equals(auth.Value.Password);
            }

            return isValid;
        }
    }
}