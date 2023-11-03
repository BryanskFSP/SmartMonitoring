using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Client.Pages.Admin.DataBases;

public partial class DataBasesPage
{
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IDataBaseController ModelController { get; set; }

    private List<DataBaseViewModel>? Models;
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
        }
        catch (ApiException exception)
        {
            Snackbar.Add($"Ошибка\n{exception.RequestMessage.Content}", Severity.Error);
        }
    }

    private async Task DeleteModel(DataBaseViewModel model)
    {
        var result = await DialogService.ShowMessageBox("Подтверждение",
            $"Вы точно хотите удалить {model.Name}?", "Да", "Нет");

        if (!result.GetValueOrDefault(false))
        {
            return;
        }

        await ModelController.Delete(model.ID);
        Models = await ModelController.GetAll();
        Snackbar.Add("База данных успешно удалён!", Severity.Success);
    }

    private bool FilterFuncInput(DataBaseViewModel model) => FilterFunc(model, SearchString);

    private bool FilterFunc(DataBaseViewModel model, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (model.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        //  {model.Group?.Name} 
        if ($"{model.Description}"
            .Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
}