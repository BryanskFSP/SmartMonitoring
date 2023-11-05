using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Client.Pages.Admin.Wikies;

public partial class WikiesPage
{
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IWikiController ModelController { get; set; }

    private List<WikiViewModel>? Models;
    private string SearchString = "";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Models = await ModelController.GetFull();
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

    private async Task DeleteModel(WikiViewModel model)
    {
        var result = await DialogService.ShowMessageBox("Подтверждение",
            $"Вы точно хотите удалить {model.Name}?", "Да", "Нет");

        if (!result.GetValueOrDefault(false))
        {
            return;
        }

        await ModelController.Delete(model.ID);
        Models = await ModelController.GetAll();
        Snackbar.Add("Wiki успешно удалён!", Severity.Success);
    }

    private bool FilterFuncInput(WikiViewModel model) => FilterFunc(model, SearchString);

    private bool FilterFunc(WikiViewModel model, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (model.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        //  {model.Group?.Name} 
        if ($"{string.Join(", ", model.WikiSolutions?.Select(x => x.Name))} {model.Type.GetAttribute<DisplayAttribute>().Name}"
            .Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
}