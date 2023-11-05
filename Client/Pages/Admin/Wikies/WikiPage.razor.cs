using Microsoft.AspNetCore.Components;

namespace SmartMonitoring.Client.Pages.Admin.Wikies;

public partial class WikiPage
{
    [Parameter] public Guid? ID { get; set; }

    public bool IsEdit => ID != null;
    public string PageTitle => IsEdit ? "Изменение Wiki" : "Создание Wiki";
}