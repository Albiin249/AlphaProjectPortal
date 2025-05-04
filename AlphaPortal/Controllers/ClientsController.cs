using AlphaPortal.Models;
using Business.Interfaces;
using Business.Services;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaPortal.Controllers;

[Authorize]
public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    public async Task<IActionResult> Index()
    {
        var clients = await _clientService.GetClientsAsync();
        var clientsViewModel = clients.Result?.Select(client => client.MapTo<ClientViewModel>()).ToList();

        var model = new ClientsViewModel
        {
            Clients = clientsViewModel!,
            AddClientFormData = new AddClientViewModel(),
            EditClientFormData = new EditClientViewModel()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(AddClientFormData formData)
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

        var result = await _clientService.CreateClientAsync(formData);
        
        if (result.Succeeded)
            return Ok();

        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditClientViewModel formData)
    {

        if (!ModelState.IsValid)
        {
            var clients = await _clientService.GetClientsAsync();
            var clientsViewModel = new ClientsViewModel
            {
                Clients = clients.Result?.Select(c => c.MapTo<ClientViewModel>()).ToList()!,
                AddClientFormData = new AddClientViewModel(),
                EditClientFormData = formData 
            };

            return View("Index", clientsViewModel);
        }

        var addClientFormData = formData.MapTo<AddClientFormData>();

        var result = await _clientService.UpdateClientAsync(addClientFormData);

        if (result.Succeeded)
            return RedirectToAction("Index");

        var clientsAfterError = await _clientService.GetClientsAsync();
        var model = new ClientsViewModel
        {
            Clients = clientsAfterError.Result?.Select(c => c.MapTo<ClientViewModel>()).ToList()!,
            AddClientFormData = new AddClientViewModel(),
            EditClientFormData = formData 
        };

        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Index");

        var result = await _clientService.DeleteClientAsync(id);

        if (!result.Succeeded)
        {
            var clients = await _clientService.GetClientsAsync();
            var model = new ClientsViewModel
            {
                Clients = clients.Result?.Select(c => c.MapTo<ClientViewModel>()).ToList()!,
                AddClientFormData = new AddClientViewModel(),
                EditClientFormData = new EditClientViewModel()
            };

            ViewData["ErrorMessage"] = result.Error;
            return View("Index", model);
        }

        return RedirectToAction("Index");
    }

}
