using AlphaPortal.Models;
using Business.Interfaces;
using Data.Entities;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaPortal.Controllers;

[Authorize(Roles = "Administrator")]
public class StatusController(IStatusService statusService) : Controller
{
    private readonly IStatusService _statusService = statusService;

    [Route("admin/status")]
    public async Task<IActionResult> Index()
    {
        var statuses = await _statusService.GetStatusesAsync();
        var statusList = statuses.Result?.Select(status => status.MapTo<StatusViewModel>()).ToList();

        var viewModel = new StatusesViewModel
        {
            Statuses = statusList ?? [],
            StatusViewModel = new StatusViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(StatusViewModel formData)
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
        var statusEntity = formData.MapTo<StatusEntity>();
        var result = await _statusService.CreateStatusAsync(statusEntity);

        if (result.Succeeded)
            return Ok();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(StatusViewModel formData)
    {

        if (!ModelState.IsValid)
        {
            var statuses = await _statusService.GetStatusesAsync();
            var statusesViewModel = new StatusesViewModel
            {
                Statuses = statuses.Result?.Select(s => s.MapTo<StatusViewModel>()).ToList()!,
                StatusViewModel = new StatusViewModel()
            };

            return View("Index", statusesViewModel);
        }

        var addStatusFormData = formData.MapTo<StatusEntity>();

        var result = await _statusService.UpdateStatusAsync(addStatusFormData);

        if (result.Succeeded)
            return RedirectToAction("Index");

        var statusesAfterError = await _statusService.GetStatusesAsync();
        var model = new StatusesViewModel
        {
            Statuses = statusesAfterError.Result?.Select(s => s.MapTo<StatusViewModel>()).ToList()!,
            StatusViewModel = new StatusViewModel()
        };

        return View("Index", model);
    }

 
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _statusService.DeleteStatusAsync(id);
        return RedirectToAction(nameof(Index));

    }


}
