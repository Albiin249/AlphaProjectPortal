using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlphaPortal.Controllers;

public class ExternalAuthController(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager) : Controller
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;

    [HttpPost]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            return RedirectToAction("Login", "Auth");
        }

        var redirectUrl = Url.Action("ExternalSignInCallback", "ExternalAuth", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider : {remoteError}");
            return RedirectToAction("Login", "Auth");
        }

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
            return RedirectToAction("Login", "Auth");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string firstName = string.Empty;
            string lastName = string.Empty;
            try
            {
                firstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                lastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname)!;
            }
            catch { }

            string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)!;
            string username = $"ext_{externalLoginInfo.LoginProvider.ToLower()}_{email}";

            var user = new UserEntity { UserName = username, Email = email, FirstName = firstName, LastName = lastName };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, externalLoginInfo);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Login", "Auth");

        }
    }
}