using AlphaPortal.Hubs;
using AlphaPortal.Models;
using Business.Interfaces;
using Business.Services;
using Data.Entities;
using Domain.Dtos;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SqlServer.Server;
using System.Runtime.InteropServices.Marshalling;

namespace AlphaPortal.Controllers;

public class AuthController(IAuthService authService, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, INotificationService notificationService, IHubContext<NotificationHub> notificationHub) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    [Route("auth/signup")]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [Route("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {

        if (!ModelState.IsValid)
            return View(model);

        var result =  await _authService.SignUpAsync(model);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var notificationEntity = new NotificationEntity
                {
                    Message = $"User {user.FirstName} has registered.",
                    NotificationTypeId = 1,
                    NotificationTargetGroupId = 2
                };
                await _notificationService.AddNotificationAsync(notificationEntity);
                var notifications = await _notificationService.GetNotificationsAsync(user.Id);
                var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

                if (newNotification != null)
                {
                    await _notificationHub.Clients.Group("Admins").SendAsync("AdminReceiveNotification", newNotification);

                }
            }
            return RedirectToAction("LogIn", "Auth");
        }
            

        ModelState.AddModelError(string.Empty, result.Error!);
        return View(model);

    }

    [Route("auth/login")]
    public IActionResult Login(string returnUrl = "~/")
    {
        Response.Cookies.Append("SessionCookie", "Essential", new CookieOptions
        {
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });

        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;  

        if(ModelState.IsValid)
        {
            var signInFormData = model.MapTo<SignInFormData>();
            var result = await _authService.SignInAsync(signInFormData);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var notificationEntity = new NotificationEntity
                    {
                        Message = $"{user.FirstName} {user.LastName} signed in.",
                        NotificationTypeId = 1
                    };
                    await _notificationService.AddNotificationAsync(notificationEntity);
                    var notifications = await _notificationService.GetNotificationsAsync(user.Id);
                    var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

                    if (newNotification != null)
                    {
                        await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
                    }
                }
                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ErrorMessage = "Incorrect email or password";
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("LogIn", "Auth");
    }

    [HttpGet]
    [Route("auth/adminlogin")]
    public IActionResult AdminLogin(string returnUrl = "~/admin/dashboard")
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = null;

        ViewBag.ReturnUrl = returnUrl;

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                ViewBag.ErrorMessage = "You are not authorized to access the admin panel.";
                return View("AdminLogin", model); 
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.IsPersistent, lockoutOnFailure: false);

            if (result.Succeeded)
                return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = "Incorrect email or password";
        return View("AdminLogin", model); 
    }
}

