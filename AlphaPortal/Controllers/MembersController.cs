using AlphaPortal.Hubs;
using AlphaPortal.Models;
using Business.Interfaces;
using Data.Entities;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AlphaPortal.Controllers;

[Authorize(Roles = "Administrator")]

public class MembersController(IUserService userService, INotificationService notificationService, IHubContext<NotificationHub> notificationHub, UserManager<UserEntity> userManager)  : Controller
{
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly UserManager<UserEntity> _userManager = userManager;
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetUsersAsync();
        var memberViewModel = users.Result.Select(u =>
        {
            var memberViewModel = u.MapTo<User>();
            if (!string.IsNullOrEmpty(memberViewModel.ProfileImage))
                memberViewModel.ProfileImage = u.ProfileImage;
            else
                memberViewModel.ProfileImage = "images/projects/project-template.svg";
            return memberViewModel;
        }).ToList();



        foreach (var user in users.Result)
        {
            var address = user.Address != null ? user.Address.Address : "Ingen adress";
        }

        var viewModel = new MemberViewModel
        {
            User = users.Result,
            AddMemberViewModel = new AddMemberViewModel()
        };
        return View(viewModel); 
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddMemberViewModel model)
    {

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, errors });
        }

        string? imagePath = null;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var originalFileName = Path.GetFileName(model.ProfileImage.FileName);
            var guid = Guid.NewGuid().ToString();
            var shortGuid = guid.Substring(0, 10);
            var uniqueFileName = $"{shortGuid}_{originalFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(stream);
            }

            imagePath = Path.Combine("images", "users", uniqueFileName).Replace("\\", "/");
        }

        var formData = model.MapTo<AddMemberFormData>();
        formData.ProfileImage = imagePath;
        var result = await _userService.CreateMemberAsync(formData);

        if (result.Succeeded)
        {
            var user = await _userService.GetUserAsync(formData.Email!);
            if (user != null)
            {
                var notificationEntity = new NotificationEntity
                {
                    Message = $"User {formData.FirstName} {formData.LastName!} has been added.",
                    NotificationTypeId = 1,
                    NotificationTargetGroupId = 2
                };
                await _notificationService.AddNotificationAsync(notificationEntity);
                var notifications = await _notificationService.GetNotificationsAsync(formData.Id!);
                var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

                if (newNotification != null)
                {
                    await _notificationHub.Clients.Group("Admins").SendAsync("AdminReceiveNotification", newNotification);
                }
            }
            return Ok();
        }
            

        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Index");

        var result = await _userService.DeleteMemberAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = "Member not deleted, might be because it's connected to a project. If not contact support";
        }

        return RedirectToAction("Index");
    }

    //Tog hjälp från ChatGPT för att kunna ändra bild i editmodalen.
    [HttpPost]
    public async Task<IActionResult> Edit(EditMemberViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, errors });
        }

        string? imagePath = model.ExistingProfileImagePath;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
        
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var originalFileName = Path.GetFileName(model.ProfileImage.FileName);
            var guid = Guid.NewGuid().ToString();
            var shortGuid = guid.Substring(0, 10);
            var uniqueFileName = $"{shortGuid}_{originalFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(stream);
            }

            imagePath = Path.Combine("images", "users", uniqueFileName).Replace("\\", "/");
        }

        var addMemberFormData = model.MapTo<AddMemberFormData>();
        addMemberFormData.ProfileImage = imagePath;
        var result = await _userService.UpdateUserAsync(addMemberFormData);

        if (result.Succeeded)
            return Ok();
       
        Console.WriteLine("Något gick fel..");
        return Json(new { success = false, message = "Något gick fel vid uppdateringen." });
    }
}
