using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FormatWith;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.Extensions;
using SmartMonitoring.Shared.Interfaces.Refit;
using SmartMonitoring.Shared.Models;

namespace SmartMonitoring.Client.Components.Wiki;

public partial class WikiEditCard
{
    [Inject] private IMapper Mapper { get; set; }

    [Inject] private IWikiController ModelController { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Parameter]
    public Guid? ID { get; set; }

    public bool IsEdit => ID != null;
    public string PageTitle => IsEdit ? "Изменение Wiki" : "Создание Wiki";
    
    private WikiEditModel Model = new();
    private WikiSolutionEditModel WikiSolutionBeforeEdit;

    [Parameter]
    public bool UseNavigations { get; set; } = false;

    private string _navigationUrl = "/admin/wikies/{ID}/edit";

    [Parameter]
    public string? NavigationUrl
    {
        get => _navigationUrl.FormatWith(new { ID });
        set => _navigationUrl = value ?? string.Empty;
    }


    private List<ActionType> ActionTypes = new(Enum.GetValues<ActionType>());

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (IsEdit)
            {
                Model = Mapper.Map<WikiEditModel>(await ModelController.GetByIDFull(ID.GetValueOrDefault()));
            }
        }
        catch (ValidationApiException validationException)
        {
            Snackbar.Add(validationException.Content.Detail, Severity.Error);
        }
        catch (ApiException exception)
        {
            Snackbar.Add(exception.Content, Severity.Error);
        }
        catch (Exception exception)
        {
            Snackbar.Add(exception.Message, Severity.Error);
        }
    }

    private async Task<IEnumerable<ActionType>> SearchActionType(string arg)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            return ActionTypes;
        }

        return ActionTypes.Where(x =>
            x.GetAttribute<DisplayAttribute>().Name
                .Contains(arg, StringComparison.InvariantCultureIgnoreCase));
    }

    private void AddWikiSolution()
    {
        Model.WikiSolutions.Add(new()
        {
        });
        StateHasChanged();
    }

    private void RemoveWikiSolution(WikiSolutionEditModel call)
    {
        Model.WikiSolutions.Remove(call);
    }

    private void BackupItem(object element)
    {
        WikiSolutionBeforeEdit = (WikiSolutionEditModel)element;
    }

    private void ResetItemToOriginalValues(object element)
    {
        element = WikiSolutionBeforeEdit;
    }
    
    private async Task<bool> CheckWikiSolutions()
    {
        foreach (var ws in Model.WikiSolutions)
        {
            if (string.IsNullOrWhiteSpace(ws.Name))
            {
                Snackbar.Add("Не указано название решения!", Severity.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(ws.Description))
            {
                Snackbar.Add("Не указано описание решения!", Severity.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(ws.SqlScript))
            {
                Snackbar.Add("Не указан скрипт решения!", Severity.Warning);
                return false;
            }
        }

        return true;
    }

    private async Task OnValidSumbit()
    {
        if (!await CheckWikiSolutions())
        {
            return;
        }

        try
        {
            var result = IsEdit
                ? await ModelController.Update(ID.GetValueOrDefault(), Model) // todo: fix
                : await ModelController.Create(Model);

            Model = Mapper.Map<WikiEditModel>(result);
            ID = result.ID;

            if (UseNavigations)
            {
                NavigationManager.NavigateTo(NavigationUrl);
            }
            Snackbar.Add("Wiki успешно сохранено");
        }
        catch (ValidationApiException validationException)
        {
            Snackbar.Add(validationException.Content.Detail, Severity.Error);
        }
        catch (ApiException exception)
        {
            Snackbar.Add(exception.RequestMessage.Content.ToString(), Severity.Error);
        }
    }

    private async Task SaveAsNew()
    {
        if (!await CheckWikiSolutions())
        {
            return;
        }

        try
        {
            var result =
                await ModelController.Create(Model);

            Model = Mapper.Map<WikiEditModel>(result);
            ID = result.ID;

            if (UseNavigations)
            {
                NavigationManager.NavigateTo(NavigationUrl);
            }
            Snackbar.Add("Wiki успешно сохранено");
        }
        catch (ApiException exception)
        {
            Snackbar.Add(exception.RequestMessage.Content.ToString(), Severity.Error);
        }
    }
}