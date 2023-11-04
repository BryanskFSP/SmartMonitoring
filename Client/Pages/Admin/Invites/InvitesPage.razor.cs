using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;
using Serilog;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Client.Pages.Admin.Invites;

public partial class InvitesPage
{
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IInviteController ModelController { get; set; }

    private List<InviteViewModel>? Models;
    private string SearchString = "";
    
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Models = await ModelController.GetAll();
        }
        catch (ValidationApiException validationException)
        {
            Snackbar.Add(validationException.Content.Detail, Severity.Error);
            Log.Error(validationException, "Validation error!");
        }
        catch (ApiException exception)
        {
            Snackbar.Add($"Ошибка\n{exception.RequestMessage.Content}", Severity.Error);
            Log.Error(exception, "Exception!");
        }
    }
    
    private async Task DeleteModel(InviteViewModel model)
    {
        var result = await DialogService.ShowMessageBox("Подтверждение",
            $"Вы точно хотите удалить с кодом {model.Code}?", "Да", "Нет");

        if (!result.GetValueOrDefault(false))
        {
            return;
        }

        await ModelController.Delete(model.ID);
        Models = await ModelController.GetAll();
        Snackbar.Add("Приглашение успешно удалено!", Severity.Success);
    }

    private bool FilterFuncInput(InviteViewModel model) => FilterFunc(model, SearchString);

    private bool FilterFunc(InviteViewModel model, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (model.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        //  {model.Group?.Name} 
        if ($"{model.Organization?.Name}"
            .Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
}